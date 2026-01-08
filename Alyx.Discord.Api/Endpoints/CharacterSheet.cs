using FastEndpoints;
using MediatR;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Api.Endpoints;

public record CharacterSheetRequest
{
    public required string LodestoneId { get; init; }
}

public class CharacterSheet(ISender sender) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/sheet/{lodestoneId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CT ct)
    {
        var lodestoneId = Route<string>("lodestoneId");

        if (lodestoneId is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var image = await sender.Send(new Core.Requests.Character.Sheet.CharacterSheetRequest(lodestoneId, false), ct);

        var memoryStream = new MemoryStream();
        await image.Image.SaveAsWebpAsync(memoryStream, ct);
        memoryStream.Seek(0, SeekOrigin.Begin);

        await Send.BytesAsync(memoryStream.ToArray(), $"{lodestoneId}.webp", "image/webp", cancellation: ct);
    }
}