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
    internal struct Call
    {
        internal int elapsed;
        internal Seat caller;
        internal Naki type;
    }

    public State State { get; private set; } = State.None;
    public Wind Wind { get; private set; } = Wind.East;
    public int Pool { get; private set; } = 0;
    private int _round = 0;
    private Seat _turn = 0;

    internal int Elapsed { get; private set; } = 0;
    internal LinkedList<Call> Calls { get; } = new LinkedList<Call>();
    private Tile _justDiscarded;

    private Mountain _mountain = new Mountain();
    internal Player[] Players { get; } = new Player[4]
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

    public Player GetPlayer(Seat? seat=null)
    {
        return seat == null ? Players[(int)_turn] : Players[(int)seat];
    }

    public Player GetDealer()
    {
        int seat = _round > 3 ? _round - 4 : _round;
        return Players[seat];
    }

    public HashSet<Player> GetOtherPlayers()
    {
        HashSet<Player> others = new HashSet<Player>(Players);
        others.Remove(GetPlayer());
        return others;
    }

    internal bool UninterruptedFirstRound()
    {
        return !Calls.Any() && Elapsed < 4;
    }

    private bool CanKan()
    {
        return _mountain.DoraCount() < 5 && !_mountain.IsEmpty();
    }

    private void RectifyCallables(Value value=Value.None)
    {
        Player player = GetPlayer();
        if (!CanKan())
        {
            player.CallableValues.Clear(Naki.AnKan);
            player.CallableValues.Clear(Naki.ShouMinKan);
            player.CallableValues.Clear(Naki.DaiMinKan);
        }

        HashSet<Player> players = player.HasDrawn() ? 
            new HashSet<Player> { GetPlayer() } :
            GetOtherPlayers();

        foreach (Player relevant in players)
        {
            DetermineYaku(relevant, value);
            if (!relevant.CanWin()) relevant.CallableValues.Clear(Naki.Agari);
        }
    }

    private HashSet<Player> CanCallOnDiscard()
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

    public bool Draw(Seat? turn = null)
    {
        State = State.Draw;
        Tile tile = _mountain.Draw();
        if (tile.value != Value.None)
        {
            GetPlayer(turn).Draw(tile);
            RectifyCallables();
            return true;
        }
        else return false;
    }

    public HashSet<Player> Discard(Tile tile, bool riichi = false)
    {
        State = State.Discard;
        Player player = GetPlayer();
        if (riichi) player.DeclareRiichi(tile);
        else player.Discard(tile);
        _justDiscarded = tile;
        RectifyCallables(_justDiscarded.value);
        return CanCallOnDiscard();
    }

    private void InitialDraw()
    {
        foreach (Seat seat in Enum.GetValues(typeof(Seat)))
        {
            for (int i = seat == _turn ? 14 : 13; i > 0; i--) Draw(seat);
        }
    }

    public HashSet<Player> FormMeld(Meld meld, Seat caller)
    {
        State = State.Call;
        Calls.AddLast(new Call 
        {
            elapsed = Elapsed, caller = caller, type = meld.Naki
        });

        GetPlayer(caller).AddMeld(meld);
        if (caller != _turn) ChangeTurn(caller);

        HashSet<Player> able = new HashSet<Player>();
        if (meld.Naki is Naki.ShouMinKan or Naki.AnKan)
            foreach (Player player in GetOtherPlayers())
            {
                RectifyCallables(meld[0].value);
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

    public void Rinshan()
    {
        Player player = GetPlayer();
        Tile tile = _mountain.Rinshan();
        player.Draw(tile);
        RectifyCallables();
    }

    public HashSet<Player> StartRiichi(Tile tile)
    {
        Discard(tile, riichi: true);

        Calls.AddLast(new Call 
        {
            elapsed = Elapsed, caller = GetPlayer().Seat, type = Naki.Riichi 
        });

        return CanCallOnDiscard();
    }

    public void EndRiichi()
    {
        Player player = GetPlayer();
        player.ScoreChange -= Tabulation.RIICHI_COST;
        Pool += Tabulation.RIICHI_COST;
    }

    public void NextTurn()
    {
        _turn = _turn.Next<Seat>();
        Elapsed++;
    }

    private void ChangeTurn(Seat seat)
    {
        _turn = seat;
        Elapsed++;
    }

    public bool RoundIsOver()
    {
        return _mountain.IsEmpty();
    }

    private void DetermineYaku(Player player, Value value=Value.None)
    {
        if (!player.IsComplete()) return;
        player.YakuList.Clear();

        //TODO:
    }

    private void Tabulate()
    {
        // TODO:
    }

    public bool Ryuukyoku()
    {
        // TODO: Point distribution based on Tenpai || Nagashi Mangan

        return GetDealer().IsTenpai() ? false : true;
    }

    public bool Agari()
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

        return GetDealer().IsWinner() ? false : true;
    }

    private Seat DetermineNextDealer()
    {
        return GetDealer().Seat.Next<Seat>();
    }

    public void NextRound(bool overthrow)
    {
        _mountain.Reset();
        State = State.None;
        Elapsed = 0;
        Calls.Clear();

        if (overthrow)
        {
            Wind = Wind.Next<Wind>();
            _round++;
            _turn = DetermineNextDealer();
        }
        foreach (Player player in Players) player.NextRound(overthrow);

        InitialDraw();
    }
}

/*
Special Yaku:

Ippatsu
    Player.IsRiichi() && Calls.Last().type == Naki.Riichi && Calls.Last().caller == Player && Table.Elapsed - Calls.Last().elapsed < 5

RyanRiichi
    Player.IsRiichi() && Calls.First().type == Naki.Riichi && Calls.First().caller == Player && Calls.First().elapsed < 4

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
	Table.State == State.Draw && Table.UninterruptedFirstRound() Player == Table.GetDealer()

RenHou
	Table.State == State.Discard && Table.UninterruptedFirstRound() && Player.Graveyard.IsEmpty()

ChiiHou
	Table.State == State.Draw && Table.UninterruptedFirstRound() && Player != Table.GetDealer()

*/
