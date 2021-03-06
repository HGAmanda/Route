﻿using DirectScale.Disco.Extension;
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
            var hookResponse = func(request);
            var usedCoupons = hookResponse.OrderCoupons.UsedCoupons.ToList();
            var totalDiscount = 0.0;
            if (request.LineItems.Any(x=> x.SKU == "STRKIT001" || x.SKU == "STRKIT002" || x.SKU == "STRKIT003"))
            {
                List<LineItem> includedItems = request.LineItems.Where(x => x.SKU != "STRKIT001" && x.SKU != "STRKIT002" && x.SKU != "STRKIT003").ToList();
                totalDiscount = includedItems.Sum(x => x.ExtendedPrice)*25/100;
            }

            OrderCoupons coupons = new OrderCoupons { DiscountTotal = totalDiscount, UsedCoupons = usedCoupons.ToArray() };

            return new ProcessCouponCodesHookResponse
            {
                OrderCoupons = coupons,
                CouponAdjustedTax = hookResponse.CouponAdjustedTax
            };
        }

    }



}
