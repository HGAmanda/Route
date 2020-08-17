using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;
using System.Text;
using Dapper;

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
            var updateAffect = 0;
            var sqlRaw = "";
            var req = _requestParsing.ParseBody<RouteRateRequest>(request);

            if (req.Rates.Length == 0) {
                return new Ok();
            }

            using (var conn = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                {
                    var sql = new StringBuilder();
                    var count = conn.QueryFirstOrDefault<int>(@"SELECT COUNT(RecordNumber) FROM RouteRate");
                    if (count == 0)
                    {
                        sql.Append(@"INSERT INTO RouteRate (Rate, MinimumChargeAmount, RecordNumber) VALUES ");
                    } else
                    {
                        sql.Append(@"CREATE TABLE #tempTable (
	                        Rate decimal NOT NULL,
	                        MinimumChargeAmount decimal NOT NULL,
	                        RecordNumber int NOT NULL PRIMARY KEY
                        )

                        INSERT INTO #tempTable (Rate, MinimumChargeAmount, RecordNumber) VALUES ");
                    }

                    for (int i = 0; i < req.Rates.Length; i++)
                    {
                        var rate = req.Rates[i];
                        sql.AppendFormat("({0},{1},{2}),", rate.Rate, rate.MinimumChargeAmount, i);
                    }
                    sql.Length--; // erase last ","

                    sqlRaw = sql.ToString();
                    using (var cmd = new SqlCommand(sqlRaw, conn, trans))  
                    {
                        affect = cmd.ExecuteNonQuery();
                    }

                    if (count == 0)
                    {
                        var updateSql = @"UPDATE
                                T1
                            SET
                                [Rate] = T2.[Rate]
                                [MinimumChargeAmount] = T2.[MinimumChargeAmount]
                            FROM
                                RouteRate T1
                                JOIN
                                #tempTable T2 ON T2.[RecordNumber] = T1.[RecordNumber]";

                        using (var cmd = new SqlCommand(updateSql, conn, trans))
                        {
                            updateAffect = cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            

            return new Ok(new { Status = 1, SqlRaw = sqlRaw, InsertAffect = affect, UpdateAffect = updateAffect });
        }

        public class RouteRateRequest
        {
            public RouteRate[] Rates { get; set; }
        }
    }
}
