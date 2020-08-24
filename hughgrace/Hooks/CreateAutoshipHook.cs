using System;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;

namespace hughgrace.Hooks
{
    public class CreateAutoshipHook : IHook<CreateAutoshipHookRequest, CreateAutoshipHookResponse>
    {
        private readonly IAutoshipService _autoshipService;
        private readonly IAssociateService _associateService;

        public CreateAutoshipHook(IAutoshipService autoshipService, IAssociateService associateService)
        {
            _autoshipService = autoshipService;
            _associateService = associateService;
        }

        public CreateAutoshipHookResponse Invoke(CreateAutoshipHookRequest request, Func<CreateAutoshipHookRequest, CreateAutoshipHookResponse> func)
        {
            if (request.AutoshipInfo.ShipMethodId > 500)
            {
                if (request.AutoshipInfo.Custom == null)
                {
                    request.AutoshipInfo.Custom = new CustomFields();
                }

                request.AutoshipInfo.Custom.Field1 = "Route";
            }

            var response = func(request);

            //_orderService.UpdateOrder(new UpdateOrderInfo() { OrderNumber = response.OrderNumber, SpecialInstructions = routeInstructions });
            //request.AutoshipInfo
            //_autoshipService.UpdateAutoship(request.AutoshipInfo);

            return response;
        }
    }
}
