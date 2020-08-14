using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using Dapper;
using ServiceStack.OrmLite;
using hughgrace.Models;

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
        Route = "hughgraceServices/createTableExample",
        RequireAuthentication = false
      };
    }

    public IApiResponse Post(ApiRequest request)
    {
      // var rObject = _requestParsing.ParseBody<ExampleRequest>(request);
      // var aName = _associateService.GetAssociate(rObject.BackOfficeId).Name;

      return new Ok(new { Status = 1, RequestMessage = "Route" });
      //using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
      //{
      //    dbConnection.CreateTable<RouteRate>();
      //}
    }
  }

  public class ExampleRequest
  {
    public string Message { get; set; }
    public string BackOfficeId { get; set; }
  }
}
