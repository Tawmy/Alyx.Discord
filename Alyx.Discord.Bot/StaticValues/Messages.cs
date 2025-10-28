using DSharpPlus.Commands.Trees;

namespace Alyx.Discord.Bot.StaticValues;

internal static class Messages
{
    private static string CreateCommandDisplayStr(IReadOnlyDictionary<ulong, Command> commands, string command)
    {
        var commandParts = command.Split(' ');

        var entry = commands.Where(x => x.Value.Name.Equals(commandParts[0], StringComparison.OrdinalIgnoreCase))
            .Select(x => (KeyValuePair<ulong, Command>?)x)
            .FirstOrDefault();

        return entry is not null
            ? $"</{command}:{entry.Value.Key}>" // Clickable Discord command
            : $"`/{command}`"; // Command info not found (extension not initialised yet?), fall back to plain string
    }

    public static class Commands
    {
        public static class Parameters
        {
            public const string CharacterName = "Character name.";

            public const string CharacterNameWithCompletion =
                "Character name. Options above show your recent searches.";

            public const string CharacterWorld = "Character's home world.";

            public const string FreeCompanyName = "Free Company name.";

            public const string FreeCompanyNameWithCompletion =
                "Free Company name. Options above show your recent searches.";

            public const string FreeCompanyWorld = "Free Company's home world.";

            public const string Role = "Role(s) to list jobs levels for.";

            public const string ForceRefresh = "Whether to force a refresh from the Lodestone.";
            public const string Private = "Whether response is visible only to you.";
        }

        public static class General
        {
            public static class About
            {
                public const string Description = "Info and stats about Alyx.";

                public const string Squex = """
                                            -# FINAL FANTASY XIV ©2010 Square Enix Co., Ltd.
                                            -# FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.
                                            -# All material used under license.
                                            """;
            }
        }

        public static class Character
        {
            public static class Get
            {
                public const string Description = "Get information about a character.";

                public const string SelectMenuTitle = "More than one character found.";

                public static string SelectMenuFooter(int total)
                {
                    return total > 25
                        ? $"Showing first 25 out of {total} results."
                        : $"{total} results";
                }

                public static string CharacterNotFound(string name, string world)
                {
                    return $"Could not find **{name}** on **{world}**.";
                }

                public static string CharacterNotFoundInCache(string name, string world)
                {
                    return $"""
                            {CharacterNotFound(name, world)}

                            Search is unavailable (is the Lodestone down?), and this character was not searched for before. It cannot be loaded from the cache.
                            """;
                }
            }

            public static class Me
            {
                public const string Description = "Get information about your character.";

                public const string NotFoundTitle = "Main Character Not Found";

                public static string NotFoundDescription(IReadOnlyDictionary<ulong, Command> commands, string command)
                {
                    return
                        $"You have not claimed a character yet. You can do so using {CreateCommandDisplayStr(commands, command)}.";
                }

                public static string ForceRefreshErrorDescription(string lastRefresh, string allowedInRelative,
                    string allowedInAbsolute)
                {
                    return $"""
                            Last force refresh was {lastRefresh}.
                            You can force refresh again {allowedInRelative}, at {allowedInAbsolute}.
                            """;
                }
            }

            public static class Gear
            {
                public static class GearGet
                {
                    public const string Description = "Get information about a character's gear.";
                }

                public static class GearMe
                {
                    public const string Description = "Get information about your character's gear.";
                }
            }

            public static class Attributes
            {
                public static class AttributesGet
                {
                    public const string Description = "Get a character's attributes.";
                }

                public static class AttributesMe
                {
                    public const string Description = "Get your character's attributes.";
                }
            }

            public static class Jobs
            {
                public static class JobsGet
                {
                    public const string Description = "Get a character's jobs.";
                }

                public static class JobsMe
                {
                    public const string Description = "Get your character's jobs.";
                }
            }

            public static class Claim
            {
                public const string Description = "Claim a character as your main character.";

