using CRM.Common.Database;
using CRM.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Service.Data.Database;
using CRM.API.Client.Service.Data.Mappers;
using CRM.API.Client.Service.Data.Models.Request;
using CRM.API.Client.Service.Services;
using CRM.API.Client.Service.Services.Interfaces;
using CRM.Common.Services.Interfaces;
using CRM.Common.Services.Options;
using CRM.Integrations.GoogleMapsClient;
using CRM.Integrations.GoogleMapsClient.Options;
using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption,
            Action<GoogleMapsOptions> configureGoogleOptions,
            Action<TokenValidationOptions> tokenValidation,
            Action<AuthorizationScope> authorizationScope)
        {
            #region Miscellaneous

            services.AddDatabaseContext<ServiceContext>(dbOption);
            
            services.AddTransient<
                IMapper<IDictionary<string, StringValues>, ServiceFilterParameters>, 
                ServiceFilterParametersMapper>();
            
            services.AddTransient<
                IMapper<IDictionary<string, StringValues>, PaginationParameters>, 
                PaginationParametersMapper>();
            
            services.AddFluentValidator<ServiceFilterParameters>();
            services.AddFluentValidator<PaginationParametersMapper>();
            services.ConfigureServiceOptions<TokenValidationOptions>((_, opt) => tokenValidation(opt));
            services.ConfigureServiceOptions<AuthorizationScope>((_, opt) => authorizationScope(opt));
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));

            #endregion

            #region Services

            services.AddTransient<IServiceService, ServiceService>();
            services.RegisterGoogleMapsClient(configureGoogleOptions);

            #endregion
        }
    }
}