using Bogus;
using SharedDependencies.Model;

namespace Backend.Seeding;

public class DbSeeder
{
    private readonly HouseScoutContext _context;

    public DbSeeder(HouseScoutContext context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        if (!_context.Estates.Any())
        {
            var estateFaker = new Faker<Estate>()
                .RuleFor(e => e.ApiType, f => f.PickRandom<ApiType>())
                .RuleFor(e => e.ApiId, f => f.Random.Guid().ToString())
                .RuleFor(e => e.Address, f => f.Address.FullAddress())
                .RuleFor(e => e.Price, f => f.Random.Decimal(10000, 100000))
                .RuleFor(e => e.Link, f => f.Internet.Url())
                .RuleFor(e => e.Surface, f => f.Random.Double(10, 100))
                .RuleFor(e => e.EstateType, f => f.PickRandom<EstateType>())
                .RuleFor(e => e.OfferType, f => f.PickRandom<OfferType>())
                .RuleFor(e => e.New, f => true);

            var estates = estateFaker.Generate(200);

            await _context.Estates.AddRangeAsync(estates);
            await _context.SaveChangesAsync();
        }
    }
}
