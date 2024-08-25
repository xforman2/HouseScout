using Newtonsoft.Json;

namespace HouseScout.DTOs;
public class SrealityResponseDTO
{
    [JsonProperty("_embedded")]
    public Embedded Embedded { get; set; }
}
public class Embedded
{
    [JsonProperty("estates")]
    public List<SrealityEstate> Estates { get; set; }
}

public class SrealityEstate
{
    [JsonProperty("hash_id")]
    public string Id { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("locality")]
    public string Locality { get; set; }
    [JsonProperty("seo")]
    public Seo Seo { get; set; }
        
}

public class Seo
{
    [JsonProperty("category_main_cb")]
    public int CategoryMainCb { get; set; }

    [JsonProperty("category_sub_cb")]
    public int CategorySubCb { get; set; }

    [JsonProperty("category_type_cb")]
    public int CategoryTypeCb { get; set; }

    [JsonProperty("locality")]
    public string Locality { get; set; }
}