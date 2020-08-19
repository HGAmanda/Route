using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;

namespace hughgrace.Api
{
    public class DeleteTableEndpoint : IApiEndpoint
    {
        private readonly IDataService _dataService;

        public DeleteTableEndpoint(IDataService dataService)
        {
            _dataService = dataService;
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgraceServices/deleteTable",
                RequireAuthentication = true
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            int affect;
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                var commandStr = @"DROP TABLE RouteRate";
                dbConnection.Open();
                using (var command = new SqlCommand(commandStr, dbConnection))
                    affect = command.ExecuteNonQuery();
            }

            return new Ok(new { Status = 1, RequestMessage = affect });
        }
    }
}
