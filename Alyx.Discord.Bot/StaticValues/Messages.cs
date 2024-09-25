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

                public static string ClaimInstructionsDescription(string code)
                {
                    return $"""
                            To validate your character, please add the following code to your Lodestone profile: `{code}`.

                            If you already have a bio on your profile, it's enough to add the code to it, you do not have to delete anything.

                            Afterwards, press the button to validate the code.
                            """;
                }
            }

            public static class Me
            {
                public const string Description = "Get information about your character.";

                public const string NotFoundTitle = "Main Character Not Found";

                public const string NotFoundDescription =
                    "You have not claimed a character yet. You can do so using `/character claim`.";
            }
        }
    }
}