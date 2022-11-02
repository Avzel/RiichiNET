namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal class MinJunShimo: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    public override Naki Naki { get => Naki.ChiiShimo; }

    internal MinJunShimo(Value value, bool akadora=false)
    {
        CalledIndex = 0;
        OrderedTiles[0] = (Tile)value;
        OrderedTiles[1] = (Tile)value + 1;
        OrderedTiles[2] = (Tile)value + 2;
        SetShuntsuAkadora(akadora);
    }

    public override IList<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
