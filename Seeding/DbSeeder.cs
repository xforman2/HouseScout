using Bogus;
using HouseScout.Model;
using Microsoft.EntityFrameworkCore;

namespace HouseScout.Seeding;

public class DbSeeder
{
    private readonly HouseScoutContext _context;

    public DbSeeder(HouseScoutContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Estates.Any())
        {
            var estateFaker = new Faker<Estate>()
                .RuleFor(e => e.ApiType, f => f.PickRandom<ApiType>())
                .RuleFor(e => e.ApiId, f => f.Random.Guid().ToString())
                .RuleFor(e => e.Address, f => f.Address.FullAddress())
                .RuleFor(e => e.Price, f => f.Random.Decimal(100000, 1000000))
                .RuleFor(e => e.Link, f => f.Internet.Url())
                .RuleFor(e => e.Surface, f => f.Random.Double(50, 500))
                .RuleFor(e => e.EstateType, f => f.PickRandom<EstateType>())
                .RuleFor(e => e.OfferType, f => f.PickRandom<OfferType>());

            var estates = estateFaker.Generate(500);

            _context.Estates.AddRange(estates);
            _context.SaveChanges();
        }
    }
}
