namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Scoring;
using RiichiNET.Util.Extensions;

public sealed class Player
{
    internal Seat Seat { get; }
    public Wind Wind { get; set; }
    public int Score { get; private set; } = Tabulator.STARTING_SCORE;
    internal int ScoreChange { get; set; } = 0;

    public TileCount Hand { get; } = new TileCount();
    public IList<Meld> Melds { get; } = new List<Meld>();
    public IList<Tile> Graveyard { get; } = new List<Tile>();
    internal TileCount GraveyardContents { get; } = new TileCount();
    private int? _riichiTile = null;

    internal NakiDict Callables { get; } = new NakiDict();
    internal Value JustCalled { get; private set; } = Value.None;
    public Tile JustDrawn { get; private set; } = (Tile)Value.None;

    internal int Shanten { get; private set; } = HandEvaluator.MAX_SHANTEN;
    internal bool IchijiFuriten { get; set; }
    internal ISet<WinningHand> WinningHands { get; private set; } = new HashSet<WinningHand>();
    internal ISet<Value> WinningTiles { get; private set; } = new HashSet<Value>();
    public (int han, int fu) points { get; private set; }
    public ISet<Yaku> YakuList { get; internal set; } = new HashSet<Yaku>();

    internal Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int)seat);
    }

    public static implicit operator Seat(Player p) => p.Seat;

    internal int HandLength()
        => Hand.Length() + (3 * Melds.Count);

    internal bool HasDrawn()
        => HandLength() == TileCount.MAX_HAND_SIZE;

    internal bool IsOpen()
    {
        foreach (Meld Meld in Melds)
        {
            if (Meld.Open) return true;
        }
        return false;
    }

    internal bool IsTenpai()
        => Shanten == HandEvaluator.TENPAI;

    internal bool IsComplete()
        => Shanten == HandEvaluator.COMPLETE;

    internal bool IsFuriten()
    {
        foreach (Value value in Callables[Naki.Agari])
        {
            if (GraveyardContents.ContainsTile(value)) return true;
        }
        return false || IchijiFuriten;
    }

    internal bool PendingRiichi()
        => _riichiTile == Graveyard.Count();

    internal bool IsRiichi()
        => _riichiTile != null && !PendingRiichi();

    internal bool IsDefeated()
        => Score <= Tabulator.DEFEAT;

    private bool CanKanDuringRiichi(Naki naki, Value value)
    {
        if (!IsRiichi()) return true;

        TileCount testHand = new TileCount(Hand);
        WinningHand testMelds = new WinningHand(Melds);
        HandEvaluator sc;

        foreach (Value winner in Callables[Naki.Agari])
        {
            testHand.Draw(winner);
            sc = new HandEvaluator(testHand, testMelds, true);
            foreach (WinningHand winners in sc.WinningHands)
            {
                if (!winners.Contains(Mentsu.Koutsu, value)) return false;
            }
            testHand.Discard(winner);
        }
        return true;
    }

    private void DetermineCallOnDraw()
    {
        EvaluateHand();

        foreach (Tile tile in Hand.Tiles())
        {
            if (Hand[tile] == (int)Mentsu.Kantsu && CanKanDuringRiichi(Naki.AnKan, tile))
            {
                Callables.Add(Naki.AnKan, tile);
            }
        }

        foreach (Meld meld in Melds)
        {
            if (meld.Mentsu == Mentsu.Koutsu &&
                Hand.ContainsValue(meld[0]) && 
                CanKanDuringRiichi(Naki.ShouMinKan, meld[0]))
            {
                Callables.Add(Naki.ShouMinKan, meld[0]);
            }
        }
    }

    internal void Draw(Tile tile)
    {
        if (IchijiFuriten && !IsRiichi()) IchijiFuriten = false;
        Hand.Draw(tile);
        JustDrawn = tile;
        DetermineCallOnDraw();
    }

    private void DetermineCallOnDiscard()
    {
        EvaluateHand();

        foreach (Tile tile in Hand.Tiles())
        {
            if (Hand[tile] is 2 or 3)
            {
                Callables.Add(Naki.Pon, tile);
            }
            if (Hand[tile] == 3 && CanKanDuringRiichi(Naki.DaiMinKan, tile))
            {
                Callables.Add(Naki.DaiMinKan, tile);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile+1))
            {
                Callables.Add(Naki.ChiiShimo, tile-1);
            }
            if (Hand.ContainsTile(tile + 2))
            {
                Callables.Add(Naki.ChiiNaka, tile+1);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile-1))
            {
                Callables.Add(Naki.ChiiKami, tile+1);
            }
        }
    }

    internal void Discard(Tile tile)
    {
        bool success = Hand.Discard(tile);
        if (success)
        {
            Graveyard.Add(tile);
            GraveyardContents.Draw(tile);
            DetermineCallOnDiscard();
        }
    }

    internal Tile PopFromGraveyard()
    {
        Tile popped = Graveyard.Last();
        Graveyard.RemoveAt(Graveyard.Count() - 1);
        GraveyardContents.Discard(popped);
        return popped;
    }

    internal void AddMeld(Meld meld)
    {
        if (meld.Open && ((OpenMeld)meld).Naki != Naki.ShouMinKan)
        {
            Tile called = ((OpenMeld)meld).GetCalledTile();
            JustCalled = called;
            Draw(called);
            Hand.Discard(meld);
        }
        else
        {
            JustCalled = meld[0];
            if (meld.Open)
            {
                Meld? pon = GetPon(JustCalled);
                if (pon != null)
                {
                    Discard(JustDrawn);
                    Melds.Remove(pon);
                }
                else return;
            }
            else Hand.Discard(meld);
        }
        Melds.Add(meld);
    }

    private Meld? GetPon(Value value)
    {
        foreach (Meld meld in Melds)
        {
            if (meld.Mentsu == Mentsu.Koutsu && meld.Contains(JustCalled))
            {
                return meld;
            }
        }
        return null;
    }

    internal void DeclareRiichi(Tile tile)
    {
        Discard(tile);
        _riichiTile = Graveyard.Count() - 1;
    }

    private void EvaluateHand()
    {
        if (IsRiichi()) return;
        Callables.Clear();
        WinningHands.Clear();

        bool draw = HasDrawn();
        HandEvaluator he = new HandEvaluator(Hand, new WinningHand(Melds), draw);
        int calculated = he.MinimumShanten;

        if (!IsRiichi() && !IsOpen() && draw && calculated is 0 or -1)
        {
            Callables.Add(Naki.Riichi, he.Tiles);
        }
        if (draw && calculated == HandEvaluator.COMPLETE)
        {
            WinningHands = new HashSet<WinningHand>(he.WinningHands);
            WinningTiles.Add(JustCalled);
        }
        else if (!draw && calculated == HandEvaluator.TENPAI)
        {
            WinningTiles = new HashSet<Value>(he.Tiles);
        }
        if (calculated < Shanten) Shanten = calculated;
    }

    internal void NextRound(bool overthrow)
    {
        if (overthrow) Wind = Wind.Next<Wind>();
        Score += ScoreChange;
        ScoreChange = 0;
        Hand.Clear();
        Melds.Clear();
        Graveyard.Clear();
        GraveyardContents.Clear();
        _riichiTile = null;
        Callables.Clear();
        JustCalled = Value.None;
        Shanten = HandEvaluator.MAX_SHANTEN;
        IchijiFuriten = false;
        WinningHands.Clear();
        WinningTiles.Clear();
        points = (han: 0, fu: 0);
        YakuList.Clear();
    }
}
