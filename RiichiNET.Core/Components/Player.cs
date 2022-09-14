namespace RiichiNET.Core.Components;

using System;
using System.Linq;
using System.Collections.Generic;

using RiichiNET.Util.Extensions;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Components.Groups;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = 25000;
    internal int ScoreChange { get; set; } = 0;

    internal SortedDictionary<Tile, int> Hand { get; } = new SortedDictionary<Tile, int>();
    internal List<Group> Melds { get; } = new List<Group>();
    internal List<Tile> Graveyard { get; } = new List<Tile>();
    internal HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    private int? _riichiTile = null;

    internal Dictionary<Naki, HashSet<Value>> CallableValues { get; private set; } = new Dictionary<Naki, HashSet<Value>>()
    {
        {Naki.ChiiShimo, new HashSet<Value>()},
        {Naki.ChiiNaka, new HashSet<Value>()},
        {Naki.ChiiKami, new HashSet<Value>()},
        {Naki.Pon, new HashSet<Value>()},
        {Naki.ShouMinKan, new HashSet<Value>()},
        {Naki.DaiMinKan, new HashSet<Value>()},
        {Naki.AnKan, new HashSet<Value>()},
        {Naki.Riichi, new HashSet<Value>()},
        {Naki.Agari, new HashSet<Value>()}
    };
    internal Value JustCalled { get; private set; } = Value.None;

    internal bool Tenpai { get; private set; } = false;
    internal bool Furiten { get; set; } = false;
    internal Dictionary<Mentsu, List<Group>> WinningHand = new Dictionary<Mentsu, List<Group>>()
    {
        {Mentsu.Jantou, new List<Group>()},
        {Mentsu.Shuntsu, new List<Group>()},
        {Mentsu.Koutsu, new List<Group>()},
        {Mentsu.Kantsu, new List<Group>()}
    };

    internal Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int)seat);
    }

    internal int HandLength()
    {
        return Hand.Values.Sum() + (3 * Melds.Count);
    }

    internal bool IsOpen()
    {
        foreach (Group group in Melds)
        {
            if (group.Open) return true;
        }

        return false;
    }

    internal bool IsRiichi()
    {
        return _riichiTile != null;
    }

    internal bool IsDefeated()
    {
        return Score <= 0;
    }

    private void DetermineCallOnDraw()
    {
        CalculateShanten();

        foreach (Tile tile in Hand.Keys)
        {
            if (Hand[tile] == 4) CallableValues[Naki.AnKan]?.Add(tile.value);
        }
    }

    internal void Draw(Tile tile)
    {
        if (Hand.ContainsKey(tile)) Hand[tile]++;

        else if (Hand.ContainsKey(~tile) && tile.akadora)
        {
            int count = Hand[~tile] + 1;
            Hand.Remove(~tile);
            Hand[tile] = count;
        }

        else if (Hand.ContainsKey(~tile) && !tile.akadora)
        {
            Hand[~tile]++;
        }

        else Hand[tile] = 1;

        DetermineCallOnDraw();
    }

    private void DetermineCallOnDiscard()
    {
        CalculateShanten();

        foreach (Tile tile in Hand.Keys)
        {
            Value value = tile.value;

            if (Hand[tile] == 2 || Hand[tile] == 3)
            {
                CallableValues[Naki.Pon]?.Add(value);
            }

            if (Hand[tile] == 3)
            {
                CallableValues[Naki.DaiMinKan]?.Add(value);
            }

            if (!tile.IsYaoChuu() && Hand.ContainsKey(tile + 1))
            {
                CallableValues[Naki.ChiiShimo]?.Add((tile - 1).value);
            }

            if (Hand.ContainsKey(tile + 2))
            {
                CallableValues[Naki.ChiiNaka]?.Add((tile + 1).value);
            }

            if (!tile.IsYaoChuu() && Hand.ContainsKey(tile - 1))
            {
                CallableValues[Naki.ChiiKami]?.Add((tile + 1).value);
            }
        }
    }

    internal void Discard(Tile tile)
    {
        if (!Hand.ContainsKey(tile)) return;
        else if (Hand[tile] == 1) Hand.Remove(tile);
        else Hand[tile]--;

        Graveyard.Add(tile);
        GraveyardContents.Add(tile.value);

        DetermineCallOnDiscard();
        DetermineFuriten();
    }

    private void AddToWinningHand(Mentsu mentsu, Group group)
    {
        WinningHand.GetValueOrDefault(mentsu)?.Add(group);
    }

    internal void AddMeld(Group group)
    {
        Melds.Add(group);
        AddToWinningHand(group.Mentsu, group);

        if (group.Open && group.Mentsu == Mentsu.Koutsu)
        {
            CallableValues[Naki.ShouMinKan]?.Add(group.GetSortedTiles()[0].value);
        }
    }

    internal void DeclareRiichi(Tile tile)
    {
        _riichiTile = Graveyard.Count - 1;
    }

    internal void DetermineFuriten()
    {
        HashSet<Value>? winningTiles = CallableValues.GetValueOrDefault(Naki.Agari);

        if (winningTiles != null) foreach (Value value in winningTiles)
        {
            if (GraveyardContents.Contains(value)) Furiten = true; return;
        }

        Furiten = false; return;
    }

    internal void CalculateShanten()
    {
        // TODO
    }

    internal void NextRound()
    {
        Wind = Wind.Next<Wind>();
        Score += ScoreChange;
        Hand.Clear();
        Melds.Clear();
        Graveyard.Clear();
        GraveyardContents.Clear();
        _riichiTile = null;
        JustCalled = Value.None;
        Tenpai = false;
        Furiten = false;

        foreach (HashSet<Value> set in CallableValues.Values) set.Clear();
        foreach (List<Group> list in WinningHand.Values) list.Clear();
    }
}
