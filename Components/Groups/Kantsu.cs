namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class Kantsu: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Kantsu; }

    internal Kantsu(Tile tile, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(tile, origin, akadora, naki) {}

    internal Kantsu(Value value, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(value, origin, akadora, naki) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second, third, fourth;
        second = third = fourth = (Tile) this._value;

        return new List<Tile>() {first, second, third, fourth};
    }
}
