namespace HouseScout.Model;
    
public class Estate
{
    public int Id { get; set; }
    public string Api { get; set; }
    public string ApiId { get; set; }
    public string Address { get; set; }
    public double Price { get; set; }
    public string Link { get; set; }
    public double Surface { get; set; }
    public string EstateType { get; set; }
    public string OfferType { get; set; }

    // for seeding to work, do not remove
    public Estate()
    {
        
    }
    public Estate(string api, string apiId, string address, double price, string link, double surface, string estateType, string offerType)
    {
        Api = api;
        ApiId = apiId;
        Address = address;
        Price = price;
        Link = link;
        Surface = surface;
        EstateType = estateType;
        OfferType = offerType;
    }
}