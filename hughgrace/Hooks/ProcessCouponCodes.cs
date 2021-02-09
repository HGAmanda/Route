using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hughgrace.Hooks
{
    public class ProcessCouponCodes : IHook<ProcessCouponCodesHookRequest, ProcessCouponCodesHookResponse>
    {
        public ProcessCouponCodesHookResponse
           Invoke(ProcessCouponCodesHookRequest request, Func<ProcessCouponCodesHookRequest, ProcessCouponCodesHookResponse> func)
        {
            if(request.LineItems.Any(x=> x.SKU == "STRKIT001" || x.SKU == "STRKIT002" || x.SKU == "STRKIT003"))
            {
                List<LineItem> includedItems = request.LineItems.Where(x => x.SKU != "STRKIT001" && x.SKU != "STRKIT002" && x.SKU != "STRKIT003").ToList();

                var totalDiscount = includedItems.Sum(x => x.ExtendedPrice)%25;

                OrderCoupons coupons = new OrderCoupons {DiscountTotal = totalDiscount };

                return new ProcessCouponCodesHookResponse
                {
                    OrderCoupons = coupons
                };
            }

            return new ProcessCouponCodesHookResponse
            {
                
            };
        }

    }
}
