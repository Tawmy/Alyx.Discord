using System.Diagnostics.CodeAnalysis;

namespace Alyx.Discord.Bot.Interfaces;

internal interface IDataPersistenceService
{
    /// <summary>
    ///     Add data to cache. A new ID identifying the object in the cache will be returned.
    /// </summary>
    /// <param name="data">Data to cache.</param>
    /// <typeparam name="T">Type of data to cache. Collection is generic and uses object types.</typeparam>
    /// <returns>Newly generated ID identifying the object in the cache.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="data" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if data for ID already exists. Should never happen as ID is
    ///     automatically generated.
    /// </exception>
    string AddData<T>(T data);

    /// <inheritdoc cref="AddData{T}(T)" />
    /// <returns>Component ID to be used for Discord components. Format: ComponentId/CacheId</returns>
    /// <remarks>There is no overload for submitting data ID. It must always be automatically generated.</remarks>
    string AddData<T>(T data, string componentId);

    /// <summary>
    ///     Get data for given ID and remove from cache.
    /// </summary>
    /// <param name="id">Data it previously returned by <see cref="AddData{T}(T)" />.</param>
    /// <param name="data">Data from cache, if found.</param>
    /// <typeparam name="T">Data type. Collection is generic and saved as object.</typeparam>
    /// <returns>True if data was found in cache, false if not.</returns>
    /// <exception cref="ArgumentException">Thrown if type does not match object.</exception>
    bool TryGetData<T>(string id, [MaybeNullWhen(false)] out T data);

    /// <summary>
    ///     Parse component string and return data.
    /// </summary>
    /// <param name="componentString">Component string with Component ID and Data ID. Format: ComponentId/CacheId</param>
    /// <param name="data"></param>
    /// <typeparam name="T">Data from cache, if found.</typeparam>
    /// <returns>True if data was found in cache, false if not.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    bool TryParseStringAndGetData<T>(string componentString, [MaybeNullWhen(false)] out T data);
}