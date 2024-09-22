using Microsoft.Extensions.Configuration;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Core.Configuration;

public record NetStoneConfiguration(
    int MaxAgeCharacter,
    int MaxAgeClassJobs,
    int MaxAgeMinions,
    int MaxAgeMounts,
    int MaxAgeFreeCompany)
{
    internal static NetStoneConfiguration FromEnvironment(IConfiguration configuration)
    {
        const int hour = 60;

        var maxAgeCharacter = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeCharacter)
                              ?? hour;
        var maxAgeClassJobs = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeClassJobs)
                              ?? hour * 2;
        var maxAgeMinions = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeMinions)
                            ?? hour * 4;
        var maxAgeMounts = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeMounts)
                           ?? hour * 4;
        var maxAgeFc = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeFreeCompany)
                       ?? hour * 4;

        return new NetStoneConfiguration(maxAgeCharacter, maxAgeClassJobs, maxAgeMinions, maxAgeMounts, maxAgeFc);
    }
}