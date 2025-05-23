using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Extensions;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.FreeCompany.Me;

internal class FreeCompanyMeRequestHandler(
    ISender sender,
    [FromKeyedServices(FreeCompanyService.Key)]
    IDiscordContainerService<FreeCompanyDto> fcService,
    HttpClient httpClient) : IRequestHandler<FreeCompanyMeRequest>
{
    public async Task Handle(FreeCompanyMeRequest request, CancellationToken cancellationToken)
    {
        string mainCharacterLodestoneId;
        try
        {
            mainCharacterLodestoneId =
                await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var errorBuilder = new DiscordInteractionResponseBuilder().AddError(
                Messages.Commands.Character.Me.NotFoundDescription(request.GetSlashCommandMapping(), "character claim"),
                Messages.Commands.Character.Me.NotFoundTitle);
            await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
            return;
        }

        CharacterDto mainCharacter;
        try
        {
            mainCharacter = await sender.Send(new CharacterGetCharacterRequest(mainCharacterLodestoneId),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            // TODO show more appropriate error, maybe
            var errorBuilder = new DiscordInteractionResponseBuilder().AddError(
                Messages.Commands.Character.Me.NotFoundDescription(request.GetSlashCommandMapping(), "character claim"),
                Messages.Commands.Character.Me.NotFoundTitle);
            await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
            return;
        }

        if (mainCharacter.FreeCompany is null)
        {
            var errorBuilder = new DiscordInteractionResponseBuilder().AddError(
                Messages.Commands.FreeCompany.Me.MainCharacterNotInFreeCompanyDescription(mainCharacter.Name,
                    mainCharacter.Server),
                Messages.Commands.FreeCompany.Me.MainCharacterNotInFreeCompanyTitle);
            await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
            return;
        }

        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var container = await fcService.CreateContainerAsync(mainCharacter.FreeCompany.Id,
            cancellationToken: cancellationToken);

        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);

        var crest = await mainCharacter.FreeCompany.IconLayers.DownloadCrestAsync(httpClient);
        await using var _ = await builder.AddImageAsync(crest, Messages.FileNames.Crest, cancellationToken);

        await request.Ctx.RespondAsync(builder);
    }
}