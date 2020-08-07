using System;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Associates.Enrollment;
using hughgrace.Services;

namespace hughgrace.Hooks

{
    public class IsEmailAvailableHook : IHook<IsEmailAvailableHookRequest, IsEmailAvailableHookResponse>
    {
        private readonly IExampleService _exampleService;

        public IsEmailAvailableHook(IExampleService exampleService)
        {
            _exampleService = exampleService;
        }

        public IsEmailAvailableHookResponse Invoke(IsEmailAvailableHookRequest request, Func<IsEmailAvailableHookRequest, IsEmailAvailableHookResponse> func)
        {
            if (!_exampleService.IsValidEmail(request.EmailAddress))
            {
                return new IsEmailAvailableHookResponse
                {
                    IsAvailable = false
                };
            }

            return func(request);
        }
    }
}
