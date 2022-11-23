namespace RiichiNET.Core.Components;

using System;

using RiichiNET.Core.Enums;

public record struct Tile : IComparable<Tile>
{
    public Value value { get; init; }
    public bool akadora { get; init; }

    public Tile(Value value, bool akadora = false)
        => (this.value, this.akadora) = (value, akadora);

    public static implicit operator Tile(Value v)
        => new Tile(v, false);

    public static implicit operator Value(Tile t)
        => t.value;

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

    public static Tile operator ~(Tile tile)
        => tile.IsFive() ? tile with { akadora = !tile.akadora } : tile;

    public int CompareTo(Tile other)
        => this.value.CompareTo(other.value);

    internal bool IsTerminal()
        => (int)value % 10 is 1 or 9;

    internal bool IsHonor()
        => (int)value > 60;

    internal bool IsYaoChuu()
        => IsHonor() || IsTerminal();

    internal bool IsGreen()
        => (int)value is 46 or 48 or 82 or (>= 42 and <= 44);

    internal bool IsFive()
        => value is Value.M5 or Value.P5 or Value.S5;

    internal bool CanStartShuntsu()
        =>  (int)value % 10 < 8;

    internal bool CanStartAkadoraShuntsu()
        => (int)value % 10 is >= 3 and <= 5;

    internal Value DoraValue()
    {
        int val = (int)value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }

    internal Value NextSuit()
    {
        if (value > Value.S9) return value;
        if (value > Value.P9) return value - 40;
        else return value + 20;
    }

    internal bool SameValueDifferentSuit(Tile other)
        =>  !this.IsHonor() && 
            !other.IsHonor() && 
            Math.Abs(value - other.value) is 20 or 40;

    internal bool SameSuit(Tile other)
        =>  !this.IsHonor() &&
            !other.IsHonor() &&
            Math.Abs(value - other.value) < 9;
}
