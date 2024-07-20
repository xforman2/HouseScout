using Microsoft.EntityFrameworkCore;

namespace HouseScout.model;

public class HouseScoutContext : DbContext
{
    
    
    public DbSet<Estate> Estates { get; set; }
    
    
    public HouseScoutContext(DbContextOptions<HouseScoutContext> options)
        : base(options)
    { }


}