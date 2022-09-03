namespace RiichiNET.Components;

using Enums;

public struct Tile : IComparable<Tile>
{
    public Value value;
    public bool akadora;

    public Tile(Value val)
    {
        this.value = val;
        this.akadora = false;
    }

    public Tile(Value val, bool aka)
    {
        this.value = val;
        this.akadora = aka;
    }

    public static implicit operator Tile(int n)
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

    public int CompareTo(Tile other)
    {
        return this.value.CompareTo(other.value);
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
