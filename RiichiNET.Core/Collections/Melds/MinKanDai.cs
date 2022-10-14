namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class MinKanDai: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    public override Naki Naki { get => Naki.DaiMinKan; }

    internal MinKanDai(Value value, Direction called, (bool exists, bool taken) akadora=default)
    {
        SetKouKanTiles(true, value, called, akadora);
    }

    public override List<Tile> GetSortedTiles()
    {
        return new List<Tile>(OrderedTiles);
    }
}
