namespace RiichiNET.Core.Components;

using System;
using System.Linq;
using System.Collections.Generic;

using RiichiNET.Util.Extensions;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Components.Collections;
using RiichiNET.Core.Components.Collections.Melds;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = 25000;
    internal int ScoreChange { get; set; } = 0;

    internal ClosedHand Hand { get; } = new ClosedHand();
    internal List<Meld> Melds { get; } = new List<Meld>();
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
    internal Dictionary<Mentsu, List<Meld>> WinningHand = new Dictionary<Mentsu, List<Meld>>()
    {
        {Mentsu.Jantou, new List<Meld>()},
        {Mentsu.Shuntsu, new List<Meld>()},
        {Mentsu.Koutsu, new List<Meld>()},
        {Mentsu.Kantsu, new List<Meld>()}
    };

    internal Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int)seat);
    }

    internal int HandLength()
    {
        return Hand.Length() + (3 * Melds.Count);
    }

    internal bool IsOpen()
    {
        foreach (Meld Meld in Melds)
        {
            if (Meld.Open) return true;
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
        // CalculateShanten();

        foreach (Tile tile in Hand.Tiles())
        {
            if (Hand[tile] == 4) _callableValues[Naki.AnKan]?.Add(tile.value);
        }
    }

    internal void Draw(Tile tile)
    {
        Hand.Draw(tile);
        DetermineCallOnDraw();
    }

    private void DetermineCallOnDiscard()
    {
        // CalculateShanten();

        foreach (Tile tile in Hand.Tiles())
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

            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile + 1))
            {
                _callableValues[Naki.ChiiShimo]?.Add((tile - 1).value);
            }

            if (Hand.ContainsTile(tile + 2))
            {
                _callableValues[Naki.ChiiNaka]?.Add((tile + 1).value);
            }

            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile - 1))
            {
                _callableValues[Naki.ChiiKami]?.Add((tile + 1).value);
            }
        }
    }

    internal void Discard(Tile tile)
    {
        bool success = Hand.Discard(tile);

        if (success)
        {
            Graveyard.Add(tile);
            GraveyardContents.Add(tile.value);
        }

        DetermineCallOnDiscard();
        // DetermineFuriten();
    }

    private void AddToWinningHand(Mentsu mentsu, Meld Meld)
    {
        WinningHand.GetValueOrDefault(mentsu)?.Add(Meld);
    }

    internal void AddMeld(Meld Meld)
    {
        Melds.Add(Meld);
        AddToWinningHand(Meld.Mentsu, Meld);

        Value value = Meld[0].value;

        JustCalled = value;

        if (Meld.Open && Meld.Mentsu == Mentsu.Koutsu)
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
        foreach (List<Meld> list in WinningHand.Values) list.Clear();
    }

    // Shanten Calculation:

    private int HandCount(Value value)
    {
        Tile normal = (Tile)value;
        Tile special = ~((Tile)value);

        if (Hand.ContainsTile(normal)) return Hand[normal];
        else if (Hand.ContainsTile(special)) return Hand[special];
        else return 0;
    }

    private int ShuntsuCount(Tile tile)
    {
        int firstCount = HandCount(tile.value);
        int secondCount = HandCount(tile.value.Next());
        int thirdCount = HandCount(tile.value.Next().Next());

        if
        (
            !tile.IsHonor() && 
            tile.CanStartShuntsu() && 
            secondCount > 0 && 
            thirdCount > 0
        )
        {
            return (new int[] {firstCount, secondCount, thirdCount}).Min();
        }
        else return 0;
    }

    private SortedDictionary<Tile, int> GetUpdatedCount(SortedDictionary<Tile, int> count, Meld meld)
    {
        SortedDictionary<Tile, int> updatedCount = new SortedDictionary<Tile, int>(count);
        foreach (Tile tile in meld.GetSortedTiles())
        {
            updatedCount[tile]--;
            if (updatedCount[tile] == 0) updatedCount.Remove(tile);
            else if (tile.akadora)
            {
                updatedCount[~tile] = updatedCount[tile];
                updatedCount.Remove(tile);
            }
        }
        return updatedCount;
    }

    private List<Meld> GetUpdatedMelds(List<Meld> melds, Meld meld)
    {
        List<Meld> updatedMelds = new List<Meld>(melds);
        updatedMelds.Add(meld);
        return updatedMelds;
    }

    private bool GetShuntsuAkadora(SortedDictionary<Tile, int> count, Tile tile)
    {
        return tile.akadora || count.ContainsKey(~(tile + 1)) || count.ContainsKey(~(tile + 2));
    }

    private void DetermineFuriten()
    {
        // TODO
    }

    private void TreeOfHands(SortedDictionary<Tile, int> count, List<Meld> melds)
    {
        foreach (Tile tile in count.Keys)
        {
            Value value = tile.value;
            bool akadora = tile.akadora;

            if (count[tile] >= 2)
            {
                Meld anJan = new AnJan(value, akadora);
                TreeOfHands
                (
                    GetUpdatedCount(count, anJan), 
                    GetUpdatedMelds(melds, anJan)
                );
            }
            if (count[tile] >= 3)
            {
                Meld anKou = new AnKou(value, akadora);
                TreeOfHands
                (
                    GetUpdatedCount(count, anKou), 
                    GetUpdatedMelds(melds, anKou)
                );
            }

            SortedDictionary<Tile, int> updatedCount = new SortedDictionary<Tile, int>(count);
            List<Meld> updatedMelds = new List<Meld>(melds);
            for (int i = 0; i < ShuntsuCount(tile); i++)
            {
                Meld anJun = new AnJun(value, i == 0 ? GetShuntsuAkadora(count, tile) : false);
                updatedCount = GetUpdatedCount(updatedCount, anJun);
                updatedMelds = GetUpdatedMelds(updatedMelds, anJun);
            }
            TreeOfHands(updatedCount, updatedMelds);
        }

        // Evaluate hands here:

        // 22 44 66 888 99 WWW
        // 4444 55 666 8 9 WW
        // 111 1 22 33 4 5 6 777

        // if ()
        // {

        // }
    }
}
