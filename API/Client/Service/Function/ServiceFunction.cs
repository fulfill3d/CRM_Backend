using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using CRM.API.Client.Service.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace CRM.API.Client.Service
{
    public class ServiceFunction(
        ILogger<ServiceFunction> logger,
        IServiceService serviceService)
    {

        [Function(nameof(Services))]
        [OpenApiOperation(
            operationId: "Services",
            tags: new[] { "get" },
            Description="The Services within given range and filter query parameters")]
        public async Task<HttpResponseData> Services(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "services/get-all")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();
            var queryParameters = QueryHelpers.ParseQuery(req.Url.Query);
            var services = await serviceService.GetServices(queryParameters);
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(services));
            return response;
        }
        
        [Function(nameof(Service))]
        [OpenApiOperation(
            operationId: "HttpGet",
            tags: new[] { "get" })]
        [OpenApiParameter(
            name: "integerId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The integer ID parameter")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(string),
            Description = "The OK response")]
        public async Task<HttpResponseData> Service(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "services/get/{serviceId}")]
            HttpRequestData req,
            FunctionContext executionContext, 
            int serviceId)
        {
            var response = req.CreateResponse();
            var service = await serviceService.GetService(serviceId);
            if (service == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(service, Formatting.Indented));
            return response;
        }
    }
}