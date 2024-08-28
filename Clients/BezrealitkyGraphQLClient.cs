using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using HouseScout.Model;
using System;
using System.Threading.Tasks;
using HouseScout.DTOs;

namespace HouseScout.Clients
{
    public class BezrealitkyGraphQLClient : IClient
    {
        private readonly GraphQLHttpClient _client;

        public BezrealitkyGraphQLClient()
        {
            _client = new GraphQLHttpClient("https://api.bezrealitky.cz/graphql/", new NewtonsoftJsonSerializer());
        }

        public async Task<object> FetchDataAsync()
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

            var response = await _client.SendQueryAsync<BezrealitkyResponseDTO>(request);

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
}
