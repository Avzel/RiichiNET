namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class Kantsu: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }

    internal Kantsu(Tile tile, bool open=false)
    : base(tile, open) {}

    internal Kantsu(Value value, bool akadora=false, bool open=false)
    : base(value, akadora, open) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second, third, fourth;
        second = third = fourth = (Tile) this._value;

        return new List<Tile>() {first, second, third, fourth};
    }
}
