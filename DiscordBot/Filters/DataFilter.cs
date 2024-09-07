using SharedDependencies.Model;
using Microsoft.EntityFrameworkCore;
using SharedDependencies.Services;

namespace DiscordBot.Filters
{
    public class DataFilter
    {
        private readonly IDbContextFactory<HouseScoutContext> _contextFactory;

        public DataFilter(IDbContextFactory<HouseScoutContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Estate> SurfacePriceFilter(
            int priceMin,
            int priceMax,
            int surfaceMin,
            int surfaceMax,
            bool isNewUser
        )
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var query = context.Estates.AsQueryable();

                // Process only new data if the user is not new,
                // and all data if the user is new
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
    }
}