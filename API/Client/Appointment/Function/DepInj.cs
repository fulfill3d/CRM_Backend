using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Appointment.Data.Database;
using CRM.API.Client.Appointment.Data.Models;
using CRM.API.Client.Appointment.Services;
using CRM.API.Client.Appointment.Services.Interfaces;
using CRM.API.Client.Appointment.Services.Options;
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