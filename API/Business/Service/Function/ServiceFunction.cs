using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CRM.API.Business.Service.Data.Models;
using CRM.API.Business.Service.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using Microsoft.Extensions.Options;

namespace CRM.API.Business.Service
{
    public class ServiceFunction(
        IJwtValidatorService jwtValidatorService,
        IServiceService managementService,
        IOptions<AuthorizationScope> opt,
        IHttpRequestBodyMapper<StoreServiceRequest> storeServiceRequestBodyMapper)
    {
        private readonly AuthorizationScope _serviceScope = opt.Value;
        
        [Function(nameof(GetServiceCategories))]
        [OpenApiOperation(operationId: "GetServiceCategories", 
            tags: new[] { "GetServiceCategories" },
            Description = "Get all system defined service categories/sub categories")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(CategoryViewModel),
            Description = "CategoryViewModel response")]
        public async Task<HttpResponseData> GetServiceCategories(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "category")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _serviceScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            var categories = await managementService.GetServiceCategories();
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(categories));

            return response;
        }
        
        [Function(nameof(GetServices))]
        [OpenApiOperation(operationId: "GetServices", 
            tags: new[] { "GetServices" },
            Description = "Get services provided by a store")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<ServiceViewModel>),
            Description = "List<ServiceViewModel> response")]
        public async Task<HttpResponseData> GetServices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get/{storeId}")]
            HttpRequestData req,
            ILogger log, 
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _serviceScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            var services = await managementService.GetStoreServices(businessRefId, storeId);
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(services));

            return response;
        }
        
        [Function(nameof(AddServices))]
        [OpenApiOperation(operationId: "AddServices", 
            tags: new[] { "AddServices" },
            Description = "Add service to a store")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreServiceRequest), 
            Description = "Store Service request body, StoreServiceRequest.store-service-id is NOT required for adding an entity", 
            Required = true)]
        public async Task<HttpResponseData> AddServices(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add/{storeId}")]
            HttpRequestData req,
            ILogger log, 
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _serviceScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var body = await storeServiceRequestBodyMapper.MapAndValidate(req.Body);

            var success = await managementService.AddStoreService(businessRefId, storeId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(EditService))]
        [OpenApiOperation(operationId: "EditService", 
            tags: new[] { "EditService" },
            Description = "Edit service of a store")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreServiceRequest), 
            Description = "Store Service request body, StoreServiceRequest.store-service-id is required for editing an existing entity", 
            Required = true)]
        public async Task<HttpResponseData> EditService(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "edit")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _serviceScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var body = await storeServiceRequestBodyMapper.MapAndValidate(req.Body);

            var success = await managementService.EditStoreService(businessRefId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(DeleteService))]
        [OpenApiOperation(operationId: "DeleteService", 
            tags: new[] { "DeleteService" },
            Description = "Delete service of a store")]
        [OpenApiParameter(name: "serviceId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Service ID")]
        public async Task<HttpResponseData> DeleteService(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{serviceId}")]
            HttpRequestData req,
            ILogger log,
            int serviceId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _serviceScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }

            var success = await managementService.DeleteStoreService(businessRefId, serviceId);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}