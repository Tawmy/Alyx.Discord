using Microsoft.Extensions.Configuration;

namespace Alyx.Discord.Extensions;

public static class ConfigurationExtension
{
    /// <summary>
    ///     Get configuration as string.
    /// </summary>
    /// <param name="configuration">Instance of application configuration.</param>
    /// <param name="key">Configuration key / environment variable key.</param>
    /// <returns>Configuration value as string.</returns>
    public static string GetGuardedConfiguration(this IConfiguration configuration, string key)
    {
        if (configuration[key] is not { } value)
        {
            throw new ArgumentNullException(nameof(key), $"{key} not set.");
        }

        return value;
    }

    /// <summary>
    ///     Get configuration as specific type.
    /// </summary>
    /// <param name="configuration">Instance of application configuration.</param>
    /// <param name="key">Configuration key / environment variable key.</param>
    /// <typeparam name="T">Type to convert value to.</typeparam>
    /// <returns>Configuration value converted to provided type.</returns>
    /// <exception cref="NotImplementedException">Thrown if conversion of given type T not implemented.</exception>
    public static T GetGuardedConfiguration<T>(this IConfiguration configuration, string key) where T : struct
    {
        if (configuration[key] is not { } value)
        {
            throw new ArgumentNullException(nameof(key), $"{key} not set.");
        }

        return ConvertToType<T>(value);
    }

    /// <summary>
    ///     Get optional configuration value as specific type.
    /// </summary>
    /// <typeparam name="T">Type to convert value to.</typeparam>
    /// <param name="configuration">Instance of application configuration.</param>
    /// <param name="key">Configuration key / environment variable key.</param>
    /// <returns>
    ///     If the configuration value is null or empty, returns the default value.
    ///     Otherwise, returns the configuration value converted to the specified type.
    /// </returns>
    public static T? GetOptionalConfiguration<T>(this IConfiguration configuration, string key) where T : struct
    {
        var valueStr = configuration[key];
        if (string.IsNullOrEmpty(valueStr))
            return null;

        return ConvertToType<T>(valueStr);
    }

    private static T ConvertToType<T>(string valueStr)
    {
        object? value;

        if (typeof(T) == typeof(ulong))
        {
            value = ulong.Parse(valueStr);
        }
        else
        {
            throw new NotImplementedException($"Type {typeof(T).FullName} conversion not implemented.");
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}