namespace HouseScout.Model;

public class Advert
{
    public string Id { get; set; }
    public string Uri { get; set; }
    public string Address { get; set; }
    public string OfferType { get; set; }
    public string EstateType { get; set; }
    public int ServiceCharges { get; set; }
    public int UtilityCharges { get; set; }
    public int Surface { get; set; }
    public int Charges { get; set; }
    public int Price { get; set; }
}