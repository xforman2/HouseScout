using Backend.Clients;
using Backend.Mappers;
using HouseScout.Model;
using SharedDependencies.Model;
using SharedDependencies.Services;

namespace Backend.Services;

public class DataProcessingService
{
    private readonly Dictionary<IClient, IMapper> _clientsAndMappers;
    private readonly HouseScoutContext _context;
    private readonly RabbitMQService _rabbitMQService;

    public DataProcessingService(
        Dictionary<IClient, IMapper> clientsAndMappers,
        HouseScoutContext context,
        RabbitMQService rabbitMqService
    )
    {
        _clientsAndMappers = clientsAndMappers;
        _context = context;
        _rabbitMQService = rabbitMqService;
    }

    public async Task ProcessData()
    {
        var existingEstates = _context.Estates.ToDictionary(e => e.ApiId);

        var fetchedEstatesDictionary = new Dictionary<string, Estate>();

        foreach (var kvp in _clientsAndMappers)
        {
            IClient client = kvp.Key;
            IMapper mapper = kvp.Value;

            var data = await client.FetchDataAsync();
            var fetchedEstates = mapper.MapResponseToModel(data);
            foreach (var estate in fetchedEstates)
            {
                fetchedEstatesDictionary[estate.ApiId] = estate;
            }
        }

        var estatesToAdd = new List<Estate>();
        var estatesToRemove = new List<Estate>();

        foreach (var estate in fetchedEstatesDictionary.Values)
        {
            if (!existingEstates.ContainsKey(estate.ApiId))
            {
                estate.New = true;
                estatesToAdd.Add(estate);
            }
        }

        foreach (var estate in existingEstates.Values)
        {
            if (fetchedEstatesDictionary.ContainsKey(estate.ApiId))
            {
                estate.New = false;
            }
            else
            {
                estatesToRemove.Add(estate);
            }
        }
        if (estatesToAdd.Count > 0)
        {
            await _context.Estates.AddRangeAsync(estatesToAdd);
        }

        if (estatesToRemove.Count > 0)
        {
            _context.Estates.RemoveRange(estatesToRemove);
        }
        await _context.SaveChangesAsync();

        _rabbitMQService.PublishMessage("Hello, world from DataProcessingService!");
    }
}
