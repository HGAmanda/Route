using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;

namespace hughgrace.Api
{
    public class QueryTableEndpoint : IApiEndpoint
    {
        private readonly IDataService _dataService;

        public QueryTableEndpoint(IDataService dataService)
        {
            _dataService = dataService;
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgraceServices/queryTable",
                RequireAuthentication = true
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            IEnumerable<RouteRate> rates;
            using (var connection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                connection.Open();

                rates = connection.Query<RouteRate>("SELECT * FROM [client].[RouteRate]");
            }

            return new Ok(new { Status = 1, Rates = rates });
        }
    }
}
