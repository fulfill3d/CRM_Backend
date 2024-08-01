using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Appointment.Data.Models;
using Appointment.Services.Interfaces;
using CRM.Common.Services.Interfaces;

namespace Appointment
{
    public class AppointmentFunction(
        ILogger<AppointmentFunction> logger,
        IHttpRequestBodyMapper<Request> requestBodyMapper,
        IAppointmentService appointmentService)
    {
        [Function(nameof(HttpGet))]
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
        public async Task<HttpResponseData> HttpGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "route/{integerId}")]
            HttpRequestData req,
            FunctionContext executionContext, int integerId)
        {
            var response = req.CreateResponse();
            logger.LogInformation("Parameter is {value}", integerId);
            response.StatusCode = HttpStatusCode.OK;
            await appointmentService.AppointmentServiceMethod();
            await response.WriteStringAsync($"Received ID: {integerId}");
            return response;
        }

        [Function(nameof(HttpPost))]
        [OpenApiOperation(
            operationId: "HttpPost",
            tags: new[] { "post" })]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(Request),
            Description = "The request payload")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(Request),
            Description = "The OK response")]
        public async Task<HttpResponseData> HttpPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            var request = await requestBodyMapper.MapAndValidate(req.Body);
            await appointmentService.AppointmentServiceMethod();
            var response = req.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync(JsonConvert.SerializeObject(request));
            return response;
        }

        [Function(nameof(ServiceBus))]
        [OpenApiOperation(
            operationId: "ServiceBus",
            tags: new[] { "servicebus" })]
        public async Task ServiceBus(
            [ServiceBusTrigger("appointment-queue", Connection = "ServiceBusConnectionString",
                IsSessionsEnabled = true)]
            string message,
            FunctionContext context)
        {
            logger.LogInformation($"Message received: {message}");
            await appointmentService.AppointmentServiceMethod();
        }

        [Function(nameof(TimerFunction))]
        [OpenApiOperation(
            operationId: "TimerFunction",
            tags: new[] { "timer" })]
        [FixedDelayRetry(5, "00:00:10")]
        public async Task TimerFunction([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo,
            FunctionContext context)
        {
            logger.LogInformation("TimerTrigger function triggered");
            await appointmentService.AppointmentServiceMethod();
        }

        [Function(nameof(BlobFunction))]
        [OpenApiOperation(
            operationId: "BlobFunction",
            tags: new[] { "blob" })]
        public async Task BlobFunction(
            [BlobTrigger("appointment-container/{name}")]
            Stream myTriggerItem,
            string name,
            FunctionContext context)
        {
            logger.LogInformation("Timer function triggered");
            await appointmentService.AppointmentServiceMethod();
        }
    }
}