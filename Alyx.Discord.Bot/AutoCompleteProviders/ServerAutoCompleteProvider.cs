using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.AutoCompleteProviders;

internal class ServerAutoCompleteProvider(
    ISender sender,
    INetStoneApiCharacter character,
    ILogger<ServerAutoCompleteProvider> logger) : IAutoCompleteProvider
{
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
    {
        var servers = Enum.GetValues<Server>().Select(x => x.ToString());

        if (!string.IsNullOrWhiteSpace(context.UserInput))
        {
            // user has started searching, filter server list
            servers = servers.Where(x => x.StartsWith(context.UserInput, StringComparison.OrdinalIgnoreCase));
            servers = servers.OrderBy(x => x);
        }
        else
        {
            // user has not yet started searching

            CharacterDto? mainCharacter = null;
            try
            {
                var request = new GetMainCharacterIdRequest(context.User.Id);
                var mainCharacterId = await sender.Send(request);
                mainCharacter = await character.GetAsync(mainCharacterId, useFallback: FallbackType.Any);
            }
            catch (NotFoundException)
            {
                // no main character, do nothing
            }
            catch (Exception e)
            {
                logger.LogWarning("Failed to retrieve main character for user {user}. Exception: {e}",
                    context.User.Id, e);
            }

            // show main character's home world first, then order alphabetically
            servers = servers.OrderBy(x => !x.Equals(mainCharacter?.Server, StringComparison.OrdinalIgnoreCase))
                .ThenBy(x => x);
        }

        servers = servers.Take(25);

        return servers.Select(x => new DiscordAutoCompleteChoice(x, x));
    }
}