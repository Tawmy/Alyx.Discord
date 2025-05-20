using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacterByName;

public class CharacterGetCharacterByNameRequestHandler(INetStoneApiCharacter apiCharacter)
    : IRequestHandler<CharacterGetCharacterByNameRequest, CharacterDto>
{
    public Task<CharacterDto> Handle(CharacterGetCharacterByNameRequest request, CancellationToken cancellationToken)
    {
        return apiCharacter.GetByNameAsync(request.Name, request.World, cancellationToken);
    }
}