namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Scoring;
using RiichiNET.Util.Extensions;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = Tabulation.STARTING_SCORE;
    internal int ScoreChange { get; set; } = 0;

    internal TileCount Hand { get; } = new TileCount();
    internal List<Meld> Melds { get; } = new List<Meld>();
    internal List<Tile> Graveyard { get; } = new List<Tile>();
    internal HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    private int? _riichiTile = null;

    internal NakiDict CallableValues { get; } = new NakiDict();
    internal Value JustCalled { get; private set; } = Value.None;

    internal int Shanten { get; private set; } = ShantenCalculator.MAX_SHANTEN;
    internal bool IchijiFuriten { get; set; }
    internal HashSet<WinningHand> WinningHands = new HashSet<WinningHand>();

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

    internal bool IsTenpai()
    {
        return Shanten == 0;
    }

    internal bool IsFuriten()
    {
        foreach (Value value in CallableValues[Naki.Agari])
        {
            if (GraveyardContents.Contains(value)) return true;
        }
        return false || IchijiFuriten;
    }

    internal bool IsRiichi()
    {
        return _riichiTile != null;
    }

    internal bool IsDefeated()
    {
        return Score <= 0;
    }

    internal bool CanCallOnDraw()
    {
        return
            ((CallableValues.CanCall(Naki.ShouMinKan) || CallableValues.CanCall(Naki.AnKan))
                && CanKanDuringRiichi())
            || CallableValues.CanCall(Naki.Riichi)
            || WinningHands.Any();
    }

    internal bool CanCallOnDiscard(Value value)
    {
        return
            (!IsRiichi() && 
                (CallableValues.CanCall(Naki.ChiiShimo, value)
                || CallableValues.CanCall(Naki.ChiiNaka, value)
                || CallableValues.CanCall(Naki.ChiiKami, value)
                || CallableValues.CanCall(Naki.Pon, value)
                ))
            ||
            (CallableValues.CanCall(Naki.DaiMinKan, value) && CanKanDuringRiichi(value))
            ||
            CallableValues.CanCall(Naki.Agari, value);
    }

    private bool CanKanDuringRiichi(Value value=Value.None)
    {
        // TODO
        return false;
    }

    private void DetermineCallOnDraw()
    {
        EvaluateHand(true);

        foreach (Tile tile in Hand.Tiles())
        {
            if (Hand[tile] == 4) CallableValues.Add(Naki.AnKan, tile);
        }

        foreach (Meld meld in Melds)
        {
            if (meld.Mentsu == Mentsu.Koutsu && Hand.ContainsValue(meld[0]))
            {
                CallableValues.Add(Naki.ShouMinKan, meld[0].value);
            }
        }
    }

    internal void Draw(Tile tile)
    {
        if (IchijiFuriten && !IsRiichi()) IchijiFuriten = false;
        Hand.Draw(tile);
        DetermineCallOnDraw();
    }

    private void DetermineCallOnDiscard()
    {
        EvaluateHand(false);

        foreach (Tile tile in Hand.Tiles())
        {
            Value value = tile.value;

            if (Hand[tile] is 2 or 3)
            {
                CallableValues.Add(Naki.Pon, value);
            }
            if (Hand[tile] == 3)
            {
                CallableValues.Add(Naki.DaiMinKan, value);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile + 1))
            {
                CallableValues.Add(Naki.ChiiShimo, tile-1);
            }
            if (Hand.ContainsTile(tile + 2))
            {
                CallableValues.Add(Naki.ChiiNaka, tile+1);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile - 1))
            {
                CallableValues.Add(Naki.ChiiKami, tile+1);
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
    }

    internal void AddMeld(Meld Meld)
    {
        Melds.Add(Meld);

        Value value = Meld[0].value;

        JustCalled = value;
    }

    internal void DeclareRiichi(Tile tile)
    {
        _riichiTile = Graveyard.Count - 1;
    }

    private void EvaluateHand(bool draw)
    {
        CallableValues.Clear();
        WinningHands.Clear();

        ShantenCalculator sc = new ShantenCalculator(Hand, new WinningHand(Melds), draw);

        int calculated = sc.MinimumShanten;

        if (draw && calculated == 0)
        {
            CallableValues.Add(Naki.Riichi, sc.Tiles);
        }
        else if (draw && calculated == -1)
        {
            WinningHands = new HashSet<WinningHand>(sc.WinningHands);
        }
        else if (calculated == 0)
        {
            CallableValues.Add(Naki.Agari, sc.Tiles);
        }

        if (calculated < Shanten) Shanten = calculated;
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
        CallableValues.Clear();
        JustCalled = Value.None;
        Shanten = ShantenCalculator.MAX_SHANTEN;
        IchijiFuriten = false;
        WinningHands.Clear();
    }
}
