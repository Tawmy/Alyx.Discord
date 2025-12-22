using System.Numerics;
using Alyx.Discord.Core.Enums;
using Alyx.Discord.Core.Extensions;
using Alyx.Discord.Core.Records;
using Alyx.Discord.Core.StaticValues;
using AspNetCoreExtensions;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;
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

    private readonly Image _image;

    public CharacterSheetService(ExternalResourceService externalResourceService, HttpClient httpClient)
    {
        _externalResourceService = externalResourceService;
        _httpClient = httpClient;
        _image = _externalResourceService.GetCharacterSheetImage(CharacterSheetImage.TemplateBase);
    }

    public async Task<Image> CreateCharacterSheetAsync(CharacterDto character,
        CharacterClassJobOuterDto? classJobs = null,
        CollectionDto<CharacterMinionDto>? minions = null,
        CollectionDto<CharacterMountDto>? mounts = null,
        FreeCompanyDto? freeCompany = null)
    {
        await AddCharacterPortraitAsync(character);
        AddPortraitFrame();

        AddJobIcon(character);
        AddJobFrame();
        AddActiveJobLevel(character);

        AddCharacterName(character);
        AddHomeWorld(character);

        AddILvlMinionsMounts(character, minions, mounts);

        if (classJobs is not null)
        {
            AddJobLevels(classJobs);
        }

        var gc = AddGrandCompany(character);
        var fc = await AddFreeCompanyAsync(freeCompany);
        AddAttributes(character);

        if (!gc && !fc)
        {
            PrintInTopValueArea("New Adventurer");
        }

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

    private async Task AddCharacterPortraitAsync(CharacterDto character)
    {
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

    private void AddPortraitFrame()
    {
        var imgFrame = _externalResourceService.GetCharacterSheetImage(CharacterSheetImage.TemplateFrame);
        _image.Mutate(x => x.DrawImage(imgFrame, 1));
    }

    private void AddJobIcon(CharacterDto character)
    {
        var imgJob = _externalResourceService.GetJobIcon(character.ActiveClassJob);
        if (imgJob.Width > 64)
        {
            // icons must be 64x64
            imgJob.Mutate(x => x.Resize(new Size(64, 64)));
        }

        _image.Mutate(x => x.DrawImage(imgJob, CharacterSheetCoordinates.Other.JobIcon, 1));
    }

    private void AddJobFrame()
    {
        var imgJob = _externalResourceService.GetCharacterSheetImage(CharacterSheetImage.TemplateJobCircle);
        _image.Mutate(x => x.DrawImage(imgJob, 1));
    }

    private void AddActiveJobLevel(CharacterDto character)
    {
        var circle = new EllipsePolygon(CharacterSheetCoordinates.Other.ActiveJobLevelBackground,
            CharacterSheetValues.ActiveJobLevelRadius);
        _image.Mutate(x => x.Draw(new Color(CharacterSheetValues.ActiveJobLevelBackground),
            CharacterSheetValues.ActiveJobLevelThickness, circle));

        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.Antonio);
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

    private void AddCharacterName(CharacterDto character)
    {
        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.Vollkorn);
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

        var fontTitle = family.CreateFont(nameProperties.Title.Size, FontStyle.Regular);

        var optionsTitle = new RichTextOptions(fontTitle)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(nameProperties.Title.X, nameProperties.Title.Y)
        };

        // print character title
        _image.Mutate(x => x.DrawText(optionsTitle, character.Title!, Color.Black));
    }

    private void AddHomeWorld(CharacterDto character)
    {
        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeHomeWorld, FontStyle.Regular);

        var options = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = CharacterSheetCoordinates.Other.HomeWorld
        };

        _image.Mutate(x => x.DrawText(options, character.Server, Color.White));
    }

    private void AddILvlMinionsMounts(CharacterDto character, CollectionDto<CharacterMinionDto>? minions,
        CollectionDto<CharacterMountDto>? mounts)
    {
        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeMiMo, FontStyle.Regular);

        var optionsLvl = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Origin = CharacterSheetCoordinates.Other.ItemLevel
        };

        var avgItemLevel = character.Gear.GetAvarageItemLevel();
        _image.Mutate(x => x.DrawText(optionsLvl, avgItemLevel.ToString(), Color.White));

        var optionsMo = new RichTextOptions(optionsLvl)
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            Origin = CharacterSheetCoordinates.Other.Mounts
        };

        var mountsPercentage = Math.Round(mounts?.CollectedPercentage ?? 0);
        _image.Mutate(x => x.DrawText(optionsMo, $"{mountsPercentage}%", Color.White));

        var optionsMi = new RichTextOptions(optionsMo)
        {
            Origin = CharacterSheetCoordinates.Other.Minions
        };

        var minionsPercentage = Math.Round(minions?.CollectedPercentage ?? 0);
        _image.Mutate(x => x.DrawText(optionsMi, $"{minionsPercentage}%", Color.White));
    }

    private void AddJobLevels(CharacterClassJobOuterDto classJobs)
    {
        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.OpenSans);
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
                100 => "X",
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

    private bool AddGrandCompany(CharacterDto character)
    {
        if (character.GrandCompany == GrandCompany.NoAffiliation)
        {
            return false;
        }

        var crest = _externalResourceService.GetGrandCompanyCrest(character.GrandCompany);

        if (character.FreeCompany is null)
        {
            // if player not in any free company, use the fc space to show the gc logo and name
            var gcName = character.GrandCompany.GetDisplayName();
            PrintInTopValueArea(gcName, crest);
        }
        else
        {
            // if player is in a free company, print gc crest
            _image.Mutate(x => x.DrawImage(crest, CharacterSheetCoordinates.Other.GcBottom, 1));
        }

        return true;
    }

    private void PrintInTopValueArea(string text, Image? crest = null)
    {
        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.OpenSans);
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

    private async Task<bool> AddFreeCompanyAsync(FreeCompanyDto? freeCompany)
    {
        if (freeCompany is null)
        {
            return false;
        }

        var crest = await freeCompany.CrestLayers.DownloadCrestAsync(_httpClient);
        crest.Mutate(x => x.Resize(CharacterSheetValues.DimensionsGcFcCrest, CharacterSheetValues.DimensionsGcFcCrest,
            KnownResamplers.Lanczos3));
        var fullName = $"{freeCompany.Name} {freeCompany.Tag}";
        PrintInTopValueArea(fullName, crest);
        return true;
    }

    private void AddAttributes(CharacterDto character)
    {
        var attributes = character.ActiveClassJob.GetDisplayAttributes();

        var family = _externalResourceService.GetFontFamily(CharacterSheetFont.OpenSans);
        var font = family.CreateFont(CharacterSheetValues.FontSizeAttributes, FontStyle.Regular);

        PrintAttributes(character, font, attributes.Take(2), CharacterSheetCoordinates.Other.AttributesPrimary, true);
        PrintAttributes(character, font, attributes.Skip(2), CharacterSheetCoordinates.Other.AttributesSecondary,
            false);
    }

    private void PrintAttributes(CharacterDto character, Font font,
        IEnumerable<CharacterAttribute> attributes, Point origin, bool primary)
    {
        var options = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = origin
        };

        var result = attributes.Select(x =>
            $"{GetAttributeName(x, primary)}:{CharacterSheetValues.AttributeGapSmall}{character.Attributes[x]}");

        var text = string.Join(CharacterSheetValues.AttributeGapBig, result);

        _image.Mutate(x => x.DrawText(options, text, Color.White));
    }

    private static string GetAttributeName(CharacterAttribute a, bool showFullName)
    {
        var name = a.GetDisplayName();
        a.TryGetShortName(out var shortName);
        return showFullName ? name : shortName ?? name;
    }
}