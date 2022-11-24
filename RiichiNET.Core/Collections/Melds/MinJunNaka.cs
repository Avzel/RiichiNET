namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal class MinJunNaka: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    public override Naki Naki { get => Naki.ChiiNaka; }

    internal MinJunNaka(Value value, bool akadora=false)
    {
        CalledIndex = 0;
        OrderedTiles[0] = value;
        OrderedTiles[1] = value - 1;
        OrderedTiles[2] = value + 1;
        SetShuntsuAkadora(akadora);
    }

    public override IList<Tile> GetSortedTiles()
        => (new List<Tile>()
        {
            OrderedTiles[1],
            OrderedTiles[0],
            OrderedTiles[2]

        }).AsReadOnly();
}
