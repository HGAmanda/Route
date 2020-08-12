using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;

namespace hughgrace.Api
{
    public class ApiExample : IApiEndpoint
    {
        private readonly IAssociateService _associateService;
        private readonly IRequestParsingService _requestParsing;
        private readonly IDataService _dataService;

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
                Route = "hughgrace/example",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                //var sql = $"select FirstName, LastName, BackOfficeId from CRM_Distributors where recordnumber = '{rObject.BackOfficeId}'"; //Note. This is subject to SQL Injection. Do not use in production.
                //var qryRes = dbConnection.Query<QryResult>(sql).FirstOrDefault();
                //var aName = $"{qryRes.FirstName} {qryRes.LastName}";

                var rObject = _requestParsing.ParseBody<ExampleRequest>(request);
                var aName = _associateService.GetAssociate(rObject.BackOfficeId).Name;

                return new Ok(new { Status = 1, RequestMessage = rObject.Message, AssociateName = aName });
            }
        }
    }

    public class ExampleRequest
    {
        public string Message { get; set; }
        public string BackOfficeId { get; set; }
    }
}
