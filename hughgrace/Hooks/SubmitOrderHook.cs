using System;
using System.Data.SqlClient;
using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;

namespace hughgrace.Hooks
{
    public class SubmitOrderHook : IHook<SubmitOrderHookRequest, SubmitOrderHookResponse>
    {
        private readonly IOrderService _orderService;
        private readonly IDataService _dataService;

        public SubmitOrderHook(IOrderService orderService, IDataService dataService)
        {
            _orderService = orderService;
            _dataService = dataService;
        }

        public SubmitOrderHookResponse Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, SubmitOrderHookResponse> func)
        {
            if (request.Order.ShipMethodId > 500)
            {
                if (request.Order.Custom == null)
                {
                    request.Order.Custom = new CustomFields();
                }

                request.Order.Custom.Field1 = "Route";
            }

            var response = func(request);

            if (request.Order.ShipMethodId > 500)
            {
                RouteRate routeInsurance;
                double subtotal = 0;
                foreach (var item in request.Order.LineItems)
                {
                    subtotal += item.Price * item.Quantity;
                }

                var query = "SELECT TOP 1 Rate, MinimumChargeAmount FROM RouteRate WHERE MinimumChargeAmount >= @MinimumChargeAmount ORDER BY MinimumChargeAmount ASC";
                using (var connection = new SqlConnection(_dataService.ClientConnectionString.ToString()))
                {
                    routeInsurance = connection.QueryFirstOrDefault<RouteRate>(query, new { MinimumChargeAmount = subtotal });
                }

                var routeInstructions = string.Format("Route Shipping Protection - $ {0}", routeInsurance.Rate); 
                _orderService.UpdateOrder(new UpdateOrderInfo() { OrderNumber = response.OrderNumber, SpecialInstructions = routeInstructions });
            }

            return response;
        }
    }
}
