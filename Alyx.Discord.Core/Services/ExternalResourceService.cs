using System.Collections.Frozen;
using Alyx.Discord.Core.Enums;
using Microsoft.Extensions.Logging;
using NetStone.Common.Enums;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Alyx.Discord.Core.Services;

internal class ExternalResourceService
{
    private readonly Lazy<Task> _initializeTask;
    private readonly ILogger<ExternalResourceService> _logger;

    private FontCollection? _fontCollection = new();
    private FrozenDictionary<CharacterSheetImage, Image> _images = FrozenDictionary<CharacterSheetImage, Image>.Empty;
    private FrozenDictionary<ClassJob, Image> _jobIcons = FrozenDictionary<ClassJob, Image>.Empty;

    public ExternalResourceService(ILogger<ExternalResourceService> logger)
    {
        _logger = logger;
        _initializeTask = new Lazy<Task>(InitializeAsync);
    }

    public async Task<Image> GetImageAsync(CharacterSheetImage image)
    {
        await _initializeTask.Value;

        return _images[image].CloneAs<Rgba32>();
    }

    public async Task<Image> GetJobIconAsync(ClassJob job)
    {
        await _initializeTask.Value;

        return _jobIcons[job];
    }

    public async Task<Image> GetGrandCompanyCrestAsync(GrandCompany grandCompany)
    {
        await _initializeTask.Value;

        return grandCompany switch
        {
            GrandCompany.NoAffiliation => throw new InvalidOperationException("No affiliation, no crest."),
            GrandCompany.Maelstrom => await GetImageAsync(CharacterSheetImage.GcMaelstrom),
            GrandCompany.OrderOfTheTwinAdder => await GetImageAsync(CharacterSheetImage.GcTwinAdder),
            GrandCompany.ImmortalFlames => await GetImageAsync(CharacterSheetImage.GcImmortalFlames),
            _ => throw new ArgumentOutOfRangeException(nameof(grandCompany), grandCompany, null)
        };
    }

    public async Task<FontFamily> GetFontFamilyAsync(CharacterSheetFont font)
    {
        await _initializeTask.Value;

        return font switch
        {
            CharacterSheetFont.OpenSans when _fontCollection is not null => _fontCollection.Get("Open Sans"),
            CharacterSheetFont.Vollkorn when _fontCollection is not null => _fontCollection.Get("Vollkorn"),
            CharacterSheetFont.Antonio when _fontCollection is not null => _fontCollection.Get("Antonio ExtraLight"),
            _ => throw new ArgumentOutOfRangeException(nameof(font), font, "Font not in collection.")
        };
    }

    private async Task InitializeAsync()
    {
        _images = await LoadImagesAsync();
        _jobIcons = await LoadJobIconsAsync();
        _fontCollection = LoadFonts();

        _logger.LogInformation("External sources loaded successfully.");
    }

    private static async Task<FrozenDictionary<CharacterSheetImage, Image>> LoadImagesAsync()
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