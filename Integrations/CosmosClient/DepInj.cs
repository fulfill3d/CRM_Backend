using CRM.Common.DI;
using CRM.Integrations.CosmosClient.Interfaces;
using CRM.Integrations.CosmosClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Integrations.CosmosClient
{
    public static class DepInj
    {
        public static void RegisterCosmosClient(
            this IServiceCollection services, Action<CosmosClientOptions> configureCosmosOptions)
        {
            services.ConfigureServiceOptions<CosmosClientOptions>((_, opt) => configureCosmosOptions(opt));
            services.AddSingleton<ICosmosClient, CosmosClient>();
        }
    }
}