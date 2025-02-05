using Microsoft.EntityFrameworkCore;

namespace SharedDependencies.Model;

public class HouseScoutContext : DbContext
{
    public DbSet<Estate> Estates { get; set; }
    public DbSet<User> Users { get; set; }

    public HouseScoutContext(DbContextOptions<HouseScoutContext> options)
        : base(options) { }
}
