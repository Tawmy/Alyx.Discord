using Alyx.Discord.Db.Models;
using MediatR;

namespace Alyx.Discord.Core.Requests.OptionHistory.Get;

public record OptionHistoryGetRequest(ulong DiscordUserId, HistoryType HistoryType) : IRequest<ICollection<string>>;