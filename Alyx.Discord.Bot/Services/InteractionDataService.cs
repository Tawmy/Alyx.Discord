using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Core.Requests.InteractionData.Add;
using Alyx.Discord.Core.Requests.InteractionData.Get;
using MediatR;

namespace Alyx.Discord.Bot.Services;

internal class InteractionDataService(ISender sender) : IInteractionDataService
{
    public async Task<string> AddDataAsync<T>(T data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var response = await sender.Send(new InteractionDataAddRequest<T>(data, typeof(T)));
        return response.Key;
    }

    public async Task<string> AddDataAsync<T>(T data, string componentId)
    {
        var dataId = await AddDataAsync(data);
        return $"{componentId}/{dataId}";
    }

    public Task<T> GetDataAsync<T>(string key)
    {
        return sender.Send(new InteractionDataGetRequest<T>(key, typeof(T)));
    }

    public Task<T> ParseStringAndGetData<T>(string componentString)
    {
        var split = componentString.Split('/');
        if (split.Length != 2)
        {
            throw new ArgumentException("Format for given component string is not valid.");
        }

        return GetDataAsync<T>(split[1]);
    }
}