namespace RiichiNET.Core.Components.Groups;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal class MinJunKami: OpenGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    internal override Naki Naki { get => Naki.ChiiKami; }

    internal MinJunKami(Value value, Direction called, bool akadora=false)
    {
        CalledIndex = 0;
        OrderedTiles[0] = (Tile)value;
        OrderedTiles[1] = (Tile)value - 2;
        OrderedTiles[2] = (Tile)value - 1;
        SetShuntsuAkadora(akadora);
    }

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>()
        {
            OrderedTiles[1],
            OrderedTiles[2],
            OrderedTiles[0]
        };
    }
}
