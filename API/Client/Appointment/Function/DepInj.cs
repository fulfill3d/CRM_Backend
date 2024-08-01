using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Appointment.Data.Database;
using CRM.API.Client.Appointment.Data.Models.Request;
using CRM.API.Client.Appointment.Services;
using CRM.API.Client.Appointment.Services.Interfaces;
using CRM.Common.Database;
using CRM.Common.DI;

namespace Appointment
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption)
        {
            #region Miscellaneous

            services.AddDatabaseContext<AppointmentContext>(dbOption);
            services.AddHttpRequestBodyMapper();
            services.AddFluentValidator<NewAppointmentRequest>();

            #endregion

            #region Services

            services.AddTransient<IAppointmentService, AppointmentService>();

            #endregion
        }
    }
}