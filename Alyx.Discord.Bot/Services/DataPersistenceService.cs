using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Alyx.Discord.Bot.Interfaces;

namespace Alyx.Discord.Bot.Services;

internal class DataPersistenceService : IDataPersistenceService
{
    private const int CacheSize = 10000;

    private readonly ConcurrentDictionary<string, (Type Type, string SerializedObject)> _data = [];

    public string AddData<T>(T data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        string id;
        do
        {
            // ensure no duplicate key is generated
            id = GenerateNewId();
        } while (_data.ContainsKey(id));

        if (!_data.TryAdd(id, (typeof(T), JsonSerializer.Serialize(data))))
        {
            // should never happen because of loop right beforehand
            throw new InvalidOperationException("Data for the given ID already exists.");
        }

        if (_data.Count > CacheSize)
        {
            // keep dictionary size below CacheSize
            // TODO test this
            var first = _data.First();
            _data.Remove(first.Key, out _);
        }

        return id;
    }

    public string AddData<T>(T data, string componentId)
    {
        var dataId = AddData(data);
        return $"{componentId}/{dataId}";
    }

    public bool TryGetData<T>(string id, [MaybeNullWhen(false)] out T data)
    {
        if (!_data.TryGetValue(id, out var result))
        {
            data = default;
            return false;
        }

        if (result.Type != typeof(T))
        {
            throw new ArgumentException("Type of data for given ID does not match.");
        }

        data = JsonSerializer.Deserialize<T>(result.SerializedObject);
        return data is not null;
    }

    public bool TryParseStringAndGetData<T>(string componentString, [MaybeNullWhen(false)] out T value)
    {
        var split = componentString.Split('/');
        if (split.Length != 2)
        {
            throw new ArgumentException("Format for given component string is not valid.");
        }

        if (TryGetData<T>(split[1], out var data))
        {
            value = data;
            return true;
        }

        value = default;
        return false;
    }

    private static string GenerateNewId()
    {
        var stringBuilder = new StringBuilder();

        // generate a 32 characters long random string
        for (var i = 0; i < 4; i++)
        {
            var path = Path.GetRandomFileName();
            stringBuilder.Append(path[..8]);
        }

        return stringBuilder.ToString();
    }
}