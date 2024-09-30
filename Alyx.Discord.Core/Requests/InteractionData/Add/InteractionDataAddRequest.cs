using MediatR;

namespace Alyx.Discord.Core.Requests.InteractionData.Add;

public record InteractionDataAddRequest<T>(T Value, Type Type) : IRequest<InteractionDataAddResponse>;