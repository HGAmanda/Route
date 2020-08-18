using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;
using System.Text;
using Dapper;
using System;
using System.Net;

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

        private string CountRouteRateRows()
        {
            return "SELECT COUNT(RecordNumber) FROM RouteRate";
        }

        private string InsertRouteRateTable()
        {
            return "INSERT INTO RouteRate (Rate, MinimumChargeAmount, RecordNumber) VALUES ";
        }

        private string DeleteAllRouteRate()
        {
            return @"DELETE FROM RouteRate;";
        }

        private string UpdateRouteRateTable()
        {
            return @"UPDATE
                T1
            SET
                [Rate] = T2.[Rate]
                [MinimumChargeAmount] = T2.[MinimumChargeAmount]
            FROM
                RouteRate T1
            JOIN
                #tempTable T2 ON T2.[RecordNumber] = T1.[RecordNumber]";
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

            using (var connection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
            {
                connection.Open();

                var count = connection.QueryFirstOrDefault<int>(CountRouteRateRows());

                var command = connection.CreateCommand();
                SqlTransaction transaction;

                transaction = connection.BeginTransaction("RouteTransaction");

                command.Connection = connection;
                command.Transaction = transaction;

                var sql = new StringBuilder();

                if (count == 0)
                {
                    sql.Append(InsertRouteRateTable());
                } else
                {
                    sql.Append(DeleteAllRouteRate());
                    sql.Append(InsertRouteRateTable());
                }

                for (int i = 0; i < req.Rates.Length; i++)
                {
                    var rate = req.Rates[i];
                    sql.AppendFormat("({0},{1},{2}),", rate.Rate, rate.MinimumChargeAmount, i);
                }
                sql.Length--; // erase last ","

                sqlRaw = sql.ToString();

                command.CommandText = sqlRaw;
                affect = command.ExecuteNonQuery();

                if (count != 0)
                {
                    command.CommandText = UpdateRouteRateTable();
                    updateAffect = command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            return new Ok(new { Status = 1, SqlRaw = sqlRaw, InsertAffect = affect, UpdateAffect = updateAffect });
        }

        public class RouteRateRequest
        {
            public RouteRate[] Rates { get; set; }
        }
    }
}
