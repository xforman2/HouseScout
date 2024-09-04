namespace SharedDependencies.Model;

public class Estate
{
    public int Id { get; set; }
    public ApiType ApiType { get; set; }
    public string ApiId { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public string Link { get; set; }
    public double Surface { get; set; }
    public EstateType EstateType { get; set; }
    public OfferType OfferType { get; set; }

    public bool New { get; set; }

    // for seeding to work, do not remove
    public Estate() { }

    public Estate(
        ApiType apiType,
        string apiId,
        string address,
        decimal price,
        string link,
        double surface,
        EstateType estateType,
        OfferType offerType
    )
    {
        ApiType = apiType;
        ApiId = apiId;
        Address = address;
        Price = price;
        Link = link;
        Surface = surface;
        EstateType = estateType;
        OfferType = offerType;
    }
}
