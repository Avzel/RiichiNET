namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Util.Extensions;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = 25000;
    internal int ScoreChange { get; set; } = 0;

    internal TileCount Hand { get; } = new TileCount();
    internal List<Meld> Melds { get; } = new List<Meld>();
    internal List<Tile> Graveyard { get; } = new List<Tile>();
    internal HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    private int? _riichiTile = null;

    internal NakiDict CallableValues { get; } = new NakiDict();
    internal Value JustCalled { get; private set; } = Value.None;

    internal bool Tenpai { get; private set; } = false;
    internal bool Furiten { get; set; } = false;
    internal List<WinningHand> WinningHands = new List<WinningHand>();

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

    internal bool CanCallOnDraw()
    {
        return
            CallableValues.CanCall(Naki.ShouMinKan)
            || CallableValues.CanCall(Naki.AnKan)
            || CallableValues.CanCall(Naki.Riichi)
            || CallableValues.CanCall(Naki.Agari);
    }

    internal bool CanCallOnDiscard(Value value)
    {
        return
            CallableValues.CanCall(Naki.ChiiShimo, value)
            || CallableValues.CanCall(Naki.ChiiNaka, value)
            || CallableValues.CanCall(Naki.ChiiKami, value)
            || CallableValues.CanCall(Naki.Pon, value)
            || CallableValues.CanCall(Naki.DaiMinKan, value)
            || CallableValues.CanCall(Naki.Agari, value);
    }

    private void DetermineCallOnDraw()
    {
        // CalculateShanten();

        foreach (Tile tile in Hand.Tiles())
        {
            if (Hand[tile] == 4) CallableValues.Add(Naki.AnKan, tile.value);
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
                CallableValues.Add(Naki.Pon, value);
            }
            if (Hand[tile] == 3)
            {
                CallableValues.Add(Naki.DaiMinKan, value);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile + 1))
            {
                CallableValues.Add(Naki.ChiiShimo, (tile-1).value);
            }
            if (Hand.ContainsTile(tile + 2))
            {
                CallableValues.Add(Naki.ChiiNaka, (tile+1).value);
            }
            if (!tile.IsYaoChuu() && Hand.ContainsTile(tile - 1))
            {
                CallableValues.Add(Naki.ChiiKami, (tile+1).value);
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

    internal void AddMeld(Meld Meld)
    {
        Melds.Add(Meld);

        Value value = Meld[0].value;

        JustCalled = value;

        if (Meld.Open && Meld.Mentsu == Mentsu.Koutsu)
        {
            CallableValues.Add(Naki.ShouMinKan, value);
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
        CallableValues.Clear();
        JustCalled = Value.None;
        Tenpai = false;
        Furiten = false;
        WinningHands.Clear();
    }

    // Shanten Calculation:

    private int AgnosticCount(Value value)
    {
        Tile normal = (Tile)value;
        Tile special = ~((Tile)value);
        return (new int[] {Hand[normal], Hand[special]}).Max();
    }

    private int ShuntsuCount(Tile tile)
    {
        int firstCount = AgnosticCount(tile.value);
        int secondCount = AgnosticCount(tile.value.Next());
        int thirdCount = AgnosticCount(tile.value.Next().Next());

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

    internal void BranchJantou(Tile tile, TileCount count, WinningHand hand)
    {
        if (count[tile] >= 2)
        {
            Meld anJan = new AnJan(tile.value, tile.akadora);
            TreeOfHands(UpdatedCount(count, anJan), UpdatedHand(hand, anJan));
        }
    }
    internal void BranchKoutsu(Tile tile, TileCount count, WinningHand hand)
    {
        if (count[tile] >= 3)
        {
            Meld anKou = new AnKou(tile.value, tile.akadora);
            TreeOfHands(UpdatedCount(count, anKou), UpdatedHand(hand, anKou));
        }
    }

    internal void BranchShuntsu(Tile tile, TileCount count, WinningHand hand)
    {
        int shuntsuCount = ShuntsuCount(tile);
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
            TreeOfHands(updatedCount, updatedHand);
        }
    }

    private void DetermineFuriten()
    {
        // TODO
    }

    private void TreeOfHands(TileCount count, WinningHand hand)
    {
        foreach (Tile tile in count.Tiles())
        {
            BranchJantou(tile, count, hand);
            BranchKoutsu(tile, count, hand);
            BranchShuntsu(tile, count, hand);
        }

        // Evaluate hands here:

        // 22 44 66 888 99 WWW
        // 4444 55 666 8 9 WW
        // 111 1 22 33 4 5 6 777

        if ()
        {

        }
    }
}
