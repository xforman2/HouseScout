using SharedDependencies.Model;

namespace DiscordBot.Filters;

public class DataFilter
{
    private HouseScoutContext _context;

    public DataFilter(HouseScoutContext context)
    {
        _context = context;
    }

    public List<Estate> SurfacePriceFilter(
        int priceMin,
        int priceMax,
        int surfaceMin,
        int surfaceMax,
        bool isNewUser
    )
    {
        var query = _context.Estates.AsQueryable();

        // we want to process only new data if user is not new,
        // and all data if user is new
        if (!isNewUser)
        {
            query = query.Where(e => e.IsNew);
        }
        query = query.Where(e =>
            e.Price >= priceMin
            && e.Price <= priceMax
            && e.Surface >= surfaceMin
            && e.Surface <= surfaceMax
        );

        return query.ToList();
    }
}
