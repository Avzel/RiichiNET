namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal abstract class ClosedMeld: Meld
{
    public override bool Open { get => false; }

    private protected Value _value;

    internal ClosedMeld(Value value, bool akadora = false)
        => (_value, Akadora) = (value, akadora);

    internal override bool HasYaoChuu()
        => ((Tile)_value).IsYaoChuu();

    internal override bool HasTerminals()
        => ((Tile)_value).IsTerminal();

    internal override bool OnlyHonors()
        => ((Tile)_value).IsHonor();

    internal override bool OnlyGreens()
        => ((Tile)_value).IsGreen();

    internal override bool OnlyDragons()
        => ((Tile)_value).IsDragon();

    internal override bool OnlyWinds()
        => ((Tile)_value).IsWind();

    public override bool Contains(Value value)
        => _value == value;

    internal override Tile this[int i]
    {
        get
        {
            IList<Tile> tiles = GetSortedTiles();
            if (tiles.Count > i) return GetSortedTiles()[i];
            else return (Tile)Value.None;
        }
    }
}
