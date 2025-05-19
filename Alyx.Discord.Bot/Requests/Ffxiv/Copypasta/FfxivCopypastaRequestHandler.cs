using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Entities;
using MediatR;
using CoreRequest = Alyx.Discord.Core.Requests.Ffxiv.Copypasta.FfxivCopypastaRequest;

namespace Alyx.Discord.Bot.Requests.Ffxiv.Copypasta;

internal class FfxivCopypastaRequestHandler(ISender sender) : IRequestHandler<FfxivCopypastaRequest>
{
    public async Task Handle(FfxivCopypastaRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var response = await sender.Send(new CoreRequest(), cancellationToken);

        var builder = new DiscordInteractionResponseBuilder().EnableV2Components();

        await using var _ = await builder.AddImageAsync(response.Image, Messages.FileNames.Copypasta,
            cancellationToken);
        const string fileName = $"attachment://{Messages.FileNames.Copypasta}.webp";
        builder.AddMediaGalleryComponent(new DiscordMediaGalleryItem(fileName));

        await request.Ctx.RespondAsync(builder);
    }
}