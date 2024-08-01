using CRM.Common.DI;
using CRM.Integrations.GoogleMapsClient.Interfaces;
using CRM.Integrations.GoogleMapsClient.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Integrations.GoogleMapsClient
{
    public static class DepInj
    {
        public static void RegisterGoogleMapsClient(
            this IServiceCollection services, Action<GoogleMapsOptions> config)
        {
            services.ConfigureServiceOptions<GoogleMapsOptions>((_, opt) => config(opt));
            services.AddTransient<IGoogleMapsClient, GoogleMapsClient>();
        }
    }
}