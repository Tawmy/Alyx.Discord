using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefreshFreeCompany;

public class SetLastForceRefreshFreeCompanyRequestHandler(DatabaseContext context)
    : IRequestHandler<SetLastForceRefreshFreeCompanyRequest>
{
    public async Task Handle(SetLastForceRefreshFreeCompanyRequest request, CancellationToken cancellationToken)
    {
        var character = await context.Characters.FirstAsync(x =>
            x.LodestoneId == request.LodestoneId && x.Confirmed, cancellationToken);

        character.LastFcForceRefresh = request.LastForceRefresh;

        await context.SaveChangesAsync(cancellationToken);
    }
}