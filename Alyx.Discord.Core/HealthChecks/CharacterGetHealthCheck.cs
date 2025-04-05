using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NetStone.Api.Client;

namespace Alyx.Discord.Core.HealthChecks;

public class CharacterGetHealthCheck(NetStoneApiClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await client.Character.GetAsync("28812634", cancellationToken: cancellationToken);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds > 1000 ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}