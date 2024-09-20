using System.Numerics;
using NetStone.Common.Enums;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.StaticValues;

internal static class CharacterSheetCoordinates
{
    public static class Jobs
    {
        private static int DistHorizontal => 50;
        private static int DistVertical => 100;

        private static int DistCatLine1 => 68;
        private static int DistCatLine2 => 92;
        private static int DistCatLine3 => 136;

        public static Vector2 Get(ClassJob job)
        {
            return job switch
            {
                ClassJob.Paladin => new Vector2(562, 634),
                ClassJob.Warrior => new Vector2(Get(ClassJob.Paladin).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.DarkKnight => new Vector2(Get(ClassJob.Warrior).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.Gunbreaker => new Vector2(Get(ClassJob.DarkKnight).X + DistHorizontal,
                    Get(ClassJob.Paladin).Y),

                ClassJob.WhiteMage => new Vector2(Get(ClassJob.Gunbreaker).X + DistCatLine1, Get(ClassJob.Paladin).Y),
                ClassJob.Scholar => new Vector2(Get(ClassJob.WhiteMage).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.Astrologian => new Vector2(Get(ClassJob.Scholar).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.Sage => new Vector2(Get(ClassJob.Astrologian).X + DistHorizontal, Get(ClassJob.Paladin).Y),

                ClassJob.BlackMage => new Vector2(Get(ClassJob.Sage).X + DistCatLine1, Get(ClassJob.Paladin).Y),
                ClassJob.Summoner => new Vector2(Get(ClassJob.BlackMage).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.RedMage => new Vector2(Get(ClassJob.Summoner).X + DistHorizontal, Get(ClassJob.Paladin).Y),
                ClassJob.Pictomancer => new Vector2(Get(ClassJob.RedMage).X + DistHorizontal, Get(ClassJob.Paladin).Y),

                ClassJob.Monk => new Vector2(Get(ClassJob.Paladin).X, Get(ClassJob.Paladin).Y + DistVertical),
                ClassJob.Dragoon => new Vector2(Get(ClassJob.Monk).X + DistHorizontal, Get(ClassJob.Monk).Y),
                ClassJob.Ninja => new Vector2(Get(ClassJob.Dragoon).X + DistHorizontal, Get(ClassJob.Monk).Y),
                ClassJob.Samurai => new Vector2(Get(ClassJob.Ninja).X + DistHorizontal, Get(ClassJob.Monk).Y),
                ClassJob.Reaper => new Vector2(Get(ClassJob.Samurai).X + DistHorizontal, Get(ClassJob.Monk).Y),
                ClassJob.Viper => new Vector2(Get(ClassJob.Reaper).X + DistHorizontal, Get(ClassJob.Monk).Y),

                ClassJob.Bard => new Vector2(Get(ClassJob.Viper).X + DistCatLine2, Get(ClassJob.Monk).Y),
                ClassJob.Machinist => new Vector2(Get(ClassJob.Bard).X + DistHorizontal, Get(ClassJob.Monk).Y),
                ClassJob.Dancer => new Vector2(Get(ClassJob.Machinist).X + DistHorizontal, Get(ClassJob.Monk).Y),

                ClassJob.BlueMage => new Vector2(Get(ClassJob.Dancer).X + DistCatLine2, Get(ClassJob.Monk).Y),

                ClassJob.Carpenter => new Vector2(Get(ClassJob.Paladin).X, Get(ClassJob.Monk).Y + DistVertical),
                ClassJob.Blacksmith => new Vector2(Get(ClassJob.Carpenter).X + DistHorizontal,
                    Get(ClassJob.Carpenter).Y),
                ClassJob.Armorer => new Vector2(Get(ClassJob.Blacksmith).X + DistHorizontal, Get(ClassJob.Carpenter).Y),
                ClassJob.Goldsmith => new Vector2(Get(ClassJob.Armorer).X + DistHorizontal, Get(ClassJob.Carpenter).Y),
                ClassJob.Leatherworker => new Vector2(Get(ClassJob.Goldsmith).X + DistHorizontal,
                    Get(ClassJob.Carpenter).Y),
                ClassJob.Weaver => new Vector2(Get(ClassJob.Leatherworker).X + DistHorizontal,
                    Get(ClassJob.Carpenter).Y),
                ClassJob.Alchemist => new Vector2(Get(ClassJob.Weaver).X + DistHorizontal, Get(ClassJob.Carpenter).Y),
                ClassJob.Culinarian => new Vector2(Get(ClassJob.Alchemist).X + DistHorizontal,
                    Get(ClassJob.Carpenter).Y),

                ClassJob.Miner => new Vector2(Get(ClassJob.Culinarian).X + DistCatLine3, Get(ClassJob.Carpenter).Y),
                ClassJob.Botanist => new Vector2(Get(ClassJob.Miner).X + DistHorizontal, Get(ClassJob.Carpenter).Y),
                ClassJob.Fisher => new Vector2(Get(ClassJob.Botanist).X + DistHorizontal, Get(ClassJob.Carpenter).Y),
                ClassJob.Gladiator or ClassJob.Pugilist or ClassJob.Marauder or ClassJob.Lancer or ClassJob.Archer
                    or ClassJob.Conjurer or ClassJob.Thaumaturge or ClassJob.Arcanist
                    or ClassJob.Rogue => throw new InvalidOperationException(
                        "Cannot resolve coordinates for classes, only jobs."),
                _ => throw new ArgumentOutOfRangeException(nameof(ClassJob), job, null)
            };
        }
    }

    public static class Other
    {
        public static Point JobIcon => new(214, 19);
        public static Point ActiveJobLevelBackground => new(72, 127);
        public static Point ActiveJobLevelText => new(72, 124);
        public static Point HomeWorld => new(1114, 344);
        public static Point ItemLevel => new(613, 340);
        public static Point Minions => new(772, ItemLevel.Y);
        public static Point Mounts => new(874, ItemLevel.Y);
        public static Point TextTop => new(854, 243);

        public static Point FcOrGcTop => new(TextTop.X + (int)decimal.Divide(
            CharacterSheetValues.DimensionsGcFcCrest + CharacterSheetValues.GcCrestPadding, 2), TextTop.Y);

        public static Point GcBottom => new(540, 320);
        public static Point AttributesPrimary => new(853, 443);
        public static Point AttributesSecondary => new(853, 503);
    }
}