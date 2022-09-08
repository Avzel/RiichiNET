namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class Jantou: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Jantou; }

    internal Jantou(Tile tile, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(tile, origin, akadora, naki) {}

    internal Jantou(Value value, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(value, origin, akadora, naki) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = (Tile) this._value;
        if (Akadora != null) first.akadora = true;

        Tile second = (Tile) this._value;

        return new List<Tile>() {first, second};
    }
}
