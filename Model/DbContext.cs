using Microsoft.EntityFrameworkCore;

namespace HouseScout.Model;

public class HouseScoutContext : DbContext
{
    public DbSet<Estate> Estates { get; set; }

    public HouseScoutContext(DbContextOptions<HouseScoutContext> options)
        : base(options) { }
}
