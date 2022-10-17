namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class AnKou: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }
    public override Naki Naki { get => Naki.None; }

    internal AnKou(Value value, bool akadora=false): base(value, akadora) {}

    public override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, Akadora), (Tile)_value, (Tile)_value};
    }
}
