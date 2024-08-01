using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using CRM.API.Client.Appointment.Data.Models.Request;
using CRM.API.Client.Appointment.Data.Models.Response;
using CRM.API.Client.Appointment.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Appointment
{
    public class AppointmentFunction(
        IHttpRequestBodyMapper<NewAppointmentRequest> newAppointmentRequestBodyMapper,
        IHttpRequestBodyMapper<UpdateAppointmentRequest> updateAppointmentRequestBodyMapper,
        IAppointmentService appointmentService)
    {
        [Function(nameof(GetAppointments))]
        [OpenApiOperation(
            operationId: "GetAppointments",
            tags: new[] { "get" }, 
            Description = "Returns client appointments")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(List<AppointmentViewModel>),
            Description = "The OK response")]
        public async Task<HttpResponseData> GetAppointments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "appointment/get-all")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();
            var appointments = await appointmentService.GetAppointments(5);
            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync(JsonConvert.SerializeObject(appointments, Formatting.Indented));
            return response;
        }

        [Function(nameof(SetAppointment))]
        [OpenApiOperation(
            operationId: "SetAppointment",
            tags: new[] { "post" }, 
            Description = "Set the appointment")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(NewAppointmentRequest),
            Description = "The request payload")]
        public async Task<HttpResponseData> SetAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "appointment/set")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();
            var request = await newAppointmentRequestBodyMapper.MapAndValidate(req.Body);
            var success = await appointmentService.SetNewAppointment(request, 5);
            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [Function(nameof(UpdateAppointment))]
        [OpenApiOperation(
            operationId: "UpdateAppointment",
            tags: new[] { "patch" }, 
            Description = "Update the appointment")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(UpdateAppointmentRequest),
            Description = "The request payload")]
        public async Task<HttpResponseData> UpdateAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "appointment/update")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();
            var request = await updateAppointmentRequestBodyMapper.MapAndValidate(req.Body);
            var success = await appointmentService.UpdateAppointment(request, 5);
            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        [Function(nameof(CancelAppointment))]
        [OpenApiOperation(
            operationId: "CancelAppointment",
            tags: new[] { "delete" }, 
            Description = "Cancel the appointment")]
        [OpenApiParameter(name: "appointmentId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "Appointment ID")]
        public async Task<HttpResponseData> CancelAppointment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "appointment/cancel/{appointmentId}")]
            HttpRequestData req,
            FunctionContext executionContext,
            int appointmentId)
        {
            var response = req.CreateResponse();
            var success = await appointmentService.CancelAppointment(appointmentId, 5);
            if (!success)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
}