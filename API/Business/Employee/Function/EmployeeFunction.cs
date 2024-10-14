using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CRM.API.Business.Employee.Data.Models;
using CRM.API.Business.Employee.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using Microsoft.Extensions.Options;

namespace CRM.API.Business.Employee
{
    public class EmployeeFunction(
        IJwtValidatorService jwtValidatorService,
        IEmployeeService managementService,
        IOptions<AuthorizationScope> opt,
        IHttpRequestBodyMapper<EmployeeRequest> employeeRequestBodyMapper)
    {
        private readonly AuthorizationScope _employeeScope = opt.Value;
        
        [Function(nameof(GetEmployees))]
        [OpenApiOperation(operationId: "GetEmployees",
            tags: new[] { "GetEmployees" },
            Description = "Get employees of a store")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<StoreEmployeeViewModel>),
            Description = "List<StoreEmployeeViewModel> response")]
        [OpenApiParameter(name: "storeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Store ID")]
        public async Task<HttpResponseData> GetEmployees(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{storeId}/get-all")]
            HttpRequestData req,
            ILogger log, 
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _employeeScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var employees = await managementService.GetEmployees(businessRefId, storeId);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(employees));

            return response;
        }
        
        [Function(nameof(AddEmployee))]
        [OpenApiOperation(operationId: "AddEmployee", 
            tags: new[] { "AddEmployee" },
            Description = "Add employee to a store")]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{storeId}/add")]
            HttpRequestData req,
            ILogger log,
            int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _employeeScope.Write };
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
        [OpenApiOperation(operationId: "EditEmployees", 
            tags: new[] { "EditEmployees" },
            Description = "Edit employee of a store")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(EmployeeRequest), 
            Description = "Employee request body, Employee.Id is required for editing the existing entity", 
            Required = true)]
        public async Task<HttpResponseData> EditEmployees(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "edit")]
            HttpRequestData req,
            ILogger log)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _employeeScope.Write };
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
        [OpenApiOperation(operationId: "DeleteEmployee", 
            tags: new[] { "DeleteEmployee" },
            Description = "Delete employee of a store")]
        [OpenApiParameter(name: "employeeId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Employee ID")]
        public async Task<HttpResponseData> DeleteEmployee(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{employeeId}")]
            HttpRequestData req,
            ILogger log,
            int employeeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _employeeScope.Write };
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
    }
}