using System.Net;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using CRM.API.Business.Management.Data.Models.Request;
using CRM.API.Business.Management.Data.Models.Response;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CRM.API.Business.Management.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace CRM.API.Business.Management
{
    public class ManagementFunction(
        IJwtValidatorService jwtValidatorService,
        IManagementService managementService,
        IOptions<AuthorizationScope> opt,
        IHttpRequestBodyMapper<StoreRequest> storeRequestBodyMapper,
        IHttpRequestBodyMapper<EmployeeRequest> employeeRequestBodyMapper,
        IHttpRequestBodyMapper<StoreServiceRequest> storeServiceRequestBodyMapper)
    {
        private readonly AuthorizationScope _managementScope = opt.Value;
        
        // STORE
        
        [Function(nameof(GetStores))]
        [OpenApiOperation(operationId: "GetStores", Description = "Get all stores of the business")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<StoreViewModel>),
            Description = "List<StoreViewModel> response")]
        public async Task<HttpResponseData> GetStores(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "store/get-all")]
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
        [OpenApiOperation(operationId: "GetStore", Description = "Get a store of the business")]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "store/get/{storeId}")]
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
        [OpenApiOperation(operationId: "AddStore", Description = "Add a store to the business")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreRequest), 
            Description = "Store request body", 
            Required = true)]
        public async Task<HttpResponseData> AddStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "store/add")]
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
        [OpenApiOperation(operationId: "EditStore", Description = "Edit a store of the business")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreRequest), 
            Description = "Store request body", 
            Required = true)]
        public async Task<HttpResponseData> EditStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "store/edit")]
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
        [OpenApiOperation(operationId: "DeleteStore", Description = "Delete a store of the business")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        public async Task<HttpResponseData> DeleteStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "store/delete/{storeId}")]
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
        
        // EMPLOYEE
        
        [Function(nameof(GetEmployees))]
        [OpenApiOperation(operationId: "GetEmployees", Description = "Get employees of a store")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<StoreEmployeeViewModel>),
            Description = "List<StoreEmployeeViewModel> response")]
        public async Task<HttpResponseData> GetEmployees(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employee/get-all")]
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
            
            var employees = await managementService.GetEmployees(businessRefId);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(employees));

            return response;
        }
        
        [Function(nameof(AddEmployee))]
        [OpenApiOperation(operationId: "AddEmployee", Description = "Add employee to a store")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(EmployeeRequest), 
            Description = "Employee request body, Employee.Id is NOT required for new entity", 
            Required = true)]
        public async Task<HttpResponseData> AddEmployee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employee/add/{storeId}")]
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
            
            var body = await employeeRequestBodyMapper.MapAndValidate(req.Body);
            
            var success = await managementService.AddEmployee(businessRefId, storeId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(EditEmployees))]
        [OpenApiOperation(operationId: "EditEmployees", Description = "Edit employee of a store")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(EmployeeRequest), 
            Description = "Employee request body, Employee.Id is required for editing the existing entity", 
            Required = true)]
        public async Task<HttpResponseData> EditEmployees(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "employee/edit")]
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
            
            var body = await employeeRequestBodyMapper.MapAndValidate(req.Body);
            
            var success = await managementService.EditEmployee(businessRefId, body);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        [Function(nameof(DeleteEmployee))]
        [OpenApiOperation(operationId: "DeleteEmployee", Description = "Delete employee of a store")]
        [OpenApiParameter(name: "employeeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Employee ID")]
        public async Task<HttpResponseData> DeleteEmployee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "employee/delete/{employeeId}")]
            HttpRequestData req,
            ILogger log,
            int employeeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var success = await managementService.DeleteEmployee(businessRefId, employeeId);

            if (!success)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
        
        // SERVICES
        
        [Function(nameof(GetServiceCategories))]
        [OpenApiOperation(operationId: "GetServiceCategories", Description = "Get all system defined service categories/sub categories")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(CategoryViewModel),
            Description = "CategoryViewModel response")]
        public async Task<HttpResponseData> GetServiceCategories(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service/category/get")]
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

            var categories = await managementService.GetServiceCategories();
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(categories));

            return response;
        }
        
        [Function(nameof(GetServices))]
        [OpenApiOperation(operationId: "GetServices", Description = "Get services provided by a store")]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service/{storeId}/get")]
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

            var services = await managementService.GetStoreServices(businessRefId, storeId);
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(services));

            return response;
        }
        
        [Function(nameof(AddServices))]
        [OpenApiOperation(operationId: "AddServices", Description = "Add service to a store")]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "service/{storeId}/add")]
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
        [OpenApiOperation(operationId: "EditService", Description = "Edit service of a store")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(StoreServiceRequest), 
            Description = "Store Service request body, StoreServiceRequest.store-service-id is required for editing an existing entity", 
            Required = true)]
        public async Task<HttpResponseData> EditService(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "service/edit")]
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
        [OpenApiOperation(operationId: "DeleteService", Description = "Delete service of a store")]
        [OpenApiParameter(name: "serviceId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Service ID")]
        public async Task<HttpResponseData> DeleteService(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "service/{serviceId}/delete")]
            HttpRequestData req,
            ILogger log,
            int serviceId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _managementScope.Write };
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