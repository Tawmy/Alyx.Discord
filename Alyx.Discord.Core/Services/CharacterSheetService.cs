using System.Numerics;
using Alyx.Discord.Core.Enums;
using Alyx.Discord.Core.StaticValues;
using Alyx.Discord.Core.Structs;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;
using NetStone.Common.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Alyx.Discord.Core.Services;

internal class CharacterSheetService
{
    private readonly ExternalResourceService _externalResourceService;
    private readonly HttpClient _httpClient;

    private readonly Lazy<Task<Image>> _imageTask;

    private Image? _image;

    public CharacterSheetService(ExternalResourceService externalResourceService, HttpClient httpClient)
    {
        _externalResourceService = externalResourceService;
        _httpClient = httpClient;

        _imageTask = new Lazy<Task<Image>>(InitializeAsync);
    }

    public async Task<Image> CreateCharacterSheetAsync(CharacterDto character,
        CharacterClassJobOuterDto? classJobs = null,
        CollectionDto<CharacterMinionDto>? minions = null, CollectionDto<CharacterMountDto>? mounts = null)
    {
        _image = await _imageTask.Value;

        await AddCharacterPortraitAsync(character);
        await AddPortraitFrameAsync();

        await AddJobIconAsync(character);
        await AddJobFrameAsync();
        await AddActiveJobLevelAsync(character);

        await AddCharacterNameAsync(character);
        await AddHomeWorldAsync(character);

        await AddILvlMinionsMountsAsync(character, minions, mounts);

        if (classJobs is not null)
        {
            await AddJobLevelsAsync(classJobs);
        }

        await AddGrandCompanyAsync(character);

        // TODO grand company, free company, attributes, and new adventurer

        return _image;
    }

    public Image GetSheet()
    {
        if (_image is null)
        {
            throw new InvalidOperationException(
                "Sheet has not been created yet. Create it using CreateCharacterSheetAsync.");
        }

        return _image;
    }

    private Task<Image> InitializeAsync()
    {
        return _externalResourceService.GetImageAsync(CharacterSheetImage.TemplateBase);
    }

    private async Task AddCharacterPortraitAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        // get portrait as byte[]
        var result = await _httpClient.GetByteArrayAsync(character.Portrait);

        // load image and resize
        var imgPortrait = Image.Load<Rgba32>(result);
        imgPortrait.Mutate(x => x.Resize(592, 808, LanczosResampler.Lanczos3));

        // crop image
        const int coordX = 464;
        const int coordY = 808;
        var rect = new Rectangle(imgPortrait.Width / 2 - coordX / 2, imgPortrait.Height / 2 - coordY / 2, coordX,
            coordY);
        imgPortrait.Mutate(x => x.Crop(rect));

