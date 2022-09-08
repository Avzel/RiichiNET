namespace RiichiNET.Components.Groups;

using Enums;

internal abstract class OpenGroup: Group
{
    internal override bool Open { get => true; }
    internal abstract Naki Naki { get; }

    internal List<Tile> OrderedTiles { get; private protected set; } = new List<Tile>();
    internal int CalledIndex { get; private protected set; }

    internal override bool HasYaoChuu()
    {
        return OrderedTiles[0].IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return OrderedTiles[0].IsHonor();
    }

    internal override bool OnlyGreens()
    {
        return OrderedTiles[0].IsGreen();
    }
}
