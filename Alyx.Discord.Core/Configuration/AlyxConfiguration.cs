using Microsoft.Extensions.Configuration;

namespace Alyx.Discord.Core.Configuration;

public record AlyxConfiguration(NetStoneConfiguration NetStone, string? StatusMessage)
{
    internal static AlyxConfiguration FromEnvironment(IConfiguration configuration)
    {
        var netStoneConfiguration = NetStoneConfiguration.FromEnvironment(configuration);
        var statusMessage = configuration[EnvironmentVariables.StatusMessage];
        return new AlyxConfiguration(netStoneConfiguration, statusMessage);
    }
}