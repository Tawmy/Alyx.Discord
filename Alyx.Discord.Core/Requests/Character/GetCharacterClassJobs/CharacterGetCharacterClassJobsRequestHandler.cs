using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;

namespace Alyx.Discord.Core.Requests.Character.GetCharacterClassJobs;

public class CharacterGetCharacterClassJobsRequestHandler(INetStoneApiCharacter apiCharacter)
    : IRequestHandler<CharacterGetCharacterClassJobsRequest, CharacterClassJobOuterDto>
{
    public Task<CharacterClassJobOuterDto> Handle(CharacterGetCharacterClassJobsRequest request,
        CancellationToken cancellationToken)
    {
        return apiCharacter.GetClassJobsAsync(request.LodestoneId, request.MaxAge, FallbackType.Any, cancellationToken);
    }
}