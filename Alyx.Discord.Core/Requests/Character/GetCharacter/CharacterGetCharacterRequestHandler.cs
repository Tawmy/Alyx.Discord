using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;

namespace Alyx.Discord.Core.Requests.Character.GetCharacter;

public class CharacterGetCharacterRequestHandler(INetStoneApiCharacter apiCharacter)
    : IRequestHandler<CharacterGetCharacterRequest, CharacterDto>
{
    public Task<CharacterDto> Handle(CharacterGetCharacterRequest request, CancellationToken cancellationToken)
    {
        return apiCharacter.GetAsync(request.LodestoneId, request.MaxAge, FallbackType.Any, cancellationToken);
    }
}