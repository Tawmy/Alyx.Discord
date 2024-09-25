using System.Collections.Concurrent;
using System.Text;
using Alyx.Discord.Bot.Interfaces;

namespace Alyx.Discord.Bot.Services;

internal class DataPersistenceService : IDataPersistenceService
{
    private const int CacheSize = 10000;

    private readonly ConcurrentDictionary<string, (Type, object)> _data = [];

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

        if (!_data.TryAdd(id, (typeof(T), data)))
        {
            // should never happen because of loop right beforehand
            throw new InvalidOperationException("Data for the given ID already exists.");
        }

        if (_data.Count > CacheSize)
        {
            // keep dictionary size below CacheSize
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

    public T GetData<T>(string id)
    {
        var (type, data) = _data[id];

        if (type != typeof(T))
        {
            throw new ArgumentException("Type of data for given ID does not match.");
        }

        var converted = (T)Convert.ChangeType(data, typeof(T));
        _data.TryRemove(id, out _);
        return converted;
    }

    public T ParseStringAndGetData<T>(string componentString)
    {
        var split = componentString.Split('/');
        if (split.Length != 2)
        {
            throw new ArgumentException("Format for given component string is not valid.");
        }

        return GetData<T>(split[1]);
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