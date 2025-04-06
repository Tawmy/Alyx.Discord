using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacter;

public class CharacterGetCharacterRequestHandler(INetStoneApiCharacter apiCharacter)
    : IRequestHandler<CharacterGetCharacterRequest, CharacterDtoV3>
{
    public Task<CharacterDtoV3> Handle(CharacterGetCharacterRequest request, CancellationToken cancellationToken)
    {
        // never refresh cache here, not necessary
        return apiCharacter.GetAsync(request.LodestoneId, cancellationToken: cancellationToken);
    }
}