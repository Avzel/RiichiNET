namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class MinKou: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }
    public override Naki Naki { get => Naki.Pon; }

    internal MinKou(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        SetKouKanTiles(false, value, called, akadora);
    }

    public override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
