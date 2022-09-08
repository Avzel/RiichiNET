namespace RiichiNET.Components.Groups;

using Enums;

internal sealed class Shuntsu: Group
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }

    internal Shuntsu(Tile tile, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(tile, origin, akadora, naki) {}

    internal Shuntsu(Value value, Seat origin, bool akadora=false, Naki naki=Naki.None)
    : base(value, origin, akadora, naki) {}

    internal override List<Tile> GetTiles()
    {
        Tile first = _value;
        Tile second = _value + 1;
        Tile third = _value + 2;

        if (Akadora != null)
        {
            if ((int)first.value % 5 == 0) first.akadora = true;
            else if ((int)second.value % 5 == 0) second.akadora = true;
            else if ((int)third.value % 5 == 0) third.akadora = true;
        }

        return new List<Tile>() {};
    }

    internal override bool HasYaoChuu()
    {
        return base.HasYaoChuu() && (((Tile)this._value) + 2).IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return false;
    }
}
