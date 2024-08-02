using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using CRM.API.Business.Identity.Services.Interfaces;
using CRM.API.Business.Identity.Services.Options;
using Microsoft.Extensions.Options;

namespace CRM.API.Business.Identity
{
    public class IdentityFunction(
        IOptions<IdentityOptions> opt,
        IIdentityService identityService)
    {
        private readonly IdentityOptions _identityServiceOption = opt.Value;
        
        [Function(nameof(PostRegister))]
        [OpenApiOperation(
            operationId: "PostRegister",
            tags: new[] { "PostRegister" }, 
            Description = "This is a callback function of Azure Active Directory B2C. " +
                          "B2C callbacks here after successful sign up/sign in with `authorization_code`. " +
                          "PostRegister verifies that `authorization_code`, get an `id_token` and " +
                          "create a Business entity in the database")]
        public async Task<HttpResponseData> PostRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "post-register")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            string code = req.Query["code"] ?? string.Empty;
            var response = req.CreateResponse();
            if (code == string.Empty)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                // TODO Redirect to Unauthorized page
                return response;
            }
            
            var success = await identityService.VerifyAndProcess(code);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                // TODO Redirect to Unauthorized page
                return response;
            }
            
            var urlBuilder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = _identityServiceOption.PostRegisterRedirectUri
            };

            response.StatusCode = HttpStatusCode.Redirect;
            response.Headers.Add("Location", urlBuilder.ToString());
            
            return response;
        }

        [Function(nameof(PostUpdate))]
        [OpenApiOperation(
            operationId: "PostUpdate",
            tags: new[] { "PostUpdate" }, 
            Description = "This is a callback function of Azure Active Directory B2C. " +
                          "B2C callbacks here after successful profile edit in with `authorization_code`. " +
                          "PostUpdate verifies that `authorization_code`, get an `id_token` and " +
                          "update the Business entity in the database")]
        public async Task<HttpResponseData> PostUpdate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "post-update")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            string code = req.Query["code"] ?? string.Empty;
            var response = req.CreateResponse();
            if (code == string.Empty)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                // TODO Redirect to Unauthorized page
                return response;
            }
            
            var success = await identityService.VerifyAndProcess(code, true);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                // TODO Redirect to Unauthorized page
                return response;
            }
            
            var urlBuilder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = _identityServiceOption.PostUpdateRedirectUri
            };

            response.StatusCode = HttpStatusCode.Redirect;
            response.Headers.Add("Location", urlBuilder.ToString());
            
            return response;
        }
    }
}