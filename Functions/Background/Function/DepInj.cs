using CRM.Common.Database;
using CRM.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using CRM.Functions.Background.Data.Database;
using CRM.Functions.Background.Services;
using CRM.Functions.Background.Services.Interfaces;

namespace CRM.Functions.Background
{
    public static class DepInj
    {
        public static void RegisterServices(
            this IServiceCollection services,
            DatabaseOption dbOption)
        {
            #region Miscellaneous

            services.AddDatabaseContext<BackgroundContext>(dbOption);

            #endregion

            #region Services

            services.AddTransient<IBackgroundService, BackgroundService>();

            #endregion
        }
    }
}