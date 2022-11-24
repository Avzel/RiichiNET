namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class MinKanDai: OpenMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    public override Naki Naki { get => Naki.DaiMinKan; }

    internal MinKanDai(Value value, Direction called, (bool exists, bool taken) akadora=default)
        => SetKouKanTiles(true, value, called, akadora);

    public override IList<Tile> GetSortedTiles()
        => OrderedTiles.ToList().AsReadOnly();
}
