using System;
using System.Globalization;
using System.Text.RegularExpressions;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks.Orders.Shipping;

namespace hughgrace.Services
{
  public class RouteService : IRouteService
  {
    private IOrderService _orderService;
    private IDataService _dataService;

    public SubmitOrderHookResponse Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, SubmitOrderHookResponse> func)
    {
      if (request.Order.ShipMethodId > 500)
      {
        request.Order.Custom.Field1 = "Route";
      }

      var response = func(request); // Base logic Sumit Order
      _orderService.UpdateOrder(new UpdateOrderInfo() { OrderNumber = response.OrderNumber, SpecialInstructions = "Route was used on this Order!" });
      //Update OrderPackage with ShipMethod

      //response = ProcessCouponCodesAfter(request, response);
      return response;
    }

    public GetShippingHookResponse Invoke(GetShippingHookRequest request, Func<GetShippingHookRequest, GetShippingHookResponse> func)
    {
      var routeincluded = false;
      if (request.ShipMethodId > 500)
      {
        request.ShipMethodId -= 500;
        routeincluded = true;
      }

      var response = func(request); // Base logic Normal Shipping
      if (routeincluded)
      {
        //get and calcualte Route Fee
        _dataService.ClientConnectionString;
        var query = "selet * from Client.RouteRate";

        response.ShippingAmount += 0;//Route Fee

      }

      return response;
    }
  }
}
