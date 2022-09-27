namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

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
        if (this.ContainsTile(tile))
        {
            _hand[tile]--;
        }
        else if (!tile.akadora && this.ContainsTile(~tile))
        {
            _hand[~tile]--;
        }
        else return false;

        if (tile.akadora)
        {
            _hand[~tile] = _hand[tile];
            _hand.Remove(tile);
            tile = ~tile;
        }
        if (_hand[tile] == 0)
        {
            _hand.Remove(tile);
        }
        return true;
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

    internal int Count()
    {
        return Tiles().Count();
    }

    internal IEnumerable<Tile> Tiles()
    {
        return _hand.Keys;
    }

    internal int AgnosticCount(Value value)
    {
        Tile normal = (Tile)value;
        Tile special = ~((Tile)value);
        return (new int[] {this[normal], this[special]}).Max();
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
