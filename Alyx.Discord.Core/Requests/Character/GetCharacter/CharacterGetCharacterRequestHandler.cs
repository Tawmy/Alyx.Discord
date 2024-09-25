using MediatR;
using NetStone.Api.Client;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacter;

public class CharacterGetCharacterRequestHandler(NetStoneApiClient client) : IRequestHandler<CharacterGetCharacterRequest, CharacterDto>
{
    public Task<CharacterDto> Handle(CharacterGetCharacterRequest request, CancellationToken cancellationToken)
    {
        // never refresh cache here, not necessary
        return client.Character.GetAsync(request.lodestoneId, cancellationToken: cancellationToken);
    }
}