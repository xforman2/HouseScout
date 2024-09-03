using HouseScout.Model;

namespace SharedDependencies.Model;

public class User
{
    public int UserId { get; set; }

    public int MinPrice { get; set; }

    public int MaxPrice { get; set; }

    public int MinSurface { get; set; }

    public int MaxSurface { get; set; }

    public EstateType EstateType { get; set; }

    public OfferType OfferType { get; set; }
}
