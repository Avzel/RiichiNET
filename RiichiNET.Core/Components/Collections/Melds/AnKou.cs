namespace RiichiNET.Core.Components.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal sealed class AnKou: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }

    internal AnKou(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, Akadora), (Tile)_value, (Tile)_value};
    }
}
