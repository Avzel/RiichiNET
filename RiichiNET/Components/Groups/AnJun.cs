namespace RiichiNET.Components.Groups;

using RiichiNET.Enums;

internal sealed class AnJun: ClosedGroup
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }

    internal AnJun(Value value, bool akadora=false): base(value, akadora) {}

    internal override List<Tile> GetSortedTiles()
    {
        Tile first = _value;
        Tile second = _value + 1;
        Tile third = _value + 2;

        if (Akadora)
        {
            if ((int)first.value % 5 == 0) first.akadora = true;
            else if ((int)second.value % 5 == 0) second.akadora = true;
            else if ((int)third.value % 5 == 0) third.akadora = true;
        }

        return new List<Tile>() {first, second, third};
    }

    internal override bool HasYaoChuu()
    {
        return base.HasYaoChuu() || ((Tile)_value + 2).IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return false;
    }
}
