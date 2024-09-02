using System.Diagnostics;
using Backend.Clients;
using Backend.Mappers;
using HouseScout.Model;
using SharedDependencies.Model;

namespace Backend.Services;

public class DataProcessingService
{
    private readonly Dictionary<IClient, IMapper> _clientsAndMappers;
    private readonly HouseScoutContext _context;

    public DataProcessingService(
        Dictionary<IClient, IMapper> clientsAndMappers,
        HouseScoutContext context
    )
    {
        _clientsAndMappers = clientsAndMappers;
        _context = context;
    }

    public async Task ProcessData()
    {
        var existingEstates = _context.Estates.ToList();

        var fetchedEstates = new List<Estate>();

        foreach (var kvp in _clientsAndMappers)
        {
            IClient client = kvp.Key;
            IMapper mapper = kvp.Value;

            var data = await client.FetchDataAsync();
            fetchedEstates.AddRange(mapper.MapResponseToModel(data));
        }

        var fetchedEstatesSet = new HashSet<string>(fetchedEstates.Select(fe => fe.ApiId));
        var existingEstatesSet = new HashSet<string>(existingEstates.Select(fe => fe.ApiId));

        var estatesToAdd = fetchedEstates
            .Where(fe => !existingEstatesSet.Contains(fe.ApiId))
            .ToList();

        var estatesToRemove = existingEstates
            .Where(ee => !fetchedEstatesSet.Contains(ee.ApiId))
            .ToList();

        if (estatesToAdd.Any())
        {
            await _context.Estates.AddRangeAsync(estatesToAdd);
        }

        if (estatesToRemove.Any())
        {
            _context.Estates.RemoveRange(estatesToRemove);
        }
        await _context.SaveChangesAsync();
    }
}
