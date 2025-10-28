using Alyx.Discord.Bot.Services.CharacterJobs;
using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Jobs.Me;

internal record CharacterJobsMeRequest(SlashCommandContext Ctx, Role Role, bool ForceRefresh, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;