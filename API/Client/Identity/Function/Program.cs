using Azure.Identity;
using CRM.Common.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CRM.API.Client.Identity;

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
        },
        (identityConfig) =>
        {
            identityConfig.PostRegisterRedirectUri = configuration["CRM_Client_PostRegister_RedirectUri"] ?? string.Empty;
            identityConfig.PostUpdateRedirectUri = configuration["CRM_Client_PostUpdate_RedirectUri"] ?? string.Empty;
        },
        (tokenConfig) =>
        {
            tokenConfig.TokenEndpoint = configuration["CRM_Client_B2C_TokenEndpoint"] ?? string.Empty;
            tokenConfig.SignInUpPolicy = configuration["CRM_Client_B2C_SignInUpPolicy"] ?? string.Empty;
            tokenConfig.UpdatePolicy = configuration["CRM_Client_B2C_UpdatePolicy"] ?? string.Empty;
            tokenConfig.ClientId = configuration["CRM_Client_B2C_ClientId"] ?? string.Empty;
            tokenConfig.ClientSecret = configuration["CRM_Client_B2C_ClientSecret"] ?? string.Empty;
            tokenConfig.Scope = configuration["B2C_Scope"] ?? string.Empty;
            tokenConfig.GrantType = configuration["B2C_GrantType"] ?? string.Empty;
            tokenConfig.B2CPostRegisterRedirectUri = configuration["CRM_Client_B2C_SignIn_RedirectUri"] ?? string.Empty;
            tokenConfig.B2CPostUpdateRedirectUri = configuration["CRM_Client_B2C_ProfileEdit_RedirectUri"] ?? string.Empty;
        });
    })
    .Build();

host.Run();