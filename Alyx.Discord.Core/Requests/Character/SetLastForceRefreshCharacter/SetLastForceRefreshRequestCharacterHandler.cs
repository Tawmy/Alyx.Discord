using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefreshCharacter;

public class SetLastForceRefreshRequestCharacterHandler(DatabaseContext context)
    : IRequestHandler<SetLastForceRefreshCharacterRequest>
{
    public async Task Handle(SetLastForceRefreshCharacterRequest request, CancellationToken cancellationToken)
    {
        var character = await context.Characters.FirstAsync(x =>
            x.LodestoneId == request.LodestoneId && x.Confirmed, cancellationToken);

        character.LastForceRefresh = request.LastForceRefresh;

        await context.SaveChangesAsync(cancellationToken);
    }
}