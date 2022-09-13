namespace RiichiNET.Core.Components.Groups;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal sealed class AnKan: ClosedGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }

    internal AnKan(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, true), (Tile)_value, (Tile)_value, (Tile)_value};
    }
}
