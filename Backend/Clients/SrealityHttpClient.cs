using Backend.DTOs;
using Newtonsoft.Json;

namespace Backend.Clients
{
    public class SrealityHttpClient : IClient
    {
        private const string SREALITY_ENDPOINT =
            "https://www.sreality.cz/api/cs/v2/estates?category_main_cb=1&category_type_cb=2&locality_district_id=72&locality_region_id=14&per_page=999";

        public async Task<object> FetchDataAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string responseBody = await client.GetStringAsync(SREALITY_ENDPOINT);

                    return JsonConvert.DeserializeObject<SrealityResponseDTO>(responseBody)
                        ?? throw new InvalidOperationException("Deserialization has failed");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                    throw;
                }
            }
        }
    }
}
