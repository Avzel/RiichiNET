namespace RiichiNET.Components.Groups;

using Enums;

internal class Koutsu: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }

    internal Koutsu(Tile tile, bool open=false)
    : base(tile, open) {}

    internal Koutsu(Value value, bool akadora=false, bool open=false)
    : base(value, akadora, open) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second, third;
        second = third = (Tile) this._value;

        return new List<Tile>() {first, second, third};
    }
}
