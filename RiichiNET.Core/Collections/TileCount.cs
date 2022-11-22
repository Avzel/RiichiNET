namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;

public sealed class TileCount
{
    internal static readonly int MAX_HAND_SIZE = 14;
    internal static readonly int MIN_HAND_SIZE = 13;

    private SortedDictionary<Tile, int> _hand { get; }

    internal TileCount(TileCount? original = default)
    {
        _hand = new SortedDictionary<Tile, int>();
        if (original != null) foreach (Tile tile in original.Tiles())
        {
            this.Draw(tile);
        }
    }

    internal void Draw(Tile tile)
    {
        if (this.Length() == MAX_HAND_SIZE) return;

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

    internal void Clear() => _hand.Clear();

    internal bool ContainsTile(Tile tile) => _hand.ContainsKey(tile);

    internal bool ContainsValue(Tile tile)
        => this.ContainsTile(tile) || this.ContainsTile(~tile);

    internal int AgnosticCount(Tile tile)
        => (new int[] {this[tile], this[~tile]}).Max();

    internal int Length()
        => _hand.Values.Sum();

    internal int Count()
        => Tiles().Count();

    internal Tile First()
        => _hand.Keys.First();

    public IEnumerable<Tile> Tiles()
        => _hand.Keys;

    public int this[Tile tile]
    {
        get
        {
            if (this.ContainsTile(tile)) return _hand[tile];
            else return 0;
        }
    }
}
