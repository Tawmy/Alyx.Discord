using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.FreeCompany.Get;

internal record FreeCompanyGetRequest(SlashCommandContext Ctx, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;