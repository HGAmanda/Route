using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using System.Data;

namespace hughgrace.Api
{
    public class CreateTableEndpoint : IApiEndpoint
    {
        private readonly IDataService _dataService;

        public CreateTableEndpoint(IDataService dataService)
        {
            _dataService = dataService;
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgraceServices/table",
                RequireAuthentication = true
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            int affect;
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                var commandStr = @"CREATE TABLE RouteRate ( Rate decimal, MinimumChargeAmount decimal, RecordNumber int);";
                dbConnection.Open();
                using (var command = new SqlCommand(commandStr, dbConnection))
                    affect = command.ExecuteNonQuery();
            }
            // var rObject = _requestParsing.ParseBody<ExampleRequest>(request);

            return new Ok(new { Status = 1, RequestMessage = affect });
        }
    }
}
