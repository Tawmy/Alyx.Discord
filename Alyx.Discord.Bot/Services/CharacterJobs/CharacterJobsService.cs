using System.Text;
using Alyx.Discord.Bot.ComponentInteractionHandler;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using Alyx.Discord.Core.Requests.Character.GetCharacterClassJobs;
using AspNetCoreExtensions;
using DSharpPlus;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;
using NetStone.Common.Exceptions;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Services.CharacterJobs;

internal class CharacterClassJobsService(
    ISender sender,
    AlyxConfiguration config,
    CachingService cachingService,
    ProgressBarService progressBarService,
    IInteractionDataService interactionDataService)
    : IDiscordContainerServiceCustom<(CharacterDto, CharacterClassJobOuterDto), Role>
{
    public const string Key = "jobs";
    private const short ProgressBarLength = 15;

    public async Task<DiscordContainerComponent> CreateContainerAsync(Role role,
        (CharacterDto, CharacterClassJobOuterDto) entity, CancellationToken cancellationToken = default)
    {
        return new DiscordContainerComponent(await CreateComponentsAsync(role, entity.Item1, entity.Item2, true));
    }

    public async Task<DiscordContainerComponent> CreateContainerAsync(Role role, string lodestoneId,
        bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        var (container, _) = await RetrieveDataAndCreateContainerAsync(role, lodestoneId, forceRefresh,
            cancellationToken);
        return container;
    }

    public async Task<(DiscordContainerComponent, (CharacterDto, CharacterClassJobOuterDto))>
        RetrieveDataAndCreateContainerAsync(
            Role role, string lodestoneId, bool forceRefresh = false,
            CancellationToken cancellationToken = default)
    {
        var maxAgeCharacter = forceRefresh ? 0 : config.NetStone.MaxAgeClassJobs;
        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId, maxAgeCharacter),
            cancellationToken);
        var maxAgeClassJobs = forceRefresh ? 0 : config.NetStone.MaxAgeClassJobs;
        var classJobs = await sender.Send(new CharacterGetCharacterClassJobsRequest(lodestoneId, maxAgeClassJobs),
            cancellationToken);
        var container = new DiscordContainerComponent(await CreateComponentsAsync(role, character, classJobs, false));
        return (container, (character, classJobs));
    }

    private async Task<List<DiscordComponent>> CreateComponentsAsync(Role role, CharacterDto character,
        CharacterClassJobOuterDto classJobs, bool cachedFromSheet)
    {
        List<DiscordComponent> c =
        [
            character.ToSectionComponent(cachingService),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large)
        ];

        c.AddRange(role switch
        {
            Role.TanksHealers =>
            [
                CreateTanksComponent(classJobs),
                new DiscordSeparatorComponent(spacing: DiscordSeparatorSpacing.Large),
                CreateHealersComponent(classJobs)
            ],
            Role.DpsMelee => [CreateDpsMeleeComponent(classJobs)],
            Role.DpsRanged =>
            [
                CreateDpsRangedPhysicalComponent(classJobs),
                new DiscordSeparatorComponent(spacing: DiscordSeparatorSpacing.Large),
                CreateDpsRangedMagicalComponent(classJobs)
            ],
            Role.DiscipleHand => [CreateDiscipleHandComponent(classJobs)],
            Role.DiscipleLand => [CreateDiscipleLandComponent(classJobs)],
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        });

        if (classJobs.LastUpdated is not null)
        {
            c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

            var lastUpdatedStr = $"-# Last updated {Formatter.Timestamp(classJobs.LastUpdated.Value)}";

            var maxAgeClassJobs = TimeSpan.FromMinutes(config.NetStone.MaxAgeClassJobs);
            if (cachedFromSheet && DateTime.Now.Subtract(maxAgeClassJobs) > classJobs.LastUpdated)
            {
                c.Add(new DiscordSectionComponent(
                    new DiscordTextDisplayComponent(
                        $"""
                         -# {Messages.InteractionData.CachedFromSheet("Jobs", true)}
                         {lastUpdatedStr}
                         """),
                    await CreateCharacterClassJobsButtonAsync(character, role)
                ));
            }
            else
            {
                c.Add(new DiscordTextDisplayComponent(lastUpdatedStr));
            }
        }

        if (classJobs.FallbackUsed)
        {
            c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

            c.Add(new DiscordTextDisplayComponent(
                $"""
                 {Messages.Other.RefreshFailed}
                 -# {Messages.Other.RefreshFailedDescription}
                 -# {CreateFallbackMessage(character.FallbackReason)}
                 """
            ));
        }

        return c;
    }

    private DiscordTextDisplayComponent CreateTanksComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Tanks");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Paladin));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Warrior));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.DarkKnight));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Gunbreaker));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateHealersComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Healers");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.WhiteMage));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Scholar));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Astrologian));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Sage));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateDpsMeleeComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Melee DPS");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Monk));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Dragoon));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Ninja));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Samurai));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Reaper));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Viper));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateDpsRangedPhysicalComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Physical Ranged DPS");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Bard));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Machinist));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Dancer));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateDpsRangedMagicalComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Magical Ranged DPS");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.BlackMage));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Summoner));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.RedMage));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Pictomancer));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.BlueMage));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateDiscipleHandComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Disciples of the Hand");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Carpenter));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Blacksmith));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Armorer));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Goldsmith));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Leatherworker));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Weaver));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Alchemist));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Culinarian));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private DiscordTextDisplayComponent CreateDiscipleLandComponent(CharacterClassJobOuterDto classJobs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("## Disciples of the Land");
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Miner));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Botanist));
        sb.AppendLine(CreateClassJobString(classJobs, ClassJob.Fisher));
        return new DiscordTextDisplayComponent(sb.ToString());
    }

    private string CreateClassJobString(CharacterClassJobOuterDto dto, ClassJob classJob)
    {
        var baseClass = classJob.GetClass();
        var unlocked = dto.Unlocked.FirstOrDefault(x => x.ClassJob == classJob);

        if (unlocked is null && baseClass is not null)
        {
            unlocked = dto.Unlocked.FirstOrDefault(x => x.ClassJob == baseClass);
        }

        var emoji = GetDiscordEmoji();
        var jobAbbreviation = GetJobAbbreviation();

        var sb = new StringBuilder();

        if (unlocked is null)
        {
            sb.AppendLine(progressBarService.CreateProgressBar(ProgressBarStyle.ClassJobLevel, ProgressBarLength, 0));
            sb.Append($"-# {emoji} `{jobAbbreviation}  -`");
        }
        else
        {
            var progress = unlocked.ExpMax > 0
                ? (short)decimal.Multiply(decimal.Divide(unlocked.ExpCurrent, unlocked.ExpMax), 100)
                : (short)0; // max level jobs have no exp max, so progress is at 0%
            sb.AppendLine(progressBarService.CreateProgressBar(ProgressBarStyle.ClassJobLevel, ProgressBarLength,
                progress));

            var expCurrent = FormatExp(unlocked.ExpCurrent);
            var expMax = FormatExp(unlocked.ExpMax);
            sb.Append(
                $"-# {emoji} `{jobAbbreviation}  Lv {GetLevelStr(unlocked.Level)}  EXP {expCurrent}/{expMax}`");
        }

        return sb.ToString();

        DiscordEmoji GetDiscordEmoji()
        {
            var emojiName = unlocked?.ClassJob.ToString() ?? baseClass?.ToString() ?? classJob.ToString();
            return cachingService.GetApplicationEmoji(emojiName);
        }

        string GetJobAbbreviation()
        {
            var job = unlocked?.ClassJob ?? baseClass ?? classJob;
            return job.GetShortName();
        }

        static string GetLevelStr(short level)
        {
            return level < 100 ? $"{level} " : level.ToString();
        }

        static string FormatExp(long n)
        {
            return n switch
            {
                < 1000 => n.ToString(),
                < 10000 => $"{n - 5:#,.##}K",
                < 100000 => $"{n - 50:#,.#}K",
                < 1000000 => $"{n - 500:#,.}K",
                < 10000000 => $"{n - 5000:#,,.##}M",
                < 100000000 => $"{n - 50000:#,,.#}M",
                < 1000000000 => $"{n - 500000:#,,.}M",
                _ => $"{n - 5000000:#,,,.##}B"
            };
        }
    }

    private async Task<DiscordButtonComponent> CreateCharacterClassJobsButtonAsync(CharacterDto character, Role role)
    {
        var interactionData = new ClassJobInteractionDataRefresh
        {
            Role = role,
            LodestoneId = character.Id
        };

        var id = await interactionDataService.AddDataAsync(interactionData, ComponentIds.Button.CharacterClassJobs);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, id, Messages.Buttons.CurrentJobs);
    }

    private static string? CreateFallbackMessage(string? fallbackReason)
    {
        if (fallbackReason is null)
        {
            return null;
        }

        return fallbackReason.Equals(nameof(ParsingFailedException), StringComparison.OrdinalIgnoreCase)
            ? Messages.Other.ServiceUnavailableDescription
            : fallbackReason;
    }
}

internal enum Role
{
    [ChoiceDisplayName("Tanks & Healers")] TanksHealers,

    [ChoiceDisplayName("Melee DPS")] DpsMelee,

    [ChoiceDisplayName("Ranged DPS (Physical & Magical)")]
    DpsRanged,

    [ChoiceDisplayName("Disciples of the Hand")]
    DiscipleHand,

    [ChoiceDisplayName("Disciples of the Land")]
    DiscipleLand
}