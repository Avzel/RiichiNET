namespace RiichiNET.Core.Components.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components.Collections.Melds;

internal sealed class TileCount
{
    internal SortedDictionary<Tile, int> _hand { get; }

    internal TileCount(TileCount? original = default)
    {
        if (original != null) _hand = new SortedDictionary<Tile, int>(original._hand);
        else _hand = new SortedDictionary<Tile, int>();
    }

    internal void Draw(Tile tile)
    {
        if (this.ContainsTile(tile)) _hand[tile]++;

        else if (this.ContainsTile(~tile) && tile.akadora)
        {
            _hand[tile] = _hand[~tile] + 1;
            _hand.Remove(~tile);
        }

        else if (this.ContainsTile(~tile) && !tile.akadora)
        {
            _hand[~tile]++;
        }

        else _hand[tile] = 1;
    }

    internal bool Discard(Tile tile)
    {
        if (!(_hand.ContainsKey(tile) || _hand.ContainsKey(~tile))) return false;

        int count;
        if (tile.akadora && _hand.ContainsKey(tile))
        {
            count = _hand[tile] - 1;
            _hand.Remove(tile);
            if (count > 0) _hand[~tile] = count;
            return true;
        }
        else if (!tile.akadora && _hand.ContainsKey(tile))
        {
            if (_hand[tile] == 1) _hand.Remove(tile);
            else _hand[tile]--;
            return true;
        }
        else if (!tile.akadora && _hand.ContainsKey(~tile))
        {
            if (_hand[~tile] == 1) return false;
            else _hand[~tile]--;
            return true;
        }
        else return false;
    }

    internal bool Discard(Meld meld)
    {
        bool success;
        foreach (Tile tile in meld.GetSortedTiles())
        {
            if (!(success = this.Discard(tile))) return false;
        }
        return true;
    }

    internal void Clear()
    {
        _hand.Clear();
    }

    internal bool ContainsTile(Tile tile)
    {
        return _hand.ContainsKey(tile);
    }

    internal int Length()
    {
        return _hand.Values.Sum();
    }

    internal IEnumerable<Tile> Tiles()
    {
        return _hand.Keys;
    }

    internal int this[Tile tile]
    {
        get
        {
            if (this.ContainsTile(tile)) return _hand[tile];
            else return 0;
        }
    }
}
