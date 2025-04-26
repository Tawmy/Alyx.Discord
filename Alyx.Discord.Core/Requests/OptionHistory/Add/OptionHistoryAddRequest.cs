using Alyx.Discord.Db.Models;
using MediatR;

namespace Alyx.Discord.Core.Requests.OptionHistory.Add;

public record OptionHistoryAddRequest(ulong DiscordUserId, HistoryType HistoryType, string Value) : IRequest;