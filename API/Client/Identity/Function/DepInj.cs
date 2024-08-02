using Microsoft.Extensions.DependencyInjection;
using CRM.API.Client.Identity.Data.Database;
using CRM.API.Client.Identity.Services;
using CRM.API.Client.Identity.Services.Interfaces;
using CRM.API.Client.Identity.Services.Options;
using CRM.Common.Database;
using CRM.Common.Services;

namespace CRM.API.Client.Identity
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