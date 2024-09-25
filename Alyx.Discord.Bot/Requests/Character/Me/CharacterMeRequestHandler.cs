using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.Sheet;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.Requests.Character.Me;

internal class CharacterMeRequestHandler(ISender sender, DiscordEmbedService embedService)
    : IRequestHandler<CharacterMeRequest>
{
    public async Task Handle(CharacterMeRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var embed = embedService.CreateError(Messages.Commands.Character.Me.NotFoundDescription,
                Messages.Commands.Character.Me.NotFoundTitle);
            await request.Ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        await request.Ctx.DeferResponseAsync();

        var sheet = await sender.Send(new CharacterSheetRequest(lodestoneId), cancellationToken);

        await using var stream = new MemoryStream();
        await sheet.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"{DateTime.UtcNow:yyyy-MM-dd HH-mm} {lodestoneId}.webp";

        var button = CreateLodestoneLinkButton(lodestoneId);

        var builder = new DiscordInteractionResponseBuilder().AddFile(fileName, stream, true).AddComponents(button);
        await request.Ctx.FollowupAsync(builder);
    }

    private static DiscordLinkButtonComponent CreateLodestoneLinkButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.OpenLodestoneProfile);
    }
}