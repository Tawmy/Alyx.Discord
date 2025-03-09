using Alyx.Discord.Bot.Requests;
using DSharpPlus.Commands.Trees;

namespace Alyx.Discord.Bot.Extensions;

internal static class SlashCommandRequestExtensions
{
    public static IReadOnlyDictionary<ulong, Command> GetSlashCommandMapping(this SlashCommandRequest request)
    {
        return request.Ctx.Extension.GetSlashCommandMapping();
    }
}