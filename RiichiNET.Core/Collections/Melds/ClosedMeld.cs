namespace RiichiNET.Core.Collections.Melds;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal abstract class ClosedMeld: Meld
{
    internal override bool Open { get => false; }

    private protected Value _value;

    internal ClosedMeld(Value value, bool akadora=false)
    {
        _value = value;
        Akadora = akadora;
    }

    internal override bool HasYaoChuu()
    {
        return ((Tile)_value).IsYaoChuu();
    }

    internal override bool OnlyHonors()
    {
        return ((Tile)_value).IsHonor();
    }

    internal override bool OnlyGreens()
    {
        return ((Tile)_value).IsGreen();
    }

    internal override bool Contains(Value value)
    {
        return _value == value;
    }
}
