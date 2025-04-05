using System.Diagnostics;
using DSharpPlus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Alyx.Discord.Bot.HealthChecks;

public class DiscordHealthCheck(DiscordClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await client.GetUserAsync(111462437164724224, true);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds > 1000 ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}