namespace RiichiNET.Core.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;
using RiichiNET.Util.Extensions;

internal sealed class Mountain
{
    static readonly int DORA_FIRST = 5;
    static readonly int DEAD_WALL_LAST = 13;
    static readonly int EXTRA_TILES_FIRST = 14;
    static readonly int EXTRA_TILES_LAST = 18;

    static readonly Random Rand = new Random();

    private readonly LinkedList<Tile> _wall = new LinkedList<Tile>();
    private readonly Tile[] _deadWall = new Tile[18];

    internal Dictionary<Value, int> DoraList { get; } = new Dictionary<Value, int>();
    internal Dictionary<Value, int> UraDoraList { get; } = new Dictionary<Value, int>();

    internal Mountain()
    {
        Reset();
    }

    internal bool IsEmpty()
    {
        return !_wall.Any();
    }

    internal void Reset()
    {
        CreateTiles();
        SplitDeadWall();
        FlipDora();
    }

    private void CreateTiles()
    {
        if (_wall.Any()) _wall.Clear();

        Array vals = Enum.GetValues(typeof(Value));

        int m5 = Rand.Next(4);
        int p5 = Rand.Next(4);
        int s5 = Rand.Next(4);

        for (int i = 0; i < 4; i++)
        {
            vals.Shuffle(Rand);

            foreach (int j in vals)
            {
                Value val = (Value) Enum.ToObject(typeof(Value), j);
                Tile tile = new Tile(val);

                if ((tile.value == Value.M5 && i == m5) ||
                    (tile.value == Value.P5 && i == p5) ||
                    (tile.value == Value.S5 && i == s5))
                {
                    tile.akadora = true;
                }

                _wall.AddFirst(tile);
            }
        }
    }

    private void SplitDeadWall()
    {
        for (int i = DEAD_WALL_LAST; i >= 0; i--)
        {
            _deadWall[i] = _wall.Last();
            _wall.RemoveLast();
        }

        for (int i = EXTRA_TILES_FIRST; i < EXTRA_TILES_LAST; i++)
        {
            _deadWall[i] = new Tile(Value.None);
        }

        if (DoraList.Any()) DoraList.Clear();
        if (UraDoraList.Any()) UraDoraList.Clear();
    }

    internal void FlipDora()
    {
        int kanCount = DoraList.Values.Sum();
        int index = DORA_FIRST + (kanCount * 2);

        Value dora = _deadWall[index].DoraValue();
        Value uraDora = _deadWall[index - 1].DoraValue();

        DoraList.Add(dora, DoraList.GetValueOrDefault(dora) + 1);
        UraDoraList.Add(uraDora, UraDoraList.GetValueOrDefault(uraDora) + 1);

        if (kanCount > 0)
        {
            _deadWall[DEAD_WALL_LAST + kanCount] = _wall.First();
            _wall.RemoveFirst();
        }
    }

    internal Tile Draw()
    {
        if (_wall.Any())
        {
            Tile tile = _wall.Last();
            _wall.RemoveLast();
            return tile;
        }
        else return new Tile(Value.None);
    }

    internal Tile Rinshan()
    {
        int index = DoraList.Values.Sum();
        Tile tile = _deadWall[index];

        if (tile.value != Value.None)
        {
            _deadWall[index] = new Tile(Value.None);
        }

        return tile;
    }
}
