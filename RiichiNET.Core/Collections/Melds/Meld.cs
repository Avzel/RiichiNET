namespace RiichiNET.Core.Collections.Melds;

using System;
using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

public abstract class Meld: IEquatable<Meld>, IComparable<Meld>
{
    internal abstract Mentsu Mentsu { get; }
    public abstract Naki Naki { get; }
    public abstract bool Open { get; }
    internal bool Akadora { get; private protected set; }

    public abstract IList<Tile> GetSortedTiles();
    internal abstract bool HasYaoChuu();
    internal abstract bool HasTerminals();
    internal abstract bool Honors();
    internal abstract bool OnlyTerminals();
    internal abstract bool OnlyGreens();
    internal abstract bool Dragons();
    internal abstract bool Winds();
    public abstract bool Contains(Value value);

    internal abstract Tile this[int i] { get; }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + this[0].value.GetHashCode();
            hash = hash * 23 + Mentsu.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object? obj)
        => this.Equals(obj as Meld);

    public bool Equals(Meld? other)
        => other != null ? this.GetHashCode() == other.GetHashCode() : false;

    public int CompareTo(Meld? other)
    => other != null ? GetHashCode().CompareTo(other.GetHashCode()) : GetHashCode();
}
