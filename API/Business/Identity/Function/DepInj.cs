using CRM.Common.Database;
using CRM.Common.Services;
using CRM.API.Business.Identity.Data.Database;
using CRM.API.Business.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using CRM.API.Business.Identity.Services.Interfaces;
using CRM.API.Business.Identity.Services.Options;

namespace CRM.API.Business.Identity
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption,
            Action<IdentityOptions> identityOptions,
            Action<TokenServiceOption> configureToken)
        {
            #region Miscellaneous

            services.ConfigureServiceOptions<IdentityOptions>((_, opt) => identityOptions(opt));
            services.ConfigureServiceOptions<TokenServiceOption>((_, opt) => configureToken(opt));
            services.AddDatabaseContext<IdentityContext>(dbOption);

            #endregion

            #region Services

            services.AddHttpClient();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITokenService, TokenService>();

            #endregion
        }
    }
}