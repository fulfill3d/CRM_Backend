using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Appointment.Data.Database;
using CRM.API.Client.Appointment.Data.Models.Request;
using CRM.API.Client.Appointment.Services;
using CRM.API.Client.Appointment.Services.Interfaces;
using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Options;

namespace Appointment
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption,
            Action<TokenValidationOptions> tokenValidation,
            Action<AuthorizationScope> authorizationScope)
        {
            #region Miscellaneous

            services.AddDatabaseContext<AppointmentContext>(dbOption);
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidation(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddHttpRequestBodyMapper();
            services.AddFluentValidator<NewAppointmentRequest>();
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));

            #endregion

            #region Services

            services.AddTransient<IAppointmentService, AppointmentService>();
            services.AddCommonClientService(dbOption);

            #endregion
        }
    }
}