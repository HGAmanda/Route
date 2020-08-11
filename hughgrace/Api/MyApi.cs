using Dapper;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using System.Data;
using System.Linq;

namespace Route.Api
{
  public class Endpoint1 : IApiEndpoint
  {
    private readonly IAssociateService _associateService;
    private readonly IRequestParsingService _requestParsing;
    private readonly IDataService _dataService;

    public Endpoint1(IAssociateService associateService, IDataService dataService, IRequestParsingService requestParsing)
    {
      _associateService = associateService;
      _requestParsing = requestParsing;
      _dataService = dataService;
    }

    public ApiDefinition GetDefinition()
    {
      return new ApiDefinition
      {
        Route = "demo/end1",
        RequireAuthentication = false
      };
    }

    public IApiResponse Post(ApiRequest request)
    {
      using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.dbConnection.ConnectionString))
      {
        var rObject = _requestParsing.ParseBody<E1Request>(request);
        //var aName = _associateService.GetAssociate(rObject.BackOfficeId).Name;

        var sql = $"select FirstName, LastName, BackOfficeId from CRM_Distributors where recordnumber = '{rObject.BackOfficeId}'"; //Note. This is subject to SQL Injection. Do not use in production.
        var qryRes = dbConnection.Query<QryResult>(sql).FirstOrDefault();
        var aName = $"{qryRes.FirstName} {qryRes.LastName}";

        return new Ok(new { Status = 1, RequestMessage = rObject.Message, AssociateName = aName });
      }
    }
  }

  public class QryResult
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BackOfficeId { get; set; }
  }

  public class E1Request
  {
    public string Message { get; set; }
    public string BackOfficeId { get; set; }
  }
}