                public const string ClaimedBySomeoneElse = "This character has already been claimed by someone else.";

                public const string ConfirmedTitle = "Claim Confirmed";

                public const string ClaimInstructionsTitle = "Claim Character";

                public const string CodeNotFound =
                    "Code was not found. Please make sure you have added the code to the correct character on the Lodestone.";

                public const string ClaimInstructionsDescription =
                    "To validate your character, please follow these steps:";

                public const string ClaimInstructionsPart1Subtext =
                    "If you already have a bio on your profile, it's enough to add the code to it, you do not have to delete anything.";

                public const string ClaimInstructionsPart2 = "Added the code?";

                public static string AlreadyClaimed(IReadOnlyDictionary<ulong, Command> commands, string command)
                {
                    return $"""
                            You've already claimed this character.
                            To unclaim this character, use {CreateCommandDisplayStr(commands, command)}.
                            """;
                }

                public static string ConfirmedDescription(IReadOnlyDictionary<ulong, Command> commands, string command)
                {
                    return $"""
                            You can now request your character sheet using {CreateCommandDisplayStr(commands, command)}.

                            -# Feel free to remove the code from your Lodestone profile.
                            """;
                }

                public static string AlreadyClaimedDifferent(IReadOnlyDictionary<ulong, Command> commands,
                    string command)
                {
                    return
                        $"""
                         You've already claimed a different character.
                         To unclaim your main character, use {CreateCommandDisplayStr(commands, command)}.
                         """;
                }

                public static string MultipleResults(string name, string world)
                {
                    return $"Multiple results found for {name} on {world}. Please enter an exact name.";
                }

                public static string ClaimInstructionsPart1(string code)
                {
                    return $"Add this code to your Lodestone profile: `{code}`";
                }
            }

            public static class Unclaim
            {
                public const string Title = "Unclaim Character";

                public const string Description =
                    "Unclaim your main character. Use this if you want to switch main characters.";

                public const string NoMainCharacterTitle = "No Main Character";

                public const string ConfirmDescription =
                    "This action cannot be undone. You will have to verify this or another character by adding a code to the Lodestone again.";

                public const string ButtonDescription = "Are you sure you want to unclaim this character?";

                public const string SuccessTitle = "Main Character Unclaimed";

                public static string SuccessDescription(IReadOnlyDictionary<ulong, Command> commands, string command)
                {
                    return $"""
                            You've unclaimed your main chracter.
                            You can claim a different one using {CreateCommandDisplayStr(commands, command)}.
                            """;
                }

                public static string NoMainCharacterDescription(IReadOnlyDictionary<ulong, Command> commands,
                    string command)
                {
                    return
                        $"""
                         You do not have a main character to unclaim.
                         You can claim one using {CreateCommandDisplayStr(commands, command)}.
                         """;
                }
            }
        }

        public static class FreeCompany
        {
            public static class Get
            {
                public const string Description = "Get information about a free company.";

                public static string FreeCompanyNotFound(string name, string world)
                {
                    return $"Could not find **{name}** on **{world}**.";
                }

                public static string FreeCompanyNotFoundInCache(string name, string world)
                {
                    return $"""
                            {FreeCompanyNotFound(name, world)}

                            Search is unavailable (is the Lodestone down?), and this free company was not searched for before. It cannot be loaded from the cache.
                            """;
                }
            }

            public static class Me
            {
                public const string Description = "Get information about your free company.";

                public const string MainCharacterNotInFreeCompanyTitle = "No Free Company";

                public static string MainCharacterNotInFreeCompanyDescription(string name, string world)
                {
                    return $"Your main character **{name}** ({world}) is not a member of a free company.";
                }

                public static string ForceRefreshErrorDescription(string lastRefresh, string allowedInRelative,
                    string allowedInAbsolute)
                {
                    return $"""
                            Last force refresh was {lastRefresh}.
                            You can force refresh again {allowedInRelative}, at {allowedInAbsolute}.
                            """;
                }
            }
        }

