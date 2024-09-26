using System.Text;

namespace Alyx.Discord.Bot.StaticValues;

internal static class Messages
{
    public static class Commands
    {
        public static class Parameters
        {
            public const string CharacterName = "Character name";
            public const string CharacterWorld = "Character's home world.";
            public const string Private = "Whether response is private.";
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

                public const string NotFoundDescription =
                    "You have not claimed a character yet. You can do so using `/character claim`.";
            }

            public static class Claim
            {
                public const string Description = "Claim a character as your main character.";

                public const string AlreadyClaimed =
                    "You've already claimed this character. To unclaim this character, use `/character unclaim`.";

                public const string ClaimedBySomeoneElse = "This character has already been claimed by someone else.";

                public const string ConfirmedTitle = "Claim Confirmed";

                public const string ConfirmedDescription =
                    """
                    You can now request your character sheet using `/character me`.

                    Feel free to remove the code from your Lodestone profile.
                    """;

                public const string AlreadyClaimedDifferent =
                    "You've already claimed a different character. To unclaim your main character, use `/character unclaim`.";

                public const string ClaimInstructionsTitle = "Claim Character";

                public const string CodeNotFound =
                    "Code was not found. Please make sure you have added the code to the correct character on the Lodestone.";

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

                public const string NoMainCharacterDescription =
                    "You do not have a main character to unclaim. You can claim one using `/character claim`";

                public const string ConfirmDescription = """
                                                         Are you sure you want to unclaim this character?

                                                         This action cannot be undone, you will have to verify this or another character by adding a code to the Lodestone again.
                                                         """;

                public const string SuccessTitle = "Main Character Unclaimed";

                public const string SuccessDescription =
                    "You've unclaimed your main chracter. You can claim a different one using `/character claim`.";

                public static string ConfirmTitle(string name, string world)
                {
                    return $"Unclaim {name} ({world})";
                }
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

    public static class DataPersistence
    {
        public const string NotPersisted =
            "Data for this interaction has already been cleared. Please start over and run the command again.";
    }

    public static class Buttons
    {
        public const string OpenLodestoneProfile = "Open Lodestone Profile";
        public const string EditLodestoneProfile = "Edit Lodestone Profile";
        public const string ValidateCode = "Validate Code";
        public const string ConfirmUnclaim = "Confirm Unclaim";
        public const string CharacterSheetMetadata = "Show Sheet Metadata";
    }
}