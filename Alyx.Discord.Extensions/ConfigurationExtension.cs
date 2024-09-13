using Microsoft.Extensions.Configuration;

namespace Alyx.Discord.Extensions;

public static class ConfigurationExtension
{
    public static string GetGuardedConfiguration(this IConfiguration configuration, string key)
    {
        if (configuration[key] is not { } value)
        {
            throw new ArgumentNullException(nameof(key), $"{key} not set.");
        }

        return value;
    }
}