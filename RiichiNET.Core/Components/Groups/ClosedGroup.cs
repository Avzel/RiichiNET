namespace RiichiNET.Core.Components.Groups;

using RiichiNET.Core.Enums;

internal abstract class ClosedGroup: Group
{
    internal override bool Open { get => false; }

    private protected Value _value;

    internal ClosedGroup(Value value, bool akadora=false)
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
}
