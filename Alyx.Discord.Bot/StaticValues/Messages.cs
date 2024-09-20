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
        }
    }
}