        public static class Ffxiv
        {
            public static class Copypasta
            {
                public const string Description = "You know what this is.";
            }
        }
    }

    public static class UserContextMenus
    {
        public static class CharacterSheet
        {
            public const string NotFoundTitle = "No Claimed Character";

            public static string NotFoundDescription(IReadOnlyDictionary<ulong, Command> commands, string command1,
                string command2)
            {
                return $"""
                        This user has not claimed their character yet. Tell them to do so using {CreateCommandDisplayStr(commands, command1)}!

                        And if they don't have a character at all... there's always {CreateCommandDisplayStr(commands, command2)}.
                        """;
            }
        }
    }

    public static class Events
    {
        public static class SheetMetadata
        {
            public const string Title = "Sheet Metadata";

            public const string Description =
                "A character sheet consists of data from different parts of the Lodestone. These parts are cached to ease the burden on the Lodestone. Here you can see how long ago each was updated.";
        }
    }

    public static class InteractionData
    {
        public const string NotPersisted =
            """
            Data for this interaction has already been cleared. Please start over and run the command again.

            This error should never show up for commands run from version 1.3.0 onwards.
            """;

        public static string CachedFromSheet(string item, bool plural = false)
        {
            return
                $"{item} {(plural ? "are" : "is")} from character sheet. {(plural ? "They" : "It")} might be outdated.";
        }
    }

    public static class Buttons
    {
        public const string OpenLodestoneProfile = "Lodestone Profile";
        public const string EditLodestoneProfile = "Open Lodestone";
        public const string ValidateCode = "Validate Code";
        public const string ConfirmUnclaim = "Confirm Unclaim";
        public const string CharacterSheetMore = "More Details";
        public const string CharacterSheetMetadata = "Sheet Metadata";
        public const string Gear = "Gear";
        public const string Attributes = "Attributes";
        public const string ClassJobsTanksHealers = "Tanks & Healers";
        public const string ClassJobsDpsMelee = "Melee DPS";
        public const string ClassJobsDpsRanged = "Ranged DPS (Physical & Magical)";
        public const string ClassJobsDiscipleHand = "Disciples of the Hand";
        public const string ClassJobsDiscipleLand = "Disciples of the Land";
        public const string Minions = "Minions";
        public const string Mounts = "Mounts";
        public const string FreeCompany = "Free Company";
        public const string FreeCompanyMembers = "Members";
        public const string CurrentGear = "Show current gear";
        public const string CurrentAttributes = "Show current attributes";
        public const string CurrentJobs = "Show current jobs";
        public const string CurrentFreeCompany = "Show current Free Company";
    }

    public static class Other
    {
        public const string NetStoneApiServerErrorTitle = "Parsing failed";

        public const string NetStoneApiServerErrorDescription =
            "The parser failed unexpectedly. Please try again later.";

        public const string ServiceUnavailableTitle = "Retrieving data failed";
        public const string ServiceUnavailableDescription = "Profile set to private or Lodestone under maintenance.";
        public const string RefreshFailed = "Refreshing Data Failed";
        public const string RefreshFailedDescription = "Refreshing data from the Lodestone failed.";
    }

    public static class Xiv
    {
        public static class Attributes
        {
            public const string Same = "Attributes";
            public const string OffensiveProperties = "Offensive Properties";
            public const string DefensiveProperties = "Defensive Properties";
            public const string PhysicalProperties = "Physical Properties";
            public const string Crafting = "Crafting";
            public const string Gathering = "Gathering";
            public const string MentalProperties = "Mental Properties";
            public const string Role = "Role";
            public const string Gear = "Gear";
            public const string AverageItemLevel = "Average Item Level";
        }
    }

    public static class FileNames
    {
        public const string Crest = "crest";
        public const string Copypasta = "haveyouheard";
    }
}