namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class AnKan: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }
    public override Naki Naki { get => Naki.AnKan; }

    internal AnKan(Value value, bool akadora=false): base(value, akadora) {}

    internal override IList<Tile> GetSortedTiles()
        =>  (new List<Tile>() {new Tile(_value, Akadora), (Tile)_value, (Tile)_value, (Tile)_value})
            .AsReadOnly();
}
