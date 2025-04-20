using System.Text;
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
            public const string CharacterName = "Character name";
            public const string CharacterWorld = "Character's home world.";
            public const string ForceRefresh = "Whether to force a refresh from the Lodestone.";
            public const string Private = "Whether response is visible only to you.";
        }

        public static class Character
        {
            public static class Get
            {
                public const string Description = "Get information about a character.";

                public static string SelectMenu(int total)
                {
                    var sb = new StringBuilder();

                    sb.Append("More than one character found.");

                    if (total > 25)
                    {
                        sb.Append($" Showing first 25 out of {total} results.");
                    }
                    else
                    {
                        sb.Append($" Showing all {total} results.");
                    }

                    return sb.ToString();
                }

                public static string CharacterNotFound(string name, string world)
                {
                    return $"Could not find {name} on {world}.";
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
                            You can force refresh again in {allowedInRelative}, at {allowedInAbsolute}.
                            """;
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

                            Feel free to remove the code from your Lodestone profile.
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

                public static string ClaimInstructionsDescription(string code)
                {
                    return $"""
                            To validate your character, please add the following code to your Lodestone profile: `{code}`.

                            If you already have a bio on your profile, it's enough to add the code to it, you do not have to delete anything.

                            Afterwards, press the button to validate the code.
                            """;
                }
            }

            public static class Unclaim
            {
                public const string Description =
                    "Unclaim your main character. Use this if you want to switch main characters.";

                public const string NoMainCharacterTitle = "No Main Character";

                public const string ConfirmDescription = """
                                                         Are you sure you want to unclaim this character?

                                                         This action cannot be undone, you will have to verify this or another character by adding a code to the Lodestone again.
                                                         """;

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

                public static string ConfirmTitle(string name, string world)
                {
                    return $"Unclaim {name} ({world})";
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
        public const string NotPersisted = """
                                           Data for this interaction has already been cleared. Please start over and run the command again.

                                           This error should never show up for commands run from version 1.3.0 onwards.
                                           """;
    }

    public static class Buttons
    {
        public const string OpenLodestoneProfile = "Lodestone Profile";
        public const string EditLodestoneProfile = "Edit Lodestone Profile";
        public const string ValidateCode = "Validate Code";
        public const string ConfirmUnclaim = "Confirm Unclaim";
        public const string CharacterSheetMetadata = "Sheet Metadata";
        public const string Minions = "Minions";
        public const string Mounts = "Mounts";
    }

    public static class Other
    {
        public const string ServiceUnavailableTitle = "Retrieving data failed";
        public const string ServiceUnavailableDescription = "Profile set to private or Lodestone under maintenance.";
    }
}