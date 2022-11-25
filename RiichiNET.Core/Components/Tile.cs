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

    internal Value DoraValue()
    {
        int val = (int)value;

        if (val == 64) val = 61;
        else if (val == 83) val = 81;
        else if (val % 10 == 9) val -= 8;
        else val ++;

        return (Value) Enum.ToObject(typeof(Value), val);
    }

    internal Suit GetSuit()
    {
        if ((int)value is > 0 and < 10) return Suit.M;
        else if ((int)value < 40) return Suit.P;
        else if ((int)value < 60) return Suit.S;
        else if ((int)value < 80) return Suit.W;
        else return Suit.D;
    }

    internal Value NextSuit()
    {
        if (GetSuit() is Suit.D or Suit.W) return value;
        if (GetSuit() is Suit.S) return value - 40;
        return value + 20;
    }

    internal int GetNumericalValue()
        => GetSuit() is not Suit.W or Suit.D? (int)value % 10 : 0;

    internal bool IsTerminal()
        => GetNumericalValue() is 1 or 9;

    internal bool IsHonor()
        => GetSuit() is Suit.W or Suit.D;

    internal bool IsYaoChuu()
        => IsHonor() || IsTerminal();

    internal bool IsWind()
        => GetSuit() == Suit.W;

    internal bool IsDragon()
        => GetSuit() == Suit.D;

    internal bool IsGreen()
        => (int)value is 46 or 48 or 82 or (>= 42 and <= 44);

    internal bool IsFive()
        => GetNumericalValue() == 5;

    internal bool CanStartShuntsu()
        => GetNumericalValue() is > 0 and < 8;

    internal bool SameValueDifferentSuit(Tile other)
        => !this.IsHonor() &&
            this.GetNumericalValue() == other.GetNumericalValue() &&
            this.GetSuit() != other.GetSuit();

    internal static bool SameValuesDifferentSuits(Tile first, Tile second, Tile third)
        => first.SameValueDifferentSuit(second) &&
            first.SameValueDifferentSuit(third) &&
            second.SameValueDifferentSuit(third)
            ? true : false;
}
