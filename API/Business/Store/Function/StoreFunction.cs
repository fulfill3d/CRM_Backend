using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CRM.API.Business.Store.Data.Models;
using CRM.API.Business.Store.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.API.Business.Store
{
    public class StoreFunction(
        IJwtValidatorService jwtValidatorService,
        IStoreService managementService,
        IOptions<AuthorizationScope> opt,
        IHttpRequestBodyMapper<StoreRequest> storeRequestBodyMapper)
    {
        private readonly AuthorizationScope _managementScope = opt.Value;
        
        [Function(nameof(GetStores))]
        [OpenApiOperation(operationId: "GetStores",
            tags: new[] { "GetStores" },
            Description = "Get all stores of the business")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<StoreViewModel>),
            Description = "List<StoreViewModel> response")]
        public async Task<HttpResponseData> GetStores(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get-all")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var stores = await managementService.GetStores(businessRefId);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(stores));

            return response;
        }
        
        [Function(nameof(GetStore))]
        [OpenApiOperation(operationId: "GetStore",
            tags: new[] { "GetStore" },
            Description = "Get a store of the business")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(StoreViewModel),
            Description = "StoreViewModel response")]
        public async Task<HttpResponseData> GetStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get/{storeId}")]
            HttpRequestData req,
            ILogger log,
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var store = await managementService.GetStore(businessRefId, storeId);
            
            if (store == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(store));

            return response;
        }
        
        [Function(nameof(AddStore))]
        [OpenApiOperation(operationId: "AddStore", 
            tags: new[] { "AddStore" },
            Description = "Add a store to the business")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreRequest), 
            Description = "Store request body", 
            Required = true)]
        public async Task<HttpResponseData> AddStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "add")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var body = await storeRequestBodyMapper.MapAndValidate(req.Body);
            
            var success = await managementService.AddStore(businessRefId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(EditStore))]
        [OpenApiOperation(operationId: "EditStore", 
            tags: new[] { "EditStore" },
            Description = "Edit a store of the business")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreRequest), 
            Description = "Store request body", 
            Required = true)]
        public async Task<HttpResponseData> EditStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "edit")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var body = await storeRequestBodyMapper.MapAndValidate(req.Body);
            
            var success = await managementService.EditStore(businessRefId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(DeleteStore))]
        [OpenApiOperation(operationId: "DeleteStore",
            tags: new[] { "DeleteStore" },
            Description = "Delete a store of the business")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        public async Task<HttpResponseData> DeleteStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{storeId}")]
            HttpRequestData req,
            ILogger log,
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var success = await managementService.DeleteStore(businessRefId, storeId);

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