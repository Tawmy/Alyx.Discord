using System.Collections.Frozen;
using Alyx.Discord.Core.Enums;
using Microsoft.Extensions.Logging;
using NetStone.Common.Enums;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Alyx.Discord.Core.Services;

internal class ExternalResourceService(ILogger<ExternalResourceService> logger)
{
    private FrozenDictionary<CharacterSheetImage, Image> _characterSheetImages =
        FrozenDictionary<CharacterSheetImage, Image>.Empty;

    private FontCollection? _fontCollection = new();
    private FrozenDictionary<ClassJob, Image> _jobIcons = FrozenDictionary<ClassJob, Image>.Empty;
    private FrozenDictionary<string, Image> _other = FrozenDictionary<string, Image>.Empty;

    public async Task InitializeAsync()
    {
        _characterSheetImages = await LoadCharacterSheetImagesAsync();
        _jobIcons = await LoadJobIconsAsync();
        _other = await LoadOtherImagesAsync();
        _fontCollection = LoadFonts();

        logger.LogInformation("External resources loaded successfully.");
    }

    public Image GetCharacterSheetImage(CharacterSheetImage image)
    {
        return _characterSheetImages[image].CloneAs<Rgba32>();
    }

    public Image GetJobIcon(ClassJob job)
    {
        return _jobIcons[job];
    }

    public Image GetGrandCompanyCrest(GrandCompany grandCompany)
    {
        return grandCompany switch
        {
            GrandCompany.NoAffiliation => throw new InvalidOperationException("No affiliation, no crest."),
            GrandCompany.Maelstrom => GetCharacterSheetImage(CharacterSheetImage.GcMaelstrom),
            GrandCompany.OrderOfTheTwinAdder => GetCharacterSheetImage(CharacterSheetImage.GcTwinAdder),
            GrandCompany.ImmortalFlames => GetCharacterSheetImage(CharacterSheetImage.GcImmortalFlames),
            _ => throw new ArgumentOutOfRangeException(nameof(grandCompany), grandCompany, null)
        };
    }

    public Image GetOtherImage(string key)
    {
        return _other[key];
    }

    public FontFamily GetFontFamily(CharacterSheetFont font)
    {
        return font switch
        {
            CharacterSheetFont.OpenSans when _fontCollection is not null => _fontCollection.Get("Open Sans"),
            CharacterSheetFont.Vollkorn when _fontCollection is not null => _fontCollection.Get("Vollkorn"),
            CharacterSheetFont.Antonio when _fontCollection is not null => _fontCollection.Get("Antonio ExtraLight"),
            _ => throw new ArgumentOutOfRangeException(nameof(font), font, "Font not in collection.")
        };
    }

    private static async Task<FrozenDictionary<CharacterSheetImage, Image>> LoadCharacterSheetImagesAsync()
    {
        const string subdirectory = "Images/CharacterSheet";

        var templateBase = Path.Combine(AppContext.BaseDirectory, subdirectory, "characterSheetBase.png");
        var templateFrame = Path.Combine(AppContext.BaseDirectory, subdirectory, "characterSheetFrame.png");
        var templateJobCircle = Path.Combine(AppContext.BaseDirectory, subdirectory, "characterSheetJob.png");
        var gcMaelstrom = Path.Combine(AppContext.BaseDirectory, subdirectory, "chat_messengericon_town01.png");
        var gcTwinAdder = Path.Combine(AppContext.BaseDirectory, subdirectory, "chat_messengericon_town02.png");
        var gcImmortalFlames = Path.Combine(AppContext.BaseDirectory, subdirectory, "chat_messengericon_town03.png");

        var dict = new Dictionary<CharacterSheetImage, Image>
        {
            { CharacterSheetImage.TemplateBase, await Image.LoadAsync(templateBase) },
            { CharacterSheetImage.TemplateFrame, await Image.LoadAsync(templateFrame) },
            { CharacterSheetImage.TemplateJobCircle, await Image.LoadAsync(templateJobCircle) },
            { CharacterSheetImage.GcMaelstrom, await Image.LoadAsync(gcMaelstrom) },
            { CharacterSheetImage.GcTwinAdder, await Image.LoadAsync(gcTwinAdder) },
            { CharacterSheetImage.GcImmortalFlames, await Image.LoadAsync(gcImmortalFlames) }
        };

        return dict.ToFrozenDictionary();
    }

    private static async Task<FrozenDictionary<ClassJob, Image>> LoadJobIconsAsync()
    {
        const string subdirectory = "Images/JobIcons";

        var dict = new Dictionary<ClassJob, Image>();
        foreach (var job in (ClassJob[])Enum.GetValues(typeof(ClassJob)))
        {
            var path = Path.Combine(AppContext.BaseDirectory, subdirectory, $"{job.ToString().ToLowerInvariant()}.png");
            dict.Add(job, await Image.LoadAsync(path));
        }

        return dict.ToFrozenDictionary();
    }

    private async Task<FrozenDictionary<string, Image>> LoadOtherImagesAsync()
    {
        const string subdirectory = "Images/Other";

        var dict = new Dictionary<string, Image>();
        foreach (var filePath in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, subdirectory))
                     .Where(x => !x.StartsWith('.')))
        {
            dict.Add(Path.GetFileNameWithoutExtension(filePath), await Image.LoadAsync(filePath));
        }

        return dict.ToFrozenDictionary();
    }

    private static FontCollection LoadFonts()
    {
        const string subdirectory = "Fonts";

        var collection = new FontCollection();
        collection.Add(Path.Combine(AppContext.BaseDirectory, subdirectory, "OpenSans-VariableFont_wdth,wght.ttf"));
        collection.Add(Path.Combine(AppContext.BaseDirectory, subdirectory, "Vollkorn-VariableFont_wght.ttf"));
        collection.Add(Path.Combine(AppContext.BaseDirectory, subdirectory, "Antonio-ExtraLight.ttf"));
        return collection;
    }
}