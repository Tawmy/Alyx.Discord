using Microsoft.Extensions.Configuration;

namespace Alyx.Discord.Core.Configuration;

public record AlyxConfiguration(NetStoneConfiguration NetStone)
{
    internal static AlyxConfiguration FromEnvironment(IConfiguration configuration)
    {
        var netStoneConfiguration = NetStoneConfiguration.FromEnvironment(configuration);
        return new AlyxConfiguration(netStoneConfiguration);
    }
}