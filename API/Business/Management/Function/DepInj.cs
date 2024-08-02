using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Options;
using CRM.API.Business.Management.Data.Database;
using CRM.API.Business.Management.Data.Models.Request;
using CRM.API.Business.Management.Services;
using Microsoft.Extensions.DependencyInjection;
using CRM.API.Business.Management.Services.Interfaces;
using CRM.Integrations.GoogleMapsClient;
using CRM.Integrations.GoogleMapsClient.Options;

namespace CRM.API.Business.Management
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption,
            Action<TokenValidationOptions> tokenValidation,
            Action<AuthorizationScope> authorizationScope,
            Action<GoogleMapsOptions> configureGoogleMaps)
        {
            #region Miscellaneous
            
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidation(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddDatabaseContext<ManagementContext>(dbOption);
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));
            
            #endregion
            
            #region Services
            
            services.AddTransient<IManagementService, ManagementService>();
            services.RegisterGoogleMapsClient(configureGoogleMaps);
            
            #endregion
            
            #region MapperValidator
            
            services.AddFluentValidator<StoreRequest>();
            services.AddFluentValidator<AddressRequest>();
            services.AddFluentValidator<EmployeeRequest>();
            services.AddHttpRequestBodyMapper();
            
            #endregion
        }
    }
}