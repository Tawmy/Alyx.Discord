using SixLabors.ImageSharp.PixelFormats;

namespace Alyx.Discord.Core.StaticValues;

public static class CharacterSheetValues
{
    public static Rgba32 ActiveJobLevelBackground => new(0.2f, 0.2f, 0.2f);
    public static int ActiveJobLevelThickness => 40;
    public static int ActiveJobLevelRadius => 10;
    public static int ActiveJobLevelFontSize => 30;
    public static int FontSizeHomeWorld => 28;
    public static int FontSizeMiMo => 32;
}