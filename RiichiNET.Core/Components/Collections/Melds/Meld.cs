namespace RiichiNET.Core.Components.Collections.Melds;

using System.Collections.Generic;

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

    internal Tile this[int i]
    {
        get
        {
            List<Tile> tiles = GetSortedTiles();
            if (tiles.Count > i) return GetSortedTiles()[i];
            else return (Tile)Value.None;
        }
    }
}
