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
        },
        (tokenValidation) =>
        {
            tokenValidation.MetadataUrl = configuration["CRM_Client_B2C_MetadataUrl"] ?? string.Empty;
            tokenValidation.Issuer = configuration["CRM_Client_B2C_Issuer"] ?? string.Empty;
            tokenValidation.ClientId = configuration["CRM_Client_ClientBackend_ClientId"] ?? string.Empty;
        },
        (tokenOptions) =>
        {
            tokenOptions.Read = configuration["CRM_AuthScope_Client_Service_Read"] ?? string.Empty;
            tokenOptions.Write = configuration["CRM_AuthScope_Client_Service_Write"] ?? string.Empty;
        });
    })
    .Build();

host.Run();