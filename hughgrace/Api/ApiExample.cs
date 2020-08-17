using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using Dapper;
using System.Data;

namespace hughgrace.Api
{
    public class ApiExample : IApiEndpoint
    {
        private readonly IAssociateService _associateService;
        private readonly IRequestParsingService _requestParsing;
        private IDataService _dataService;

        public ApiExample(IAssociateService associateService, IDataService dataService, IRequestParsingService requestParsing)
        {
            _associateService = associateService;
            _requestParsing = requestParsing;
            _dataService = dataService;
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
            var affect = 0;
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                var createTable = "CREATE TABLE IF NOT EXISTS RouteRate (Rate DOUBLE, MinimumChargeAmount DOUBLE, RecordNumber INT)";
                affect = dbConnection.Execute(createTable);
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
