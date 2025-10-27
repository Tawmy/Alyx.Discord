using Alyx.Discord.Core.Records;
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

        var typeName = typeof(IEnumerable<SheetMetadata>).FullName;
        var sheetsRequested = await context.InteractionDatas.Where(x => x.Type.Equals(typeName))
            .CountAsync(cancellationToken);

        return new StatisticsRequestResponse(claimedCharacters, sheetsRequested);
    }
}