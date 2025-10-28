using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.FreeCompany.Me;

internal record FreeCompanyMeRequest(SlashCommandContext Ctx, bool ForceRefresh, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;