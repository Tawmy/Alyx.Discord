using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Search;

public record CharacterSearchRequest(string Name, string World) : IRequest<string>;