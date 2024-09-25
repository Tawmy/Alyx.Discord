using MediatR;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacter;

public record CharacterGetCharacterRequest(string lodestoneId) : IRequest<CharacterDto>;