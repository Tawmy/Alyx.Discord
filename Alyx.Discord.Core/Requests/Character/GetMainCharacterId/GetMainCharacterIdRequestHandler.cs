using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Core.Requests.Character.GetMainCharacterId;

public class GetMainCharacterIdRequestHandler(DatabaseContext context)
    : IRequestHandler<GetMainCharacterIdRequest, string>
{
    public async Task<string> Handle(GetMainCharacterIdRequest request, CancellationToken cancellationToken)
    {
        var result = await context.Characters.FirstOrDefaultAsync(x =>
                x.IsMainCharacter &&
                x.Confirmed &&
                x.DiscordId == request.DiscordId,
            cancellationToken);

        if (result is null)
        {
            throw new NotFoundException();
        }

        return result.LodestoneId;
    }
}