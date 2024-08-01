using Microsoft.Extensions.DependencyInjection;
using Appointment.Data.Database;
using Appointment.Data.Models;
using Appointment.Services;
using Appointment.Services.Interfaces;
using Appointment.Services.Options;
using CRM.Common.Database;
using CRM.Common.DI;

namespace Appointment
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption,
            Action<AppointmentOptions> appointmentOptions)
        {
            #region Miscellaneous

            services.ConfigureServiceOptions<AppointmentOptions>((_, opt) => appointmentOptions(opt));
            services.AddDatabaseContext<AppointmentContext>(dbOption);
            services.AddHttpRequestBodyMapper();
            services.AddFluentValidator<Request>();

            #endregion

            #region Services

            services.AddTransient<IAppointmentService, AppointmentService>();

            #endregion
        }
    }
}