using EntityFramework.Exceptions.Common;

namespace Alyx.Discord.Bot.Interfaces;

internal interface IInteractionDataService
{
    /// <summary>
    ///     Add data to cache. A new key identifying the object in the database will be returned.
    /// </summary>
    /// <param name="data">Data to cache.</param>
    /// <typeparam name="T">Type of data to cache. Data is serialized and saved to database as json.</typeparam>
    /// <returns>Newly generated key identifying the object in the database.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="data" /> is null.</exception>
    /// <exception cref="UniqueConstraintException">
    ///     Thrown if data for key already exists. Should never happen as key is automatically generated.
    /// </exception>
    Task<Guid> AddDataAsync<T>(T data);

    /// <inheritdoc cref="AddDataAsync{T}(T)" />
    /// <returns>Data Component ID to be used for Discord components. Format: ComponentId/DataKey</returns>
    /// <remarks>There is no overload for submitting data ID. It must always be automatically generated.</remarks>
    Task<string> AddDataAsync<T>(T data, string componentId);

    /// <summary>
    ///     Creates a new component ID by replacing an existing data component ID's component ID with a new one.
    ///     Allows referencing the same cached data multiple times.
    /// </summary>
    /// <param name="existingDataComponentId">The existing data component ID.</param>
    /// <param name="newComponentId">The new component ID to combine with the existing one.</param>
    /// <returns>The newly generated component ID.</returns>
    string CreateDataComponentIdFromExisting(string existingDataComponentId, string newComponentId);

    /// <summary>
    ///     Get data for given ID and remove from cache.
    /// </summary>
    /// <param name="key">Key previously returned by <see cref="AddDataAsync{T}(T)" />.</param>
    /// <typeparam name="T">Data type. Data is serialized and saved to database as json.</typeparam>
    /// <returns>Data from database.</returns>
    /// <exception cref="ArgumentException">Thrown if type does not match object.</exception>
    /// <exception cref="InvalidOperationException">Thrown data not found or could not be deserialized.</exception>
    Task<T> GetDataAsync<T>(string key);

    /// <summary>
    ///     Parse component string and return data.
    /// </summary>
    /// <param name="componentString">Component string with Component ID and Data key. Format: ComponentId/DataKey</param>
    /// <typeparam name="T">Data type. Data is serialized and saved to database as json.</typeparam>
    /// <returns>Data from database.</returns>
    /// <exception cref="ArgumentException">Thrown if type does not match object.</exception>
    /// <exception cref="InvalidOperationException">Thrown data not found or could not be deserialized.</exception>
    Task<T> ParseStringAndGetData<T>(string componentString);
}