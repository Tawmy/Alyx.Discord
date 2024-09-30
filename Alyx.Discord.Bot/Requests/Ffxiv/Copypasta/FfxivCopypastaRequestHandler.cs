using DSharpPlus.Entities;
using MediatR;
using SixLabors.ImageSharp.Formats.Webp;
using CoreRequest = Alyx.Discord.Core.Requests.Ffxiv.Copypasta.FfxivCopypastaRequest;

namespace Alyx.Discord.Bot.Requests.Ffxiv.Copypasta;

internal class FfxivCopypastaRequestHandler(ISender sender) : IRequestHandler<FfxivCopypastaRequest>
{
    public async Task Handle(FfxivCopypastaRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var response = await sender.Send(new CoreRequest(), cancellationToken);

        await using var stream = new MemoryStream();
        await response.Image.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var builder = new DiscordInteractionResponseBuilder();
        builder.AddFile("haveyouheard.webp", stream, true);
        await request.Ctx.FollowupAsync(builder);
    }
}