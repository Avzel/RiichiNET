namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal abstract class ClosedMeld: Meld
{
    public override bool Open { get => false; }

    private protected Value _value;

    internal ClosedMeld(Value value, bool akadora=false)
    {
        _value = value;
        Akadora = akadora;
    }

    internal override bool HasYaoChuu()
    {
        return ((Tile)_value).IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return ((Tile)_value).IsHonor();
    }

    internal override bool OnlyGreens()
    {
        return ((Tile)_value).IsGreen();
    }

    internal override bool OnlyDragons()
    {
        return _value is Value.DG or Value.DR or Value.DW;
    }

    internal override bool OnlyWinds()
    {
        return _value is Value.WE or Value.WN or Value.WS or Value.WW;
    }

    public override bool Contains(Value value)
    {
        return _value == value;
    }

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
