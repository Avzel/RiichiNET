namespace RiichiNET.Components;

using Enums;

internal struct Tile : IComparable<Tile>
{
    internal Value value;
    internal bool akadora;

    internal Tile(Value val)
    {
        this.value = val;
        this.akadora = false;
    }

    internal Tile(Value val, bool aka)
    {
        this.value = val;
        this.akadora = aka;
    }

    public static implicit operator Tile(int n)
    {
        try
        {
            if (n < 0) return new Tile()
            { 
                value = (Value) Enum.ToObject(typeof(Value), n * -1),
                akadora = true
            };
            else return new Tile()
            {
                value = (Value) Enum.ToObject(typeof(Value), n),
                akadora = false
            };
        }
        catch (ArgumentException)
        {
            return new Tile(Value.None);
        }
    }

    public int CompareTo(Tile other)
    {
        return this.value.CompareTo(other.value);
    }
}

internal static class TileExtensions
{
    internal static bool IsTerminal(this Tile tile)
    {
        int val = (int) tile.value;

        return val % 10 == 1 || val % 10 == 9;
    }

    internal static bool IsHonor(this Tile tile)
    {
        return (int) tile.value > 60;
    }

    internal static bool IsYaoChuu(this Tile tile)
    {
        return tile.IsHonor() && tile.IsTerminal();
    }

    internal static Value DoraValue(this Tile tile)
    {
        int val = (int) tile.value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }
}
