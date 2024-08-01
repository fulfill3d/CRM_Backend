using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Appointment.Services.Interfaces;
using Appointment.Services.Options;

namespace Appointment.Services
{
    public class AppointmentService(ILogger<AppointmentService> logger, IOptions<AppointmentOptions> opt)
        : IAppointmentService
    {
        private readonly AppointmentOptions _options = opt.Value;

        public async Task AppointmentServiceMethod()
        {
            logger.LogInformation("Option1 value: {Option1}", _options.Option1);
            logger.LogInformation("Option1 value: {Option2}", _options.Option2);
            await Task.Delay(100);
        }
    }
}