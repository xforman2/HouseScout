namespace SharedDependencies.Model;

public class User
{
    public long UserId { get; set; }

    public int MinPrice { get; set; }

    public int MaxPrice { get; set; }

    public int MinSurface { get; set; }

    public int MaxSurface { get; set; }

    public EstateType EstateType { get; set; }

    public OfferType OfferType { get; set; }

    public bool IsNew { get; set; }

    public User(
        long userId,
        int minPrice,
        int maxPrice,
        int minSurface,
        int maxSurface,
        EstateType estateType,
        OfferType offerType,
        bool isNew
    )
    {
        UserId = userId;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        MinSurface = minSurface;
        MaxSurface = maxSurface;
        EstateType = estateType;
        OfferType = offerType;
        IsNew = isNew;
    }
}
