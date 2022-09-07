namespace RiichiNET.Components.Groups;

using Enums;

internal class Jantou: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Jantou; }

    internal override Naki Naki { get => Naki.None; }
    internal override Seat Origin { get => Seat.None; }
    internal override Naki? Akadora { get => null; }

    internal Jantou(Tile tile, bool open=false)
    : base(tile, open) {}

    internal Jantou(Value value, bool akadora=false, bool open=false)
    : base(value, akadora, open) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second = (Tile) this._value;

        return new List<Tile>() {first, second};
    }
}
