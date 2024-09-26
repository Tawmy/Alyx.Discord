using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.Character.Unclaim;

internal class CharacterUnclaimRequestHandler(DatabaseContext context) : IRequestHandler<CharacterUnclaimRequest>
{
    public async Task Handle(CharacterUnclaimRequest request, CancellationToken cancellationToken)
    {
        var mainCharacter = await context.Characters.FirstAsync(x =>
                x.IsMainCharacter &&
                x.Confirmed &&
                x.DiscordId == request.DiscordId,
            cancellationToken);

        context.Characters.Remove(mainCharacter);
        await context.SaveChangesAsync(cancellationToken);
    }
}