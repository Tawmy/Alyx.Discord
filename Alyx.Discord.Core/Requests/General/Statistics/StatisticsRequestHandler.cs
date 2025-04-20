using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.General.Statistics;

internal class StatisticsRequestHandler(DatabaseContext context)
    : IRequestHandler<StatisticsRequest, StatisticsRequestResponse>
{
    public async Task<StatisticsRequestResponse> Handle(StatisticsRequest request, CancellationToken cancellationToken)
    {
        var claimedCharacters = await context.Characters.Where(x => x.Confirmed).CountAsync(cancellationToken);

        return new StatisticsRequestResponse(claimedCharacters);
    }
}