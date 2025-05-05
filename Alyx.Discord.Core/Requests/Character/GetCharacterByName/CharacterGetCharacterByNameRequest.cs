using MediatR;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacterByName;

public record CharacterGetCharacterByNameRequest(string Name, string World) : IRequest<CharacterDtoV3>;