using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Api.Sdk;
using NetStone.Api.Sdk.DependencyInjection;
using NetStone.Common.Extensions;

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
        var apiRootUri = new Uri(configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiRootUri));
        var authAuthority = new Uri(configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiAuthority));
        var authClientId = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiClientId);
        var authClientSecret = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiClientSecret);
        var authScopes = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiScopes);
        var authScopesArray =
            authScopes.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var options = new NetStoneApiOptions(apiRootUri, authAuthority, authClientId, authClientSecret,
            authScopesArray);

        services.AddNetStoneApi(options);
    }
}