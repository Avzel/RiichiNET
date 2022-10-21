namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Scoring;
using RiichiNET.Util.Extensions;

public sealed class Table
{
    public State State { get; private set; } = State.None;
    public Wind Wind { get; private set; } = Wind.East;
    public int Pool { get; private set; } = 0;
    private int _round = 0;
    private Seat _turn = 0;

    private int _elapsed = 0;
    private bool _called = false;
    private Tile _justDiscarded;

    private Mountain _mountain = new Mountain();
    private Player[] _players = new Player[4]
    {
        new Player(Seat.First), 
        new Player(Seat.Second), 
        new Player(Seat.Third), 
        new Player(Seat.Fourth)
    };

    public Table()
    {
        InitialDraw();
    }

    public Player GetCurrentPlayer()
    {
        return _players[(int)_turn];
    }

    public Player GetCurrentDealer()
    {
        int seat =_round > 3 ? _round - 4 : _round;
        return _players[seat];
    }

    public HashSet<Player> GetOtherPlayers()
    {
        HashSet<Player> others = new HashSet<Player>(_players);
        others.Remove(GetCurrentPlayer());
        return others;
    }

    internal bool UninterruptedFirstRound()
    {
        return !_called && _elapsed < 4;
    }

    private bool CanKan()
    {
        return _mountain.DoraCount() < 5 && !_mountain.IsEmpty();
    }

    private void RectifyCallables(Player player)
    {
        if (!CanKan())
        {
            player.CallableValues.Clear(Naki.AnKan);
            player.CallableValues.Clear(Naki.ShouMinKan);
            player.CallableValues.Clear(Naki.DaiMinKan);
        }

        DetermineYaku(player);
        if (!player.CanWin())
        {
            player.CallableValues.Clear(Naki.Agari);
        }
    }

    public HashSet<Player> CanCallOnDiscard()
    {
        HashSet<Player> able = new HashSet<Player>();
        if (State != State.Discard) return able;

        foreach (Player player in GetOtherPlayers())
        {
            if (player.CallableValues.CanCall(_justDiscarded.value))
            {
                able.Add(player);
            }
        }
        return able;
    }

    public bool Draw(Seat? turn=null)
    {
        State = State.Draw;
        Tile tile = _mountain.Draw();
        Player player = turn == null ? GetCurrentPlayer() : _players[(int)turn];
        if (tile.value != Value.None)
        {
            player.Draw(tile);
            RectifyCallables(player);
            return true;
        }
        else return false;
    }

    public HashSet<Player> Discard(Tile tile, bool riichi=false)
    {
        State = State.Discard;
        Player player = GetCurrentPlayer();
        if (riichi) player.DeclareRiichi(tile);
        else player.Discard(tile);
        _justDiscarded = tile;
        RectifyCallables(player);
        return CanCallOnDiscard();
    }

    private void InitialDraw()
    {
        foreach (Seat seat in Enum.GetValues(typeof(Seat)))
        {
            for (int i = seat == _turn ? 14 : 13; i > 0; i--) Draw(seat);
        }
    }

    private HashSet<Player> PerformMelds(Meld meld, Seat caller)
    {
        State = State.Call;
        _called = true;

        foreach (Player player in _players) player.ClearIppatsu();

        GetCurrentPlayer().AddMeld(meld);
        if (caller != _turn) ChangeTurn(caller);

        HashSet<Player> able = new HashSet<Player>();
        if (meld.Naki is Naki.ShouMinKan or Naki.AnKan)
        {
            foreach (Player player in GetOtherPlayers())
            if (
                player.CallableValues.CanCall(meld[0].value, Naki.Agari) && 
                (
                    meld.Naki == Naki.ShouMinKan || 
                    player.YakuList.Contains(Yaku.KokushiMusou)
                )
            )
            { able.Add(player); }
        }
        return able;
    }

    internal void Rinshan()
    {
        Player player = GetCurrentPlayer();
        Tile tile = _mountain.Rinshan();
        player.Draw(tile);
        RectifyCallables(player);
    }

    internal bool StartRiichi(Tile tile)
    {
        Discard(tile, riichi: true);

        GetCurrentPlayer().YakuList.Add(Yaku.Ippatsu);

        if (!CanCallOnDiscard().Any()) return true;
        else return false;
    }

    internal void EndRiichi()
    {
        Player player = GetCurrentPlayer();
        player.ScoreChange -= Tabulation.RIICHI_COST;
        Pool += Tabulation.RIICHI_COST;

        if (UninterruptedFirstRound()) player.YakuList.Add(Yaku.RyanRiichi);
    }

    internal void NextTurn()
    {
        _turn = _turn.Next<Seat>();
        _elapsed++;
    }

    private void ChangeTurn(Seat seat)
    {
        _turn = seat;
        _elapsed++;
    }

    internal bool RoundIsOver()
    {
        return _mountain.IsEmpty();
    }

    private void DetermineYaku(Player player)
    {
        if (!player.IsComplete()) return;

        //TODO:
    }

    private void Tabulate()
    {
        // TODO:
    }

    internal bool Ryuukyoku()
    {
        // TODO: Point distribution based on Tenpai || Nagashi Mangan

        return GetCurrentDealer().IsTenpai() ? false : true;
    }

    internal bool Agari()
    {
        if (State == State.Draw)
        {
            // TODO:
        }
        else if (State == State.Discard)
        {
            // TODO:
        }
        else if (State == State.Call)
        {
            // TODO:
        }

        return GetCurrentDealer().IsWinner() ? false : true;
    }

    private Seat DetermineNextDealer()
    {
        return GetCurrentDealer().Seat.Next<Seat>();
    }

    internal void NextRound(bool overthrow)
    {
        _mountain.Reset();
        State = default;
        _elapsed = default;
        _called = default;

        if (overthrow)
        {
            Wind = Wind.Next<Wind>();
            _round++;
            _turn = DetermineNextDealer();
        }
        foreach (Player player in _players) player.NextRound(overthrow);

        InitialDraw();
    }
}

/*
Special Yaku:

MenzenchinTsumohou
	!Player.IsOpen() && Table.State == State.Draw

Chankan
	Table.State == State.Call && _turn != Player.Seat

RinshanKaihou
	Table.State == State.Call && _turn == Player.Seat

HaiteiRaoyue
	Table.State == State.Draw && Table.RoundIsOver()
	
HouteiYaoyui
	Table.State == State.Discard && Table.RoundIsOver()

TenHou
	Table.State == State.Draw && Table.UninterruptedFirstRound() Player == Table.GetCurrentDealer()

RenHou
	Table.State == State.Discard && Table.UninterruptedFirstRound() && Player.Graveyard.IsEmpty()

ChiiHou
	Table.State == State.Draw && Table.UninterruptedFirstRound() && Player != Table.GetCurrentDealer()

*/
