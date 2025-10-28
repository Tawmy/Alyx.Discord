using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services.CharacterJobs;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Records;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonCharacterSheetMoreHandler(
    IInteractionDataService interactionDataService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        var sheet = await interactionDataService.GetDataAsync<SheetCache>(dataId, cancellationToken);

        var buttonGear = await CreateGearButtonAsync(interactionDataService, sheet.Character);
        var buttonAttributesId = interactionDataService.CreateDataComponentIdFromExisting(buttonGear.CustomId,
            ComponentIds.Button.CharacterSheetAttributes);
        var buttonAttributes = CreateAttributesButton(buttonAttributesId);
        var buttonsJobs = await CreateClassJobsButtonsAsync(interactionDataService, sheet.Character, sheet.ClassJobs);

        List<DiscordButtonComponent> buttonsLine1 = [buttonGear, buttonAttributes];

        if (sheet.MountsPublic)
        {
            buttonsLine1.Add(CreateLodestoneMountsButton(sheet.Character.Id));
        }

        if (sheet.MinionsPublic)
        {
            buttonsLine1.Add(CreateLodestoneMinionsButton(sheet.Character.Id));
        }

        if (sheet.FreeCompany is { } freeCompany)
        {
            buttonsLine1.Add(await CreateFreeCompanyButtonAsync(interactionDataService, freeCompany));
        }

        var builder = new DiscordFollowupMessageBuilder().EnableV2Components();
        builder.AddActionRowComponent(buttonsLine1).AddActionRowComponent(buttonsJobs);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }

    private static async Task<DiscordButtonComponent> CreateGearButtonAsync(
        IInteractionDataService interactionDataService,
        CharacterDto character)
    {
        var componentId = await interactionDataService.AddDataAsync(character,
            ComponentIds.Button.CharacterSheetGear);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId, Messages.Buttons.Gear);
    }

    private static DiscordButtonComponent CreateAttributesButton(string componentId)
    {
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId, Messages.Buttons.Attributes);
    }

    private static async Task<IEnumerable<DiscordButtonComponent>> CreateClassJobsButtonsAsync(
        IInteractionDataService interactionDataService,
        CharacterDto character,
        CharacterClassJobOuterDto classJobs)
    {
        var buttons = new List<DiscordButtonComponent>();

        var interactionData = new ClassJobInteractionData
        {
            Role = Role.TanksHealers,
            Character = character,
            ClassJobs = classJobs
        };

        var idTanksHealers = await interactionDataService.AddDataAsync(interactionData,
            ComponentIds.Button.CharacterSheetClassJobs);
        buttons.Add(new DiscordButtonComponent(DiscordButtonStyle.Secondary, idTanksHealers,
            Messages.Buttons.ClassJobsTanksHealers));

        var idDpsMelee = await interactionDataService.AddDataAsync(interactionData with { Role = Role.DpsMelee },
            ComponentIds.Button.CharacterSheetClassJobs);
        buttons.Add(new DiscordButtonComponent(DiscordButtonStyle.Secondary, idDpsMelee,
            Messages.Buttons.ClassJobsDpsMelee));

        var idDpsRanged = await interactionDataService.AddDataAsync(interactionData with { Role = Role.DpsRanged },
            ComponentIds.Button.CharacterSheetClassJobs);
        buttons.Add(new DiscordButtonComponent(DiscordButtonStyle.Secondary, idDpsRanged,
            Messages.Buttons.ClassJobsDpsRanged));

        var idDiscipleHand = await interactionDataService.AddDataAsync(
            interactionData with { Role = Role.DiscipleHand }, ComponentIds.Button.CharacterSheetClassJobs);
        buttons.Add(new DiscordButtonComponent(DiscordButtonStyle.Secondary, idDiscipleHand,
            Messages.Buttons.ClassJobsDiscipleHand));

        var idDiscipleLand = await interactionDataService.AddDataAsync(
            interactionData with { Role = Role.DiscipleLand }, ComponentIds.Button.CharacterSheetClassJobs);
        buttons.Add(new DiscordButtonComponent(DiscordButtonStyle.Secondary, idDiscipleLand,
            Messages.Buttons.ClassJobsDiscipleLand));

        return buttons;
    }

    private static DiscordLinkButtonComponent CreateLodestoneMountsButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}/mount";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.Mounts);
    }

    private static DiscordLinkButtonComponent CreateLodestoneMinionsButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}/minion";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.Minions);
    }

    private static async Task<DiscordButtonComponent> CreateFreeCompanyButtonAsync(
        IInteractionDataService interactionDataService,
        FreeCompanyDto freeCompany)
    {
        var componentId = await interactionDataService.AddDataAsync(freeCompany,
            ComponentIds.Button.CharacterSheetFreeCompany);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId, Messages.Buttons.FreeCompany);
    }
}