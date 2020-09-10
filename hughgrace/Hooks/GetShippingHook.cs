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
                    if (item.ChargeShipping)
                    {
                        subtotal += item.ExtendedPrice;
                    }
                }

                if (subtotal != 0)
                {
                    var query = "SELECT TOP 1 Rate, MinimumChargeAmount FROM RouteRate WHERE MinimumChargeAmount >= @MinimumChargeAmount ORDER BY MinimumChargeAmount ASC";
                    using (var connection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
                    {
                        var routeInsurance = connection.QueryFirstOrDefault<RouteRate>(query, new { MinimumChargeAmount = subtotal });
                        if (routeInsurance != null)
                        {
                            response.ShippingAmount += routeInsurance.Rate;
                        }
                    }
                }
            }

            return response;
        }
    }
}
