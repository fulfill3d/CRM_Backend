using CRM.API.Business.Store.Data.Database;
using CRM.API.Business.Store.Data.Models;
using CRM.API.Business.Store.Services;
using CRM.API.Business.Store.Services.Interfaces;
using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Options;
using CRM.Integrations.GoogleMapsClient;
using CRM.Integrations.GoogleMapsClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.API.Business.Store
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
            services.AddDatabaseContext<StoreContext>(dbOption);
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));
            
            #endregion
            
            #region Services
            
            services.AddTransient<IStoreService, StoreService>();
            services.RegisterGoogleMapsClient(configureGoogleMaps);
            
            #endregion
            
            #region MapperValidator
            
            services.AddFluentValidator<StoreRequest>();
            services.AddFluentValidator<AddressRequest>();
            services.AddHttpRequestBodyMapper();
            
            #endregion
        }
    }
}