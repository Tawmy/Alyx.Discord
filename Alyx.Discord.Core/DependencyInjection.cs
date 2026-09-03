using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Services;
using AspNetCoreExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Api.Sdk;
using NetStone.Api.Sdk.DependencyInjection;

namespace Alyx.Discord.Core;

public static class DependencyInjection
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(AlyxConfiguration.FromEnvironment(configuration));

        services.AddSingleton<ExternalResourceService>();
        services.AddHttpClient<CharacterSheetService>().AddStandardResilienceHandler();
        services.AddNetStoneApi(configuration);

        services.AddHostedService<PostStartupService>();
    }

    private static void AddNetStoneApi(this IServiceCollection services, IConfiguration configuration)
    {
        var apiRootUri = new Uri(configuration.GetRequiredConfiguration(EnvironmentVariables.NetStoneApiRootUri));
        var authAuthority = new Uri(configuration.GetRequiredConfiguration(EnvironmentVariables.NetStoneApiAuthority));
        var authClientId = configuration.GetRequiredConfiguration(EnvironmentVariables.NetStoneApiClientId);
        var authScopes = configuration.GetRequiredConfiguration(EnvironmentVariables.NetStoneApiScopes);
        var authScopesArray = authScopes.Split(" ",
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var certDir = configuration.GetRequiredConfiguration(EnvironmentVariables.NetStoneApiClientSignedJwtCert);
        var certPath = $"{certDir}.pem";
        var keyPath = $"{certDir}.key";

        var options = new NetStoneApiOptions
        {
            ApiBaseAddress = apiRootUri,
            AuthAuthority = authAuthority,
            AuthClientId = authClientId,
            AuthCertificatePath = certPath,
            AuthPrivateKeyPath = keyPath,
            AuthScopes = authScopesArray
        };

        services.AddNetStoneApi(options);
    }
}