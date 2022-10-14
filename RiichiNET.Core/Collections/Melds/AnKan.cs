namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class AnKan: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }

    internal AnKan(Value value, bool akadora=false): base(value, akadora) {}

    public override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, Akadora), (Tile)_value, (Tile)_value, (Tile)_value};
    }
}
