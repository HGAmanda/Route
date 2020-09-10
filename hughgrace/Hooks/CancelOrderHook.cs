using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;
using hughgrace.Models;
using Newtonsoft.Json;

namespace hughgrace.Hooks
{
    public class CancelOrderHook : IHook<CancelOrderHookRequest, CancelOrderHookResponse>
    {
        private const string URL = "https://directscale-stage.ngrok.io/";
        private readonly IOrderService _orderService;
        private readonly IAssociateService _associateService;

        public CancelOrderHook(IOrderService orderService, IAssociateService associateService)
        {
            _orderService = orderService;
            _associateService = associateService;
        }

        public CancelOrderHookResponse Invoke(CancelOrderHookRequest request, Func<CancelOrderHookRequest, CancelOrderHookResponse> func)
        {
            var response = func(request);

            var order = _orderService.GetOrderByOrderNumber(request.OrderNumber);
            if (string.IsNullOrEmpty(order.SpecialInstructions) || !order.SpecialInstructions.Contains("Route Shipping Protection"))
            {
                return response;
            }

            var associate = _associateService.GetAssociate(order.AssociateId);
            var warehouseId = _orderService.GetWarehouseId(associate.Address);
            var orderDateRFC3339 = order.OrderDate.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
            var currencyCode = "USD";

            if (order.LineItems.Count > 0 && !string.IsNullOrEmpty(order.LineItems[0].CurrencyCode)) {
                currencyCode = order.LineItems[0].CurrencyCode;
            }

            var req = new CancelRouteOrder
            {
                OrderId = request.OrderNumber.ToString(),
                OrderDateUtc = orderDateRFC3339,
                DistributorId = warehouseId.ToString(),
                OrderType = order.OrderType.ToString(),
                OrderTotal = order.TotalQV,
                OrderCountry = associate.Address.CountryCode,
                OrderCurrency = currencyCode,
                OrderStatus = order.Status,
                EventType = "",
                EventDateUtc = orderDateRFC3339
            };

            SendRequestToRoute(req);

            return response;
        }

        private void SendRequestToRoute(CancelRouteOrder request)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _ = client.PostAsync("/webhooks/orders/HughGrace", content).Result;

            client.Dispose();
        }
    }
}
