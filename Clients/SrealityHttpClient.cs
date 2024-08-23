using Newtonsoft.Json;

namespace HouseScout.Clients
{
    public class SrealityHttpClient
    {
        public async Task<SrealityResponse> GetSrealityData()
        {
            string url = "https://www.sreality.cz/api/cs/v2/estates?category_main_cb=1&category_type_cb=2&locality_district_id=72&locality_region_id=14&per_page=999";
            
            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    
                    response.EnsureSuccessStatusCode();
                    
                    string responseBody = await response.Content.ReadAsStringAsync();
                    
                    return JsonConvert.DeserializeObject<SrealityResponse>(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                    return null;
                }
            }
        }
    }

    public class EstateDTO
    {
        [JsonProperty("hash_id")]
        public string Id { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
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

    public class Embedded
    {
        [JsonProperty("estates")]
        public List<EstateDTO> Estates { get; set; }
    }

    public class SrealityResponse
    {
        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; }
    }

}
