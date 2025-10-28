using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Core.Requests.InteractionData.Add;
using Alyx.Discord.Core.Requests.InteractionData.Get;
using MediatR;

namespace Alyx.Discord.Bot.Services;

internal class InteractionDataService(ISender sender) : IInteractionDataService
{
    public async Task<Guid> AddDataAsync<T>(T data, CancellationToken cancellationToken = default)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var response = await sender.Send(new InteractionDataAddRequest<T>(data, typeof(T)), cancellationToken);
        return response.Key;
    }

    public async Task<string> AddDataAsync<T>(T data, string componentId, CancellationToken cancellationToken = default)
    {
        var dataId = await AddDataAsync(data, cancellationToken);
        return CreateNewDataComponentId(componentId, dataId.ToString());
    }

    public string CreateDataComponentIdFromExisting(string existingDataComponentId, string newComponentId)
    {
        var dataId = existingDataComponentId.Split('/')[1];
        return CreateNewDataComponentId(newComponentId, dataId);
    }

    public Task<T> GetDataAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return sender.Send(new InteractionDataGetRequest<T>(key, typeof(T)), cancellationToken);
    }

    public Task<T> ParseStringAndGetData<T>(string componentString, CancellationToken cancellationToken = default)
    {
        var split = componentString.Split('/');
        if (split.Length != 2)
        {
            throw new ArgumentException("Format for given component string is not valid.");
        }

        return GetDataAsync<T>(split[1], cancellationToken);
    }

    private static string CreateNewDataComponentId(string componentId, string dataId)
    {
        var fullId = $"{componentId}/{dataId}";

        if (fullId.Length > 100)
        {
            throw new InvalidOperationException(
                $"""
                 Submitted component id too long. Data id requires 36 characters, separator 1, leaving 63 for the component id. 
                 Submitted component id is {componentId.Length} characters, thus exceeding the allowed total of 100.
                 """);
        }

        return fullId;
    }
}