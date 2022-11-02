namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

public abstract class Meld
{
    internal abstract Mentsu Mentsu { get; }
    public abstract Naki Naki { get; }
    public abstract bool Open { get; }
    internal bool Akadora { get; private protected set; }

    public abstract IList<Tile> GetSortedTiles();
    internal abstract bool HasYaoChuu();
    internal abstract bool OnlyHonors();
    internal abstract bool OnlyGreens();
    public abstract bool Contains(Value value);

    internal abstract Tile this[int i] { get; }
}
