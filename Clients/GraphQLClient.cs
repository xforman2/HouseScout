using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using HouseScout.Model;
using System;
using System.Threading.Tasks;

namespace HouseScout.Clients
{
    public class GraphQLClient
    {
        private readonly GraphQLHttpClient _client;

        public GraphQLClient()
        {
            _client = new GraphQLHttpClient("https://api.bezrealitky.cz/graphql/", new NewtonsoftJsonSerializer());
        }

        public async Task<AdvertsResponse> GetAdvertsAsync()
        {
            var query = @"
            query ListAdverts {
                listAdverts(
                    offerType: PRONAJEM
                    estateType: BYT
                    order: TIMEORDER_DESC
                    regionOsmIds: ""R438171""
                    limit: 100000
                ) {
                    list {
                        id
                        uri
                        address(locale: CS)
                        offerType
                        estateType
                        serviceCharges
                        utilityCharges
                        surface
                        charges
                        price
                    }
                }
            }";

            var request = new GraphQLRequest
            {
                Query = query
            };
            
            var response = await _client.SendQueryAsync<AdvertsResponse>(request);
            
            if (response.Errors != null && response.Errors.Length > 0)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine($"Error: {error.Message}");
                }

                // TODO throw exception
                return null;
            }

            return response.Data;
        }
    }
    
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

    public class ListAdverts
    {
        public List<Advert> List { get; set; }
    }

    public class AdvertsResponse
    {
        public ListAdverts ListAdverts { get; set; }
    }
}
