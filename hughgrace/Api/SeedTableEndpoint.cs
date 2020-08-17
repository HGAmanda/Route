using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;
using System.Text;

namespace hughgrace.Api
{
    public class PopulateTableEndpoint : IApiEndpoint
    {
        private readonly IDataService _dataService;
        private readonly IRequestParsingService _requestParsing;

        public PopulateTableEndpoint(IDataService dataService, IRequestParsingService requestParsingService)
        {
            _dataService = dataService;
            _requestParsing = requestParsingService;
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgraceServices/populateTable",
                RequireAuthentication = true
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            var affect = 0;
            var sqlRaw = "";
            var req = _requestParsing.ParseBody<RouteRateRequest>(request);

            if (req.Rates.Length == 0) {
                return new Ok();
            }
            using (var conn = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    var sql = new StringBuilder();
                    sql.Append(@"INSERT INTO RouteRate (Rate, MinimumChargeAmount, RecordNumber) VALUES ");

                    for (int i = 0; i < req.Rates.Length; i++)
                    {
                        var rate = req.Rates[i];
                        sql.AppendFormat("({0},{1},{2}),", rate.Rate, rate.MinimumChargeAmount, i);
                    }
                    sql.Length--; // erase last ","

                    sqlRaw = sql.ToString();
                    using (var cmd = new SqlCommand(sqlRaw, conn, trans))  
                    {
                        conn.Open();
                        affect = cmd.ExecuteNonQuery();
                    }
                }
            }
            

            return new Ok(new { Status = 1, SqlRaw = sqlRaw, RowsAffected = affect });
        }

        public class RouteRateRequest
        {
            public RouteRate[] Rates { get; set; }
        }
    }
}
