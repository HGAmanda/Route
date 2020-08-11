using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks.Orders.Shipping;

namespace hughgrace.Services
{
  public interface IRouteService
  {
    SubmitOrderHookResponse Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, SubmitOrderHookResponse> func);
    SubmitOrderHookResponse Invoke(GetShippingHookRequest request, Func<GetShippingHookRequest, GetShippingHookResponse> func);
  }
}
