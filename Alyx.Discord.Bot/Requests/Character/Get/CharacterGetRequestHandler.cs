using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using Alyx.Discord.Core.Requests.Character.Sheet;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.Requests.Character.Get;

public class CharacterGetRequestHandler(ISender sender) : IRequestHandler<CharacterGetRequest>
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
            var content = $"Could not find {request.Name} on {request.World}.";
            builder = new DiscordInteractionResponseBuilder().WithContent(content);
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

            var sheet = await sender.Send(new CharacterSheetRequest(first.Id), cancellationToken);

            await using var stream = new MemoryStream();
            await sheet.SaveAsync(stream, new WebpEncoder(), cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd HH-mm} {first.Name}.webp";

            builder = new DiscordInteractionResponseBuilder().AddFile(fileName, stream, true);
            await request.Ctx.FollowupAsync(builder);
        }
    }
}