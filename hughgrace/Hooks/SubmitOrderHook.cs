﻿using System;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;

namespace hughgrace.Hooks
{
    public class SubmitOrderHook : IHook<SubmitOrderHookRequest, SubmitOrderHookResponse>
    {
        private readonly IOrderService _orderService;

        public SubmitOrderHook(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public SubmitOrderHookResponse Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, SubmitOrderHookResponse> func)
        {
            var routeIncluded = request.Order.ShipMethodId > 500;
            if (routeIncluded)
            {
                request.Order.Custom.Field1 = "Route";
            }

            var response = func(request);

            if (routeIncluded)
            {
                _orderService.UpdateOrder(new UpdateOrderInfo() { OrderNumber = response.OrderNumber, SpecialInstructions = "Route was used on this Order!" });
            }

            return response;
        }
    }
}
