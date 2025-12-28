using AspNetCoreExtensions;
using Microsoft.Extensions.Configuration;

namespace Alyx.Discord.Core.Configuration;

public record NetStoneConfiguration(
    int MaxAgeCharacter,
    int MaxAgeClassJobs,
    int MaxAgeMinions,
    int MaxAgeMounts,
    int MaxAgeFreeCompany,
    int ForceRefreshCooldown)
{
    internal static NetStoneConfiguration FromEnvironment(IConfiguration configuration)
    {
        const int day = 60 * 24;
        const int week = day * 7;

        var maxAgeCharacter = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeCharacter)
                              ?? day;
        var maxAgeClassJobs = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeClassJobs)
                              ?? day;
        var maxAgeMinions = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeMinions)
                            ?? day;
        var maxAgeMounts = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeMounts)
                           ?? day;
        var maxAgeFc = configuration.GetOptionalConfiguration<int>(EnvironmentVariables.NetStoneMaxAgeFreeCompany)
                       ?? week;
        var forceRefreshCooldown = configuration.GetOptionalConfiguration<int>(
            EnvironmentVariables.ForceRefreshCooldown) ?? day;

        return new NetStoneConfiguration(maxAgeCharacter, maxAgeClassJobs, maxAgeMinions, maxAgeMounts, maxAgeFc,
            forceRefreshCooldown);
    }
}