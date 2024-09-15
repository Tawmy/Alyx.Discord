using MediatR;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.Search;

public record CharacterSearchRequest(string Name, string World) : IRequest<ICollection<CharacterSearchPageResultDto>>;