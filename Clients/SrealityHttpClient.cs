using HouseScout.DTOs;
using Newtonsoft.Json;

namespace HouseScout.Clients
{
    public class SrealityHttpClient
    {
        public async Task<SrealityResponseDTO> GetSrealityData()
        {
            string url =
                "https://www.sreality.cz/api/cs/v2/estates?category_main_cb=1&category_type_cb=2&locality_district_id=72&locality_region_id=14&per_page=999";

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<SrealityResponseDTO>(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                    return null;
                }
            }
        }
    }
}
