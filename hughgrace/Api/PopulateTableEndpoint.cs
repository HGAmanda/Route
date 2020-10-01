using System.Data.SqlClient;
using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;
using System.Text;
using Dapper;
using System;
using System.Net;
using System.Collections.Generic;

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
            return "SELECT COUNT(RecordNumber) FROM [client].[RouteRate]";
        }

        private string InsertRouteRateTable()
        {
            return "INSERT INTO [client].[RouteRate] (Rate, MinimumChargeAmount, RecordNumber) VALUES ";
        }

        private string DeleteAllRouteRate()
        {
            return @"DELETE FROM [client].[RouteRate]";
        }

        public IApiResponse Post(ApiRequest request)
        {
            var insertAffect = 0;
            var deletedAffect = 0;
            IEnumerable<RouteRate> rates;
            string sqlRaw;
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

                try
                {
                    command.Connection = connection;
                    command.Transaction = transaction;

                    var sql = new StringBuilder();

                    if (count != 0)
                    {
                        command.CommandText = DeleteAllRouteRate();
                        deletedAffect = command.ExecuteNonQuery();
                    }
                    
                    sql.Append(InsertRouteRateTable());

                    for (int i = 0; i < req.Rates.Length; i++)
                    {
                        var rate = req.Rates[i];
                        sql.AppendFormat("({0},{1},{2}),", rate.Rate, rate.MinimumChargeAmount, i);
                    }

                    sql.Length--; // erase last ","

                    sqlRaw = sql.ToString();

                    command.CommandText = sqlRaw;
                    insertAffect = command.ExecuteNonQuery();

                    transaction.Commit();
                } catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Ok(new { Status = 500, Message = "Transaction Rolled Back", Error = ex.Message });
                }

                rates = connection.Query<RouteRate>("SELECT * FROM [client].[RouteRate]");
            }

            return new Ok(new { Status = 1, Rates = rates, SQLRawInsert = sqlRaw, InsertAffect = insertAffect, DeletedAffect = deletedAffect });
        }

        public class RouteRateRequest
        {
            public RouteRate[] Rates { get; set; }
        }
    }
}
