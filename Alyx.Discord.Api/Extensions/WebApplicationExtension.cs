using Alyx.Discord.Db;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Alyx.Discord.Api.Extensions;

internal static class WebApplicationExtension
{
    public static void UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health").RequireAuthorization();

        app.MapHealthChecks("/health/db", new HealthCheckOptions
                { Predicate = check => check.Name.Equals(nameof(DatabaseContext), StringComparison.OrdinalIgnoreCase) })
            .RequireAuthorization();

        app.MapHealthChecks("/health/bot", new HealthCheckOptions
                { Predicate = check => check.Name.Equals("bot", StringComparison.OrdinalIgnoreCase) })
            .RequireAuthorization();

        app.MapHealthChecks("/health/netstone", new HealthCheckOptions
                { Predicate = check => check.Name.Equals("netstone", StringComparison.OrdinalIgnoreCase) })
            .RequireAuthorization();
    }
}