using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using System;

namespace hughgrace.Hooks
{
    public class GetCouponVolume : IHook<GetCouponAdjustedVolumeHookRequest, GetCouponAdjustedVolumeHookResponse>
    {
        public GetCouponAdjustedVolumeHookResponse
            Invoke(GetCouponAdjustedVolumeHookRequest request, Func<GetCouponAdjustedVolumeHookRequest, GetCouponAdjustedVolumeHookResponse> func)
        {
            return new GetCouponAdjustedVolumeHookResponse
            {
                CouponAdjustedVolume = new CouponAdjustedVolume
                { Cv = request.Volume.Cv, Qv = request.Volume.Qv }
            };
        }
    }
}
