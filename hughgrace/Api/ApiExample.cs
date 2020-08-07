using DirectScale.Disco.Extension.Api;
using DirectScale.Disco.Extension.Services;

namespace hughgrace.Api
{
    public class ApiExample : IApiEndpoint
    {
        private readonly IAssociateService _associateService;
        private readonly IRequestParsingService _requestParsing;

        public ApiExample(IAssociateService associateService, IRequestParsingService requestParsing)
        {
            _associateService = associateService;
            _requestParsing = requestParsing;
        }

        public ApiDefinition GetDefinition()
        {
            return new ApiDefinition
            {
                Route = "hughgrace/example",
                RequireAuthentication = false
            };
        }

        public IApiResponse Post(ApiRequest request)
        {
            var rObject = _requestParsing.ParseBody<ExampleRequest>(request);
            var aName = _associateService.GetAssociate(rObject.BackOfficeId).Name;

            return new Ok(new { Status = 1, RequestMessage = rObject.Message, AssociateName = aName });
        }
    }

    public class ExampleRequest
    {
        public string Message { get; set; }
        public string BackOfficeId { get; set; }
    }
}
