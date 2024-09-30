using MediatR;

namespace Alyx.Discord.Core.Requests.InteractionData.Get;

public record InteractionDataGetRequest<T>(string Key, Type Type) : IRequest<T>;