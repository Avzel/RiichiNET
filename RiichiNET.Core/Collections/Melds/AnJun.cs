namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class AnJun: ClosedMeld
{
    internal override Mentsu Mentsu { get => Mentsu.Shuntsu; }
    public override Naki Naki { get => Naki.None; }

    internal AnJun(Value value, bool akadora=false): base(value, akadora) {}

    public override IList<Tile> GetSortedTiles()
    {
        Tile first = _value;
        Tile second = _value + 1;
        Tile third = _value + 2;

        if (Akadora)
        {
            if (first.IsFive()) first = first with { akadora = true };
            else if (second.IsFive()) second = second with { akadora = true };
            else if (third.IsFive()) third = third with { akadora = true };
        }

        return new List<Tile>() {first, second, third};
    }

    internal override bool HasYaoChuu()
        => base.HasYaoChuu() || ((Tile)_value + 2).IsYaoChuu();

    internal override bool HasTerminals()
        => base.HasTerminals() || ((Tile)_value + 2).IsTerminal();

    public override bool Contains(Value value)
        => _value - value is <= 0 and >= -2;
}
