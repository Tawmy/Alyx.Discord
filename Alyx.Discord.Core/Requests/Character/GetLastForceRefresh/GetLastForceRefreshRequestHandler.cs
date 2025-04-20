using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;

public class GetLastForceRefreshRequestHandler(DatabaseContext context)
    : IRequestHandler<GetLastForceRefreshRequest, DateTime?>
{
    public async Task<DateTime?> Handle(GetLastForceRefreshRequest request, CancellationToken cancellationToken)
    {
        var character = await context.Characters.FirstOrDefaultAsync(x =>
            x.LodestoneId == request.LodestoneId && x.Confirmed, cancellationToken);

        return character?.LastForceRefresh;
    }
}