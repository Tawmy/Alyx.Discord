using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Get;

internal class CharacterGetRequestHandler(
    ISender sender,
    DiscordEmbedService embedService,
    IDataPersistenceService dataPersistenceService)
    : IRequestHandler<CharacterGetRequest>
{
    public async Task Handle(CharacterGetRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        ICollection<CharacterSearchPageResultDto> searchDtos;
        DiscordInteractionResponseBuilder builder;
        try
        {
            searchDtos = await sender.Send(new CharacterSearchRequest(request.Name, request.World), cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.Character.Get.CharacterNotFound(request.Name, request.World);
            builder = new DiscordInteractionResponseBuilder().AddEmbed(embedService.CreateError(description));
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        if (request.IsPrivate && searchDtos.Count > 1)
        {
            var select = searchDtos.AsSelectComponent();
            var content = Messages.Commands.Character.Get.SelectMenu(searchDtos.Count);
            builder = new DiscordInteractionResponseBuilder().AddComponents(select).WithContent(content);
            await request.Ctx.FollowupAsync(builder);
        }
        else
        {
            var first = searchDtos.First();

            builder = new DiscordInteractionResponseBuilder();
            await builder.CreateSheetAndSendFollowupAsync(sender, dataPersistenceService, first.Id,
                async b => await request.Ctx.FollowupAsync(b), cancellationToken);
        }
    }
}