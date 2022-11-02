namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal class MinJunKami: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    public override Naki Naki { get => Naki.ChiiKami; }

    internal MinJunKami(Value value, bool akadora=false)
    {
        CalledIndex = 0;
        OrderedTiles[0] = (Tile)value;
        OrderedTiles[1] = (Tile)value - 2;
        OrderedTiles[2] = (Tile)value - 1;
        SetShuntsuAkadora(akadora);
    }

    public override IList<Tile> GetSortedTiles()
    {
        return new List<Tile>()
        {
            OrderedTiles[1],
            OrderedTiles[2],
            OrderedTiles[0]
        };
    }
}
