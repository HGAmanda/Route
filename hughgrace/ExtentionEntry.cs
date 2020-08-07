using Microsoft.Extensions.DependencyInjection;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks.Associates.Enrollment;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Api;
using hughgrace.Hooks;
using hughgrace.Api;
using hughgrace.Services;

namespace hughgrace
{
    public class ExtensionEntry : IExtension
    {
        public void ConfigureServices(IServiceCollection services)
        {

            //NOTE: These are examples of how to implement a custom Api Endpoint and a custom hook.
            //services.AddSingleton<IExampleService, ExampleService>();
            //services.AddSingleton<IApiEndpoint, ApiExample>();
            //services.AddSingleton<IHook<IsEmailAvailableHookRequest, IsEmailAvailableHookResponse>, IsEmailAvailableHook>();
        }
    }
}
