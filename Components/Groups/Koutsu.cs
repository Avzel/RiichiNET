namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class Koutsu: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Koutsu; }

    internal Koutsu(Tile tile, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(tile, origin, akadora, naki) {}

    internal Koutsu(Value value, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(value, origin, akadora, naki) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second, third;
        second = third = (Tile) this._value;

        return new List<Tile>() {first, second, third};
    }
}
