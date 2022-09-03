namespace RiichiNET.Components;

using Enums;

public struct Tile : IComparable<Tile>
{
    public Value Value { get; }
    public bool Akadora { get; internal set; } = false;

    public Tile(Value val)
    {
        this.Value = val;
    }

    public int CompareTo(Tile other)
    {
        return this.Value.CompareTo(other.Value);
    }
}

public static class TileExtensions
{
    public static bool IsTerminal(this Tile tile)
    {
        int val = (int) tile.Value;

        return val % 10 == 1 || val % 10 == 9;
    }

    public static bool IsHonor(this Tile tile)
    {
        return (int) tile.Value > 60;
    }

    public static bool IsYaoChuu(this Tile tile)
    {
        return tile.IsHonor() && tile.IsTerminal();
    }

    public static Value DoraValue(this Tile tile)
    {
        int val = (int) tile.Value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }
}
