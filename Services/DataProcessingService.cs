using HouseScout.Clients;
using HouseScout.Mappers;
using HouseScout.Model;

namespace HouseScout.Services;

public class DataProcessingService
{
    private readonly Dictionary<IClient, IMapper> _clientsAndMappers;
    private readonly HouseScoutContext _context;

    public DataProcessingService(Dictionary<IClient, IMapper> clientsAndMappers, HouseScoutContext context)
    {
        _clientsAndMappers = clientsAndMappers;
        _context = context;
    }

    public async Task ProcessData()
    {
        foreach (var kvp in _clientsAndMappers)
        {
            IClient client = kvp.Key;
            IMapper mapper = kvp.Value;

            var data = await client.FetchDataAsync();
            var mappedData = mapper.MapResponseToModel(data);
            _context.Estates.AddRange(mappedData);
            await _context.SaveChangesAsync();
        }
    }
}