using Alyx.Discord.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alyx.Discord.Core.Requests.OptionHistory.Add;

public class OptionHistoryAddRequestHandler(DatabaseContext context) : IRequestHandler<OptionHistoryAddRequest>
{
    public async Task Handle(OptionHistoryAddRequest request, CancellationToken cancellationToken)
    {
        var entries = await context.OptionHistories.Where(x =>
                x.DiscordId == request.DiscordUserId &&
                x.HistoryType == request.HistoryType)
            .ToListAsync(cancellationToken);

        if (entries.FirstOrDefault(x => x.Value.Equals(request.Value)) is { } existingEntry)
        {
            // Value was cached before, only update last used
            existingEntry.LastUsed = DateTime.UtcNow;
        }
        else
        {
            // Value was not cached before, add it
            if (entries.Count > 24)
            {
                // Discord auto complete providers have 25 entries max, remove the oldest entry
                var oldest = entries.OrderBy(x => x.LastUsed).First();
                context.OptionHistories.Remove(oldest);
            }

            await context.OptionHistories.AddAsync(new Db.Models.OptionHistory
            {
                DiscordId = request.DiscordUserId,
                HistoryType = request.HistoryType,
                Value = request.Value,
                LastUsed = DateTime.UtcNow
            }, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}