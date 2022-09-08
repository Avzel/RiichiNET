namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class AnKou: ClosedGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }

    internal AnKou(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        return new List<Tile>() {new Tile(_value, true), (Tile)_value, (Tile)_value};
    }
}
