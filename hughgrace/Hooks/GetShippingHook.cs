using System;
using System.Data.SqlClient;
using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders.Shipping;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;

namespace hughgrace.Hooks
{
    public class GetShippingHook : IHook<GetShippingHookRequest, GetShippingHookResponse>
    {
        private readonly IDataService _dataService;
        public GetShippingHook(IDataService dataService)
        {
            _dataService = dataService;
        }

        public GetShippingHookResponse Invoke(GetShippingHookRequest request, Func<GetShippingHookRequest, GetShippingHookResponse> func)
        {
            var routeincluded = false;
            if (request.ShipMethodId > 500)
            {
                request.ShipMethodId -= 500;
                routeincluded = true;
            }

            var response = func(request);
            if (routeincluded)
            {
                double subtotal = 0;
                foreach (var item in request.Items)
                {
                    subtotal += item.Price * item.Quantity;
                }

                using(var dbConneciton = new SqlConnection(_dataService.ClientConnectionString.ToString()))
                {
                    var query = string.Format(@"SELECT * FROM RouteRate
                        WHERE MinimumChargeAmount <= {0}
                        ORDER BY MinimumChargeAmount DESC;", subtotal);
                    var routeInsurance = dbConneciton.QueryFirstOrDefault<RouteRate>(query);
                    if (routeInsurance != null)
                    {
                        response.ShippingAmount += routeInsurance.Rate;
                    }   
                }
            }

            return response;
        }
    }
}
