namespace RiichiNET.Core.Scoring;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class HandEvaluator
{
    internal const int MAX_SHANTEN = 6;
    internal const int TENPAI = 0;
    internal const int COMPLETE = -1;
    private bool draw;
    internal int MinimumShanten { get; private set; } = MAX_SHANTEN;
    internal ISet<Value> Tiles { get; private set; } = new HashSet<Value>();
    internal ISet<WinningHand> WinningHands { get; private set; } = new HashSet<WinningHand>();

    internal HandEvaluator(TileCount count, WinningHand hand, bool draw=true)
    {
        this.draw = draw;
        EvaluateHand(count, hand);
    }

    private int ShuntsuCount(TileCount count, Tile tile)
    {
        int firstCount = count.ValueCount(tile);
        int secondCount = count.ValueCount(tile+1);
        int thirdCount = count.ValueCount(tile+2);

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

    private int TaatsuCount(TileCount count)
    {
        if (count.Count() <= 1) return 0;
        else foreach (Tile tile in count.Held())
        {
            if
            (
                count.ValueCount(tile+1) > 0 ||
                count.ValueCount(tile+2) > 0
            )
            {
                return 1;
            }
        }
        return 0;
    }

    private int JantouCount(WinningHand hand)
    {
        int jantou = hand.Count(Mentsu.Jantou);
        return jantou > 2 ? 2 : jantou;
    }

    private int UniqueTerminals(TileCount count)
    {
        int uniqueTerminals = 0;
        foreach (Tile tile in count.Held())
        {
            if (tile.IsYaoChuu()) uniqueTerminals++;
        }
        return uniqueTerminals;
    }

    private int TerminalPairs(WinningHand hand)
    {
        foreach (Meld jantou in hand.GetMelds(Mentsu.Jantou))
        {
            if (jantou.HasYaoChuu()) return 1;
        }
        return 0;
    }

    private bool GetShuntsuAkadora(TileCount count, Tile tile)
    {
        return tile.akadora || count.Has(~(tile + 1)) || count.Has(~(tile + 2));
    }

    private TileCount UpdatedCount(TileCount count, Meld meld)
    {
        TileCount updatedCount = new TileCount(count);
        updatedCount.Discard(meld);
        return updatedCount;
    }

    private WinningHand UpdatedHand(WinningHand hand, Meld meld)
    {
        WinningHand updatedHand = new WinningHand(hand);
        updatedHand.Add(meld);
        return updatedHand;
    }

    private void BranchJantou(Tile tile, TileCount count, WinningHand hand)
    {
        if (count[tile] >= 2)
        {
            Meld anJan = new AnJan(tile.value, tile.akadora);
            EvaluateHand(UpdatedCount(count, anJan), UpdatedHand(hand, anJan));
        }
    }
    private void BranchKoutsu(Tile tile, TileCount count, WinningHand hand)
    {
        if (count[tile] >= 3)
        {
            Meld anKou = new AnKou(tile.value, tile.akadora);
            EvaluateHand(UpdatedCount(count, anKou), UpdatedHand(hand, anKou));
        }
    }

    private void BranchShuntsu(Tile tile, TileCount count, WinningHand hand)
    {
        int shuntsuCount = ShuntsuCount(count, tile);
        if ((shuntsuCount) > 0)
        {
            TileCount updatedCount = new TileCount(count);
            WinningHand updatedHand = new WinningHand(hand);
            for (int i = 0; i < shuntsuCount; i++)
            {
                Meld anJun = new AnJun(tile.value, i == 0 ? GetShuntsuAkadora(count, tile) : false);
                updatedCount = UpdatedCount(updatedCount, anJun);
                updatedHand = UpdatedHand(updatedHand, anJun);
            }
            EvaluateHand(updatedCount, updatedHand);
        }
    }
    private int CalculateShanten(TileCount count, WinningHand hand)
    {
        int doubles = hand.Count(Mentsu.Jantou);
        int triples = hand.Count(Mentsu.Shuntsu, Mentsu.Koutsu, Mentsu.Kantsu);
        int shanten =
        (
            new int[]
            {
                8 - (2*triples) - JantouCount(hand) + TaatsuCount(count),
                6 - doubles,
                13 - UniqueTerminals(count) - TerminalPairs(hand)
            }
        ).Min();

        if (shanten < MinimumShanten) MinimumShanten = shanten;

        return shanten;
    }

    private void GetWinningTiles(TileCount count, WinningHand hand)
    {
        int singles = count.Count();
        if (singles == 0)
        {
            foreach (Meld meld in hand.GetMelds(Mentsu.Jantou))
            {
                Tiles.Add(meld[0]);
            }
        }
        else if (singles is 1 or 13)
        {
            foreach (Tile tile in count.Held())
            {
                Tiles.Add(tile);
            }
        }
        else if (singles == 2)
        {
            Tile tile = count.First();
            if (count.ContainsValue(tile+1))
            {
                if (!tile.IsYaoChuu()) Tiles.Add((tile-1));
                if (!(tile+1).IsYaoChuu()) Tiles.Add((tile+2));
            }
            else Tiles.Add((tile+1));
        }
        else if (singles == 11)
        {
            foreach (Value value in Enum.GetValues(typeof(Value)))
            {
                Tile tile = (Tile)value;
                if
                (
                    tile.IsYaoChuu() && 
                    !count.ContainsValue(tile) &&
                    value != hand.GetMelds(Mentsu.Jantou)[0][0]
                )
                {
                    Tiles.Add(value);
                    break;
                }
            }
        }
    }

    private void GetRiichiTiles(TileCount count, WinningHand hand)
    {
        int singles = count.Count();
        if (singles is 1 or 2)
        {
            foreach (Tile tile in count.Held()) Tiles.Add(tile);
        }
        else if (singles == 3)
        {
            int taatsu;
            TileCount tester = new TileCount(count);
            foreach (Tile tile in count.Held())
            {
                tester.Discard(tile);
                if ((taatsu = TaatsuCount(tester)) == 1) Tiles.Add(tile);
                tester.Draw(tile);
            }
        }
        else if (singles >= 13)
        {
            foreach (Tile tile in count.Held())
            {
                if (!tile.IsYaoChuu())
                {
                    Tiles.Add(tile);
                    break;
                }
            }
        }
    }

    private void EvaluateHand(TileCount count, WinningHand hand)
    {
        foreach (Tile tile in count.Held())
        {
            BranchJantou(tile, count, hand);
            BranchKoutsu(tile, count, hand);
            BranchShuntsu(tile, count, hand);
        }

        int shanten = CalculateShanten(count, hand);

        if (shanten == COMPLETE) WinningHands.Add(hand);

        else if (shanten == 0)
        {
            if (draw) GetRiichiTiles(count, hand);
            else GetWinningTiles(count, hand);
        }
    }
}
