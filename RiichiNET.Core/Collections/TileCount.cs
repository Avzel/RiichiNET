namespace RiichiNET.Core.Collections;

using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Util.Collections;

public sealed class TileCount: ObjectCounter<Tile>
{
    internal static readonly int MAX_HAND_SIZE = 14;
    internal static readonly int MIN_HAND_SIZE = 13;

    internal TileCount(TileCount? original = null): base(original) {}

    internal new void Draw(Tile tile)
    {
        if (Length() == MAX_HAND_SIZE) return;

        if (Has(~tile) && !tile.akadora) base.Draw(~tile);
        else if (Has(~tile) && tile.akadora)
        {
            this[tile] = this[~tile] + 1;
            Eliminate(tile);
        }
        else base.Draw(tile);
    }

    internal new bool Discard(Tile tile)
    {
        if (Has(tile)) base.Discard(tile);
        else if (Has(~tile)) base.Discard(~tile);
        else return false;

        if (Has(tile) && tile.akadora)
        {
            this[~tile] = this[tile];
            Eliminate(tile);
        }
        return true;
    }

    internal bool Discard(Meld meld)
    {
        bool success;
        foreach (Tile tile in meld.GetSortedTiles())
        {
            if (!(success = Discard(tile))) return false;
        }
        return true;
    }

    internal bool ContainsValue(Tile tile)
        => Has(tile) || Has(~tile);

    internal int ValueCount(Tile tile)
        => (new int[] {this[tile], this[~tile]}).Max();
    
    private new void Eliminate(Tile tile) {}
    internal new void Clear() {}
}
