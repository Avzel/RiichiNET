namespace OpenRiichi.Components;

using Value = Enums.Value;
using Wind = Enums.Wind;
using Meld = Enums.Meld;

public struct Tile
{
    public Value value { get; }
    public Wind wind { get; internal set; } = Wind.None;
    public Meld meld { get; internal set; } = Meld.None;
    public bool akadora { get; internal set; } = false;
    public bool visible { get; internal set; } = false;
    public bool called { get; internal set; } = false;

    public Tile(Value val)
    {
        this.value = val;
    }
}

public static class TileExtensions
{
    public static bool IsTerminal(this Tile tile)
    {
        int val = (int) tile.value;

        return val % 10 == 1 || val % 10 == 9;
    }

    public static bool IsHonor(this Tile tile)
    {
        return (int) tile.value > 60;
    }

    public static bool IsYaoChuu(this Tile tile)
    {
        return tile.IsHonor() && tile.IsTerminal();
    }

    public static Value DoraValue(this Tile tile)
    {
        int val = (int) tile.value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }
}
