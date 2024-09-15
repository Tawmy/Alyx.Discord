using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Api.Client;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Core;

public static class DependencyInjection
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNetStoneApiClient(configuration);
    }

    private static void AddNetStoneApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var apiRootUri = configuration.GetGuardedConfiguration<Uri>(EnvironmentVariables.NetStoneApiRootUri);
        var authAuthority = configuration.GetGuardedConfiguration<Uri>(EnvironmentVariables.NetStoneApiAuthority);
        var authClientId = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiClientId);
        var authClientSecret = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiClientSecret);
        var authScopes = configuration.GetGuardedConfiguration(EnvironmentVariables.NetStoneApiScopes);
        var authScopesArray =
            authScopes.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


        var netStoneApiClientConfiguration = new NetStoneApiClientConfiguration(apiRootUri, authAuthority, authClientId,
            authClientSecret, authScopesArray);
        services.AddSingleton(new NetStoneApiClient(netStoneApiClientConfiguration));
    }
}