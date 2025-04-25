using Alyx.Discord.Core.Requests.Character.Sheet;
using MediatR;
using Microsoft.AspNetCore.Components;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Api.Components;

public partial class Demo : ComponentBase
{
    private const string LodestoneId = "28812634";
    private string? _imageSource;
    private bool _showMessage;

    [Inject] public required ISender Sender { get; set; }

    private async Task LoadCharacterSheetAsync()
    {
        _showMessage = true;
        await InvokeAsync(StateHasChanged);

        var sheet = await Sender.Send(new CharacterSheetRequest(LodestoneId, false));
        var image = sheet.Image;
        _imageSource = image.ToBase64String(WebpFormat.Instance);
        await InvokeAsync(StateHasChanged);
    }
}