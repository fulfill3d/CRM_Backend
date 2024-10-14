using Microsoft.Extensions.DependencyInjection;
using CRM.API.Business.Service.Data.Database;
using CRM.API.Business.Service.Data.Models;
using CRM.API.Business.Service.Services;
using CRM.API.Business.Service.Services.Interfaces;
using CRM.Common.Database;
using CRM.Common.Services;
using CRM.Common.Services.Options;

namespace CRM.API.Business.Service
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
            services.AddDatabaseContext<ServiceContext>(dbOption);
            services.AddB2CJwtTokenValidator((_, opt) => tokenValidation(opt));
            
            #endregion
            
            #region Services
            
            services.AddTransient<IServiceService, ServiceService>();
            
            #endregion
            
            #region MapperValidator
            
            services.AddFluentValidator<StoreServiceRequest>();
            services.AddHttpRequestBodyMapper();
            
            #endregion
        }
    }
}