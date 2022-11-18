namespace RiichiNET.Core.Components;

using System;

using RiichiNET.Core.Enums;

public struct Tile : IComparable<Tile>
{
    public Value value;
    public bool akadora;

    public Tile(Value value, bool akadora=false)
    {
        this.value = value;
        this.akadora = akadora;
    }

    public static implicit operator Tile(Value v)
    {
        return new Tile()
        {
            value = v,
            akadora = false
        };
    }

    public static implicit operator Value(Tile t)
    {
        return t.value;
    }

    public static implicit operator Tile(int n)
    {
        try
        {
            if (n < 0 && n % 5 == 0) return new Tile()
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

    public static Tile operator+ (Tile tile, int i)
    {
        int val = (int) tile.value + i;

        if (i > 8 || !Enum.IsDefined(typeof(Value), val)) return (Tile) Value.None;
        
        else return new Tile((Value)Enum.ToObject(typeof(Value), val));
    }

    public static Tile operator- (Tile tile, int i)
    {
        int val = (int) tile.value - i;

        if (i > 8 || !Enum.IsDefined(typeof(Value), val)) return (Tile) Value.None;
        
        else return new Tile((Value)Enum.ToObject(typeof(Value), val));
    }

    public static Tile operator~ (Tile tile)
    {
        if (tile.IsFive()) return tile;

        if (tile.akadora) tile.akadora = false;
        else tile.akadora = true;

        return tile;
    }

    public int CompareTo(Tile other)
    {
        return this.value.CompareTo(other.value);
    }

    internal bool IsTerminal()
    {
        int val = (int)value;

        return val % 10 is 1 or 9;
    }

    internal bool IsHonor()
    {
        return (int)value > 60;
    }

    internal bool IsYaoChuu()
    {
        return IsHonor() || IsTerminal();
    }

    internal bool IsGreen()
    {
        int val = (int)value;

        return val is 46 or 48 or 82 or (>= 42 and <= 44);
    }

    internal bool IsFive()
    {
        return value is Value.M5 or Value.P5 or Value.S5;
    }

    internal bool CanStartShuntsu()
    {
        return (int)value % 10 < 8;
    }

    internal bool CanStartAkadoraShuntsu()
    {
        int val = (int)value;

        return val % 10 is >= 3 and <= 5;
    }

    internal Value DoraValue()
    {
        int val = (int)value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }
}
