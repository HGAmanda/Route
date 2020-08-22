using Microsoft.Extensions.DependencyInjection;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks.Orders.Shipping;
using DirectScale.Disco.Extension.Api;
using hughgrace.Api;
using hughgrace.Hooks;
using DirectScale.Disco.Extension.Hooks.Autoships;

namespace hughgrace
{
    public class ExtensionEntry : IExtension
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IHook<SubmitOrderHookRequest, SubmitOrderHookResponse>, SubmitOrderHook>();
            services.AddScoped<IHook<GetShippingHookRequest, GetShippingHookResponse>, GetShippingHook>();
            services.AddScoped<IHook<CancelOrderHookRequest, CancelOrderHookResponse>, CancelOrderHook>();

            services.AddScoped<IHook<CreateAutoshipHookRequest, CreateAutoshipHookResponse>, CreateAutoshipHook>();

            services.AddSingleton<IApiEndpoint, CreateTableEndpoint>();
            services.AddSingleton<IApiEndpoint, PopulateTableEndpoint>();
            services.AddSingleton<IApiEndpoint, DeleteTableEndpoint>();
            services.AddSingleton<IApiEndpoint, QueryTableEndpoint>();
        }
    }
}
