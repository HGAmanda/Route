using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using System.Data;

namespace hughgrace.Api
{
    public class ApiExample : IApiEndpoint
    {
        private readonly IAssociateService _associateService;
        private readonly IRequestParsingService _requestParsing;
        private IDataService _dataService;
        private DataSet _dataSet;

        public ApiExample(IAssociateService associateService, IDataService dataService, IRequestParsingService requestParsing)
        {
            _associateService = associateService;
            _requestParsing = requestParsing;
            _dataService = dataService;
            _dataSet = new DataSet()
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgraceServices/createTableExample",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            int affect;
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                var commandStr = @"CREATE TABLE IF NOT EXISTS RouteRate (
                      Rate DOUBLE,
                      MinimumChargeAmount DOUBLE,
                      RecordNumber INT
                    );";
                using (var command = new SqlCommand(commandStr, dbConnection))
                    affect = command.ExecuteNonQuery();
            }
            ////var rObject = _requestParsing.ParseBody<ExampleRequest>(request);
            ////var aName = _associateService.GetAssociate(rObject.BackOfficeId).Name;

            //var response = new ExampleResponse { Status = 1, RequestMessage = "Route" };
            return new Ok(new { Status = 1, RequestMessage = affect });
        }
    }

    public class ExampleResponse
    {
        public int Status { get; set; }
        public string RequestMessage { get; set; }
    }


    public class ExampleRequest
    {
        public string Message { get; set; }
        public string BackOfficeId { get; set; }
    }
}
