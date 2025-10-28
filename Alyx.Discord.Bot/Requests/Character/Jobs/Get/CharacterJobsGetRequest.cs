using Alyx.Discord.Bot.Services.CharacterJobs;
using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Jobs.Get;

internal record CharacterJobsGetRequest(SlashCommandContext Ctx, Role Role, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;