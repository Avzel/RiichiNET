namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;

internal sealed class ShantenCalculator
{
    internal static int MAX_SHANTEN = 6;
    private bool draw;
    internal int MinimumShanten { get; private set; } = MAX_SHANTEN;
    internal HashSet<Value> Tiles { get; private set; } = new HashSet<Value>();
    internal HashSet<WinningHand> WinningHands { get; private set; } = new HashSet<WinningHand>();

    internal ShantenCalculator(TileCount count, WinningHand hand, bool draw)
    {
        this.draw = draw;
        EvaluateHand(count, hand);
    }

    private int ShuntsuCount(TileCount count, Tile tile)
    {
        int firstCount = count.AgnosticCount(tile);
        int secondCount = count.AgnosticCount(tile+1);
        int thirdCount = count.AgnosticCount(tile+2);

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
        else foreach (Tile tile in count.Tiles())
        {
            if
            (
                count.AgnosticCount(tile+1) > 0 ||
                count.AgnosticCount(tile+2) > 0
            )
            {
                return 1;
            }
        }
        return 0;
    }

    private int JantouCount(WinningHand hand)
    {
        int jantou = hand.Doubles();
        return jantou > 2 ? 2 : jantou;
    }

    private int UniqueTerminals(TileCount count)
    {
        int uniqueTerminals = 0;
        foreach (Tile tile in count.Tiles())
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
        return tile.akadora || count.ContainsTile(~(tile + 1)) || count.ContainsTile(~(tile + 2));
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
        int shanten =
        (
            new int[]
            {
                8 - (2*hand.Triples()) - JantouCount(hand) + TaatsuCount(count),
                6 - hand.Doubles(),
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
                Tiles.Add(meld[0].value);
            }
        }
        else if (singles is 1 or 13)
        {
            foreach (Tile tile in count.Tiles())
            {
                Tiles.Add(tile.value);
            }
        }
        else if (singles == 2)
        {
            Tile tile = count.First();
            if (count.ContainsValue(tile+1))
            {
                if (!tile.IsYaoChuu()) Tiles.Add((tile-1).value);
                if (!(tile+1).IsYaoChuu()) Tiles.Add((tile+2).value);
            }
            else Tiles.Add((tile+1).value);
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
                    value != hand.GetMelds(Mentsu.Jantou)[0][0].value
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
            foreach (Tile tile in count.Tiles()) Tiles.Add(tile.value);
        }
        else if (singles == 3)
        {
            int taatsu;
            TileCount tester = new TileCount(count);
            foreach (Tile tile in count.Tiles())
            {
                tester.Discard(tile);
                if ((taatsu = TaatsuCount(tester)) == 1) Tiles.Add(tile.value);
                tester.Draw(tile);
            }
        }
        else if (singles >= 13)
        {
            foreach (Tile tile in count.Tiles())
            {
                if (!tile.IsYaoChuu())
                {
                    Tiles.Add(tile.value);
                    break;
                }
            }
        }
    }

    private void EvaluateHand(TileCount count, WinningHand hand)
    {
        foreach (Tile tile in count.Tiles())
        {
            BranchJantou(tile, count, hand);
            BranchKoutsu(tile, count, hand);
            BranchShuntsu(tile, count, hand);
        }

        int shanten = CalculateShanten(count, hand);

        if (shanten == -1) WinningHands.Add(hand);

        else if (shanten == 0)
        {
            if (draw) GetRiichiTiles(count, hand);
            else GetWinningTiles(count, hand);
        }
    }
}
