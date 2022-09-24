namespace RiichiNET.Core.Components.Groups;

using System.Collections.Generic;

using RiichiNET.Core.Enums;

internal sealed class AnPai: ClosedGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Dokupai; }

    internal AnPai(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, Akadora)};
    }
}
