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

    private Dictionary<Naki, HashSet<Value>> _callableValues = new Dictionary<Naki, HashSet<Value>>()
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

    internal bool CanCall(Naki naki, Value value=Value.None)
    {
        if (value != Value.None) return _callableValues[naki].Contains(value);
        else return _callableValues[naki].Any();
    }

    internal bool CanCallOnDraw()
    {
        return
            CanCall(Naki.ShouMinKan)
            || CanCall(Naki.AnKan)
            || CanCall(Naki.Riichi)
            || CanCall(Naki.Agari);
    }

    internal bool CanCallOnDiscard(Value value)
    {
        return
            CanCall(Naki.ChiiShimo, value)
            || CanCall(Naki.ChiiNaka, value)
            || CanCall(Naki.ChiiKami, value)
            || CanCall(Naki.Pon, value)
            || CanCall(Naki.DaiMinKan, value)
            || CanCall(Naki.Agari, value);
    }

    private void DetermineCallOnDraw()
    {
        CalculateShanten();

        foreach (Tile tile in Hand.Keys)
        {
            if (Hand[tile] == 4) _callableValues[Naki.AnKan]?.Add(tile.value);
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

            if (Hand[tile] is 2 or 3)
            {
                _callableValues[Naki.Pon]?.Add(value);
            }

            if (Hand[tile] == 3)
            {
                _callableValues[Naki.DaiMinKan]?.Add(value);
            }

            if (!tile.IsYaoChuu() && Hand.ContainsKey(tile + 1))
            {
                _callableValues[Naki.ChiiShimo]?.Add((tile - 1).value);
            }

            if (Hand.ContainsKey(tile + 2))
            {
                _callableValues[Naki.ChiiNaka]?.Add((tile + 1).value);
            }

            if (!tile.IsYaoChuu() && Hand.ContainsKey(tile - 1))
            {
                _callableValues[Naki.ChiiKami]?.Add((tile + 1).value);
            }
        }
    }

    private void DetermineFuriten()
    {
        HashSet<Value>? winningTiles = _callableValues.GetValueOrDefault(Naki.Agari);

        if (winningTiles != null) foreach (Value value in winningTiles)
        {
            if (GraveyardContents.Contains(value)) Furiten = true; return;
        }

        Furiten = false; return;
    }

    internal void Discard(Tile tile)
    {
        if (!(Hand.ContainsKey(tile) || Hand.ContainsKey(~tile))) return;

        int count;
        if (tile.akadora && Hand.ContainsKey(tile))
        {
            count = Hand[tile] - 1;
            Hand.Remove(tile);
            if (count > 0) Hand[~tile] = count;
        }
        else if (!tile.akadora && Hand.ContainsKey(tile))
        {
            if (Hand[tile] == 1) Hand.Remove(tile);
            else Hand[tile]--;
        }
        else if (!tile.akadora && Hand.ContainsKey(~tile))
        {
            if (Hand[tile] == 1) return;
            else Hand[~tile]--;
        }
        else return;

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

        Value value = group.GetSortedTiles()[0].value;

        JustCalled = value;

        if (group.Open && group.Mentsu == Mentsu.Koutsu)
        {
            _callableValues[Naki.ShouMinKan]?.Add(value);
        }
    }

    internal void DeclareRiichi(Tile tile)
    {
        _riichiTile = Graveyard.Count - 1;
    }

    internal void NextRound()
    {
        Wind = Wind.Next<Wind>();
        Score += ScoreChange;
        ScoreChange = 0;
        Hand.Clear();
        Melds.Clear();
        Graveyard.Clear();
        GraveyardContents.Clear();
        _riichiTile = null;
        JustCalled = Value.None;
        Tenpai = false;
        Furiten = false;

        foreach (HashSet<Value> set in _callableValues.Values) set.Clear();
        foreach (List<Group> list in WinningHand.Values) list.Clear();
    }

    // Shanten Calculation:

    internal bool CanShuntsuAkadora(Tile tile)
    {
        return tile.akadora || Hand.ContainsKey(~(tile + 1)) || Hand.ContainsKey(~(tile + 2));
    }

    private int HandCount(Value value)
    {
        Tile normal = (Tile)value;
        Tile special = ~((Tile)value);

        if (Hand.ContainsKey(normal)) return Hand[normal];
        else if (Hand.ContainsKey(special)) return Hand[special];
        else return 0;
    }

    private int ShuntsuCount(Tile tile)
    {
        int firstCount = HandCount(tile.value);
        int secondCount = HandCount(tile.value.Next());
        int thirdCount = HandCount(tile.value.Next().Next());

        if (!tile.IsHonor() && 
            tile.CanStartShuntsu() && 
            secondCount > 0 && 
            thirdCount > 0)
        {
            return (new int[] {firstCount, secondCount, thirdCount}).Min();
        }
        else return 0;
    }

    private void CalculateShanten()
    {
        List<Group> pairs = new List<Group>();
        List<Group> triples = new List<Group>();

        foreach (Tile tile in Hand.Keys)
        {
            Value value = tile.value;
            bool akadora = tile.akadora;

            if (Hand[tile] >= 2) pairs.Add(new AnJan(value, akadora));
            if (Hand[tile] >= 3) triples.Add(new AnKou(value, akadora));
            if (Hand[tile] == 4) pairs.Add(new AnJan(value));

            for (int i = 0; i < ShuntsuCount(tile); i++)
            {
                triples.Add(new AnJun(value, i == 0 ? CanShuntsuAkadora(tile) : false));
            }
        }
        // TODO: take combinations of 1 pair and 4 triples, look for 7 pairs, or look for 14 singles

        
    }
}
