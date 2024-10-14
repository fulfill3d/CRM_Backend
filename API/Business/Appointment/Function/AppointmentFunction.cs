using System.Net;
using CRM.API.Business.Appointment.Data.Models;
using CRM.API.Business.Appointment.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CRM.API.Business.Appointment
{
    public class AppointmentFunction(
        IJwtValidatorService jwtValidatorService,
        IOptions<AuthorizationScope> opt,
        IAppointmentService appointmentService)
    {
        private readonly AuthorizationScope _appointmentScope = opt.Value;
        
        [Function(nameof(GetAppointments))]
        [OpenApiOperation(operationId: "GetAppointments",
            tags: new[] { "GetAppointments" },
            Description = "Get all appointments of the store")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<AppointmentViewModel>),
            Description = "List<AppointmentViewModel> response")]
        public async Task<HttpResponseData> GetAppointments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "get/{storeId}")]
            HttpRequestData req,
            ILogger log, int storeId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _appointmentScope.Read };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var appointments = await appointmentService.GetAll(businessRefId, storeId);

            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(appointments));

            return response;
        }
        
        [Function(nameof(CancelAppointment))]
        [OpenApiOperation(operationId: "CancelAppointment",
            tags: new[] { "CancelAppointment" },
            Description = "Cancel an appointment of the store")]
        [OpenApiParameter(name: "appointmentId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Appointment ID")]
        public async Task<HttpResponseData> CancelAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "cancel/{appointmentId}")]
            HttpRequestData req,
            ILogger log,
            int appointmentId)
        {
            var response = req.CreateResponse();
            var acceptedScopes = new[] { _appointmentScope.Write };
            var businessRefId = await jwtValidatorService.AuthenticateAndAuthorize(req, acceptedScopes);
            
            if (businessRefId == null)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
                return response;
            }
            
            var success = await appointmentService.Cancel(businessRefId, appointmentId);

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