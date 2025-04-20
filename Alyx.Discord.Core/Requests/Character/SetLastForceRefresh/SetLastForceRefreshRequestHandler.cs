using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefresh;

public class SetLastForceRefreshRequestHandler(DatabaseContext context) : IRequestHandler<SetLastForceRefreshRequest>
{
    public async Task Handle(SetLastForceRefreshRequest request, CancellationToken cancellationToken)
    {
        var character = await context.Characters.FirstAsync(x =>
            x.LodestoneId == request.LodestoneId && x.Confirmed, cancellationToken);

        character.LastForceRefresh = request.LastForceRefresh;

        await context.SaveChangesAsync(cancellationToken);
    }
}