using System.Diagnostics;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Alyx.Discord.Bot.HealthChecks;

public class DiscordHealthCheck(DiscordClient client, IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var timeoutStr = configuration[EnvironmentVariables.DiscordHealthCheckTimeout];
        var timeout = timeoutStr is not null ? int.Parse(timeoutStr) : 1000;
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await client.GetUserAsync(111462437164724224, true);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds > timeout ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}