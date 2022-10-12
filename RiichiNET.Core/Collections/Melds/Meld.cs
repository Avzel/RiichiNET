namespace RiichiNET.Core.Collections.Melds;

using System.Collections.Generic;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal abstract class Meld
{
    internal abstract Mentsu Mentsu { get; }
    internal abstract bool Open { get; }
    internal bool Akadora { get; private protected set; }

    internal abstract List<Tile> GetSortedTiles();
    internal abstract bool HasYaoChuu();
    internal abstract bool OnlyHonors();
    internal abstract bool OnlyGreens();
    internal abstract bool Contains(Value value);

    internal abstract Tile this[int i] { get; }
}
