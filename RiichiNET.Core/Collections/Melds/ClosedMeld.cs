namespace RiichiNET.Core.Collections.Melds;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal abstract class ClosedMeld: Meld
{
    internal override bool Open { get => false; }

    private protected Value _value;

    internal ClosedMeld(Value value, bool akadora = false)
        => (_value, Akadora) = (value, akadora);

    internal override bool HasYaoChuu()
        => ((Tile)_value).IsYaoChuu();

    internal override bool HasTerminals()
        => ((Tile)_value).IsTerminal();

    internal override bool OnlyHonors()
        => ((Tile)_value).IsHonor();

    internal override bool OnlyGreens()
        => ((Tile)_value).IsGreen();

    internal override bool OnlyDragons()
        => ((Tile)_value).IsDragon();

    internal override bool OnlyWinds()
        => ((Tile)_value).IsWind();

    internal override bool Contains(Value value)
        => _value == value;
}
