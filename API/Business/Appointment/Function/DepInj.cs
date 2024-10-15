using Microsoft.Extensions.DependencyInjection;
using CRM.API.Business.Appointment.Data.Database;
using CRM.API.Business.Appointment.Services;
using CRM.API.Business.Appointment.Services.Interfaces;
using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Options;

namespace CRM.API.Business.Appointment
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
            
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidation(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddDatabaseContext<AppointmentContext>(dbOption);
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));

            #endregion

            #region Services

            services.AddTransient<IAppointmentService, AppointmentService>();

            #endregion
        }
    }
}