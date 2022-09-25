namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class AnJan: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Jantou; }

    internal AnJan(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, Akadora), (Tile)_value};
    }
}