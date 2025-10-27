namespace Alyx.Discord.Core.Records;

internal readonly record struct NameProperties
{
    public readonly FontProperties Name;
    public readonly FontProperties? Title;

    public NameProperties(string? characterTitle)
    {
        const int x = 854;

        if (string.IsNullOrEmpty(characterTitle))
        {
            Name = new FontProperties(x, 100, 46);
        }
        else
        {
            Name = new FontProperties(x, 120, 46);
            Title = new FontProperties(x, 77, 30);
        }
    }
}

internal readonly record struct FontProperties(int X, int Y, int Size);