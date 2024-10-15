using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using CRM.Functions.Background.Services.Interfaces;

namespace CRM.Functions.Background
{
    public class BackgroundFunction(
        IBackgroundService backgroundService)
    {
        [Function(nameof(ChangeAppointmentStatus))]
        [OpenApiOperation(
            operationId: "ChangeAppointmentStatus Timer Function",
            tags: new[] { "timer" })]
        [FixedDelayRetry(5, "00:00:30")]
        public async Task ChangeAppointmentStatus([TimerTrigger("0 0 0 * * *")] TimerInfo timerInfo,
            FunctionContext context)
        {
            await backgroundService.ExecuteAppointmentTask();
        }
    }
}