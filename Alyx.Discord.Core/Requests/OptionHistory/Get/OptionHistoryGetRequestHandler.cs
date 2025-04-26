using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.OptionHistory.Get;

public class OptionHistoryGetRequestHandler(DatabaseContext context)
    : IRequestHandler<OptionHistoryGetRequest, ICollection<string>>
{
    public async Task<ICollection<string>> Handle(OptionHistoryGetRequest request, CancellationToken cancellationToken)
    {
        return await context.OptionHistories.Where(x =>
                x.DiscordId == request.DiscordUserId &&
                x.HistoryType == request.HistoryType)
            .OrderByDescending(x => x.LastUsed)
            .Select(x => x.Value)
            .ToArrayAsync(cancellationToken);
    }
}