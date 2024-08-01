using Azure.Identity;
using CRM.Common.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CRM.API.Client.Service;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        var configuration = builder.Build();
        var token = new DefaultAzureCredential();
        var appConfigUrl = configuration["AppConfigUrl"] ?? string.Empty;

        builder.AddAzureAppConfiguration(config =>
        {
            config.Connect(new Uri(appConfigUrl), token);
            config.ConfigureKeyVault(kv => kv.SetCredential(token));
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.RegisterServices(new DatabaseOption
        {
            ConnectionString = configuration["CRM_ConnectionString_Db"] ?? string.Empty,
        }, configureGoogle =>
        {
            configureGoogle.ApiKey = configuration["ApiKey_GoogleMaps"] ?? string.Empty;
        });
    })
    .Build();

host.Run();