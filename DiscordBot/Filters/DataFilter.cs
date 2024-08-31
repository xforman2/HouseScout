using HouseScout.Model;
using SharedDependencies.Model;

namespace HouseScout.Filters;

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
        int surfaceMax
    )
    {
        return _context
            .Estates.Where(e =>
                e.Price >= priceMin
                && e.Price <= priceMax
                && e.Surface >= surfaceMin
                && e.Surface <= surfaceMax
            )
            .ToList();
    }
}
