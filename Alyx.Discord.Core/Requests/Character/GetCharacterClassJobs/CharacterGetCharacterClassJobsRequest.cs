using MediatR;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Core.Requests.Character.GetCharacterClassJobs;

public record CharacterGetCharacterClassJobsRequest(string LodestoneId, int? MaxAge = null)
    : IRequest<CharacterClassJobOuterDto>;