        // draw cropped portrait on top of base image
        _image.Mutate(x => x.DrawImage(imgPortrait, new Point(16, 66), 1));
    }

    private async Task AddPortraitFrameAsync()
    {
        _image = await _imageTask.Value;

        var imgFrame = await _externalResourceService.GetImageAsync(CharacterSheetImage.TemplateFrame);
        _image.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private async Task AddJobIconAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        var imgJob = await _externalResourceService.GetJobIconAsync(character.ActiveClassJob);
        _image.Mutate(x => x.DrawImage(imgJob, CharacterSheetCoordinates.Other.JobIcon, 1));
    }

    private async Task AddJobFrameAsync()
    {
        _image = await _imageTask.Value;

        var imgJob = await _externalResourceService.GetImageAsync(CharacterSheetImage.TemplateJobCircle);
        _image.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private async Task AddActiveJobLevelAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        var circle = new EllipsePolygon(CharacterSheetCoordinates.Other.ActiveJobLevelBackground,
            CharacterSheetValues.ActiveJobLevelRadius);
        _image.Mutate(x => x.Draw(new Color(CharacterSheetValues.ActiveJobLevelBackground),
            CharacterSheetValues.ActiveJobLevelThickness, circle));

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.Antonio);
        var font = family.CreateFont(CharacterSheetValues.ActiveJobLevelFontSize, FontStyle.Regular);
        var text = $"Lv. {character.ActiveClassJobLevel}";

        var textOptions = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = CharacterSheetCoordinates.Other.ActiveJobLevelText
        };

        _image.Mutate(x => x.DrawText(textOptions, text, Color.White));
    }

    private async Task AddCharacterNameAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.Vollkorn);
        var nameProperties = new NameProperties(character.Title);

        var fontName = family.CreateFont(nameProperties.Name.Size, FontStyle.Regular);

        var optionsName = new RichTextOptions(fontName)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Name.X, nameProperties.Name.Y)
        };

        // print character name
        _image.Mutate(x => x.DrawText(optionsName, character.Name, Color.Black));

        if (nameProperties.Title is null)
        {
            // no title, return
            return;
        }

        var fontTitle = family.CreateFont(nameProperties.Title.Value.Size, FontStyle.Regular);

        var optionsTitle = new RichTextOptions(fontTitle)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Title.Value.X, nameProperties.Title.Value.Y)
        };

        // print character title
        _image.Mutate(x => x.DrawText(optionsTitle, character.Title!, Color.Black));
    }

    private async Task AddHomeWorldAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeHomeWorld, FontStyle.Regular);

        var options = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = CharacterSheetCoordinates.Other.HomeWorld
        };

        _image.Mutate(x => x.DrawText(options, character.Server, Color.White));
    }

    private async Task AddILvlMinionsMountsAsync(CharacterDto character, CollectionDto<CharacterMinionDto>? minions,
        CollectionDto<CharacterMountDto>? mounts)
    {
        _image = await _imageTask.Value;

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeMiMo, FontStyle.Regular);

        var optionsLvl = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Origin = CharacterSheetCoordinates.Other.ItemLevel
        };

        var avgItemLevel = character.Gear.GetAvarageItemLevel();
        _image.Mutate(x => x.DrawText(optionsLvl, avgItemLevel.ToString(), Color.White));

        var optionsMi = new RichTextOptions(optionsLvl)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            Origin = CharacterSheetCoordinates.Other.Minions
        };

        if (minions is not null)
        {
            var text = Math.Round(minions.CollectedPercentage);
            _image.Mutate(x => x.DrawText(optionsMi, $"{text}%", Color.White));
        }

        if (mounts is not null)
        {
            var optionsMo = new RichTextOptions(optionsMi)
            {
                Origin = CharacterSheetCoordinates.Other.Mounts
            };

            var text = Math.Round(mounts.CollectedPercentage);
            _image.Mutate(x => x.DrawText(optionsMo, $"{text}%", Color.White));
        }
    }

    private async Task AddJobLevelsAsync(CharacterClassJobOuterDto classJobs)
    {
        _image = await _imageTask.Value;

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(28, FontStyle.Regular);

        foreach (var job in Enum.GetValues<ClassJob>().Where(x => !x.EvolvesIntoJob()))
        {
            // fall back to class in case job not unlocked yet
            // sheet always uses job, not class, so we need this fallback
            var jobForLookup = job.GetClass() ?? job;
            var jobLevel = classJobs.Unlocked.FirstOrDefault(x =>
                x.ClassJob == jobForLookup)?.Level;

            var levelString = jobLevel switch
            {
                null or 0 => "-",
                100 => "1X",
                _ => jobLevel.Value.ToString()
            };

            var options = new RichTextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Origin = CharacterSheetCoordinates.Jobs.Get(job)
            };

            _image.Mutate(x => x.DrawText(options, levelString, Color.White));
        }
    }

    private async Task<bool> AddGrandCompanyAsync(CharacterDto character)
    {
        _image = await _imageTask.Value;

        if (character.GrandCompany == GrandCompany.NoAffiliation)
        {
            return false;
        }

        var crest = await _externalResourceService.GetGrandCompanyCrestAsync(character.GrandCompany);

        if (character.FreeCompany is null)
        {
            // if player not in any free company, use the fc space to show the gc logo and name
            var gcName = character.GrandCompany.GetDisplayName();
            await PrintInTopValueAreaAsync(gcName, crest);
        }
        else
        {
            // if player is in a free company, print gc crest
            _image.Mutate(x => x.DrawImage(crest, CharacterSheetCoordinates.Other.GcBottom, 1));
        }

        return true;
    }

    private async Task PrintInTopValueAreaAsync(string text, Image? crest = null)
    {
        _image = await _imageTask.Value;

        var family = await _externalResourceService.GetFontFamilyAsync(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeGrandCompany, FontStyle.Regular);

        // if no crest, get coordinates without offset
        var coords = crest is null
            ? CharacterSheetCoordinates.Other.TextTop
            : CharacterSheetCoordinates.Other.FcOrGcTop;

        var textOptions = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(coords.X, coords.Y - 2) // arbitrarily move text up 2px, looks better
        };

        _image.Mutate(x => x.DrawText(textOptions, text, Color.White));

        if (crest is null)
        {
            return;
        }

        // get text width and calculate position of crest
        var textWidth = TextMeasurer.MeasureSize(text, textOptions);
        coords.X -= (int)decimal.Divide((int)textWidth.Width, 2) +
                    CharacterSheetValues.DimensionsGcFcCrest + CharacterSheetValues.GcCrestPadding;
        coords.Y -= (int)decimal.Divide(CharacterSheetValues.DimensionsGcFcCrest, 2);

        _image.Mutate(x => x.DrawImage(crest, coords, 1));
    }
}