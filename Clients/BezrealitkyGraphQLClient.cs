using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using HouseScout.DTOs;
using HouseScout.Model;

namespace HouseScout.Clients
{
    public class BezrealitkyGraphQLClient : IClient
    {
        private const string BEZREALITKY_ENDPOINT = "https://api.bezrealitky.cz/graphql/";
        private const string QUERY =
            @"
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
        private readonly GraphQLHttpClient _client;

        public BezrealitkyGraphQLClient()
        {
            _client = new GraphQLHttpClient(BEZREALITKY_ENDPOINT, new NewtonsoftJsonSerializer());
        }

        public async Task<object> FetchDataAsync()
        {
            var request = new GraphQLRequest { Query = QUERY };

            var response = await _client.SendQueryAsync<BezrealitkyResponseDTO>(request);

            if (response.Errors != null && response.Errors.Length > 0)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine($"Error: {error.Message}");
                }
            }

            return response.Data;
        }
    }
}
