namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class AnJan: ClosedGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Jantou; }

    internal AnJan(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, true), (Tile)_value};
    }
}
