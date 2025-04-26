using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Unclaim;

internal class CharacterUnclaimRequestHandler(ISender sender, CachingService cachingService)
    : IRequestHandler<CharacterUnclaimRequest>
{
    public async Task Handle(CharacterUnclaimRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var errorBuilder = new DiscordInteractionResponseBuilder()
                .AddError(Messages.Commands.Character.Unclaim.NoMainCharacterDescription(
                    request.GetSlashCommandMapping(),
                    "character claim"), Messages.Commands.Character.Unclaim.NoMainCharacterTitle);
            await request.Ctx.RespondAsync(errorBuilder);
            return;
        }

        await request.Ctx.DeferResponseAsync(true);

        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId), cancellationToken);

        var builder = CreateConfirmationBuiler(character);
        await request.Ctx.FollowupAsync(builder);
    }

    private DiscordFollowupMessageBuilder CreateConfirmationBuiler(CharacterDtoV3 character)
    {
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components();
        builder.AddContainerComponent(new DiscordContainerComponent([
            new DiscordTextDisplayComponent($"# {Messages.Commands.Character.Unclaim.Title}"),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            character.ToSectionComponent(cachingService),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordTextDisplayComponent(Messages.Commands.Character.Unclaim.ConfirmDescription),
            new DiscordSeparatorComponent(spacing: DiscordSeparatorSpacing.Small),
            new DiscordSectionComponent(
                new DiscordTextDisplayComponent(Messages.Commands.Character.Unclaim.ButtonDescription),
                new DiscordButtonComponent(DiscordButtonStyle.Danger, ComponentIds.Button.ConfirmUnclaim,
                    Messages.Buttons.ConfirmUnclaim))
        ]));

        return builder;
    }
}