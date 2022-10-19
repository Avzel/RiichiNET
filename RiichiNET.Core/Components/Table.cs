namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;
using RiichiNET.Core.Scoring;
using RiichiNET.Util.Extensions;

using Call = System.ValueTuple<int, Enums.Seat, Enums.Naki>;

internal sealed class Table
{
    public State State { get; private set; } = State.None;
    public Wind Wind { get; private set; } = Wind.East;
    public int Pool { get; private set; } = 0;
    private int _round = 0;
    private Seat _turn = 0;
    private int _elapsed = 0;
    private Tile _justDiscarded;

    private Mountain _mountain = new Mountain();
    private Player[] _players = new Player[4]
    {
        new Player(Seat.First), 
        new Player(Seat.Second), 
        new Player(Seat.Third), 
        new Player(Seat.Fourth)
    };

    // List of calls in order (for determining Ippatsu, Daburu, etc.)
    private LinkedList<Call> _calls = new LinkedList<Call>();

    internal Table()
    {
        InitialDraw();
    }

    internal Player GetCurrentPlayer()
    {
        return _players[(int)_turn];
    }

    internal Player GetCurrentDealer()
    {
        int seat =_round > 3 ? _round - 4 : _round;
        return _players[seat];
    }

    internal bool Draw(Seat? turn=null)
    {
        State = State.Draw;
        Tile tile = _mountain.Draw();
        if (tile.value != Value.None)
        {
            if (turn == null) GetCurrentPlayer().Draw(tile);
            else _players[(int)turn].Draw(tile);
            return true;
        }
        else return false;
    }

    internal void Discard(Tile tile)
    {
        State = State.Discard;
        GetCurrentPlayer().Discard(tile);
        _justDiscarded = tile;
    }

    internal void InitialDraw()
    {
        int count;
        foreach (Seat seat in Enum.GetValues(typeof(Seat)))
        {
            if (seat == _turn) count = 14;
            else count = 13;

            for (; count > 0; count--) Draw(seat);
        }
    }

    private void PerformMelds(Meld meld)
    {
        GetCurrentPlayer().AddMeld(meld);
        _calls.AddLast((Call)(_elapsed, _turn, meld.Naki));
    }

    internal void PerformMeldsDraw(Meld meld)
    {
        if (meld.Naki is not Naki.AnKan or Naki.ShouMinKan) return;

        State = State.Call;
        PerformMelds(meld);
    }

    internal void PerformMeldsDiscard(Seat caller, Meld meld)
    {
        if (meld.Naki is not 
            Naki.ChiiKami or 
            Naki.ChiiNaka or 
            Naki.ChiiShimo or 
            Naki.Pon or 
            Naki.DaiMinKan
        ) return;

        State = State.Call;
        ChangeTurn(caller);
        PerformMelds(meld);
    }

    internal void Rinshan()
    {
        State = State.Draw;
        Tile tile = _mountain.Rinshan();
        GetCurrentPlayer().Draw(tile);
    }

    internal void StartRiichi(Tile tile)
    {
        if (!GetCurrentPlayer().Hand.ContainsTile(tile)) return;
        GetCurrentPlayer().DeclareRiichi(tile);
        Discard(tile);
    }

    internal void EndRiichi()
    {
        GetCurrentPlayer().ScoreChange -= Tabulation.RIICHI_COST;
        Pool += Tabulation.RIICHI_COST;
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

    private void Tabulate(Player player)
    {
        // TODO:
    }

    private void Ryuukyoku()
    {
        State = State.RyuuKyoku;

        // TODO: Point distribution based on Tenpai

        if (GetCurrentDealer().IsTenpai()) NextRound(false);
        else NextRound(true);
    }

    private void Agari(params Seat[] winners)
    {
        if (State == State.Draw)
        {
            // TODO:
        }
        else if (State == State.Discard)
        {
            // TODO:
        }
        State = State.Agari;

        bool overthrow = false;
        foreach (Seat seat in winners)
        {
            if (seat == GetCurrentDealer().Seat) overthrow = true;
        }
        NextRound(overthrow);
    }

    private Seat DetermineNextDealer()
    {
        return GetCurrentDealer().Seat.Next<Seat>();
    }

    private void NextRound(bool overthrow)
    {
        State = default;
        _elapsed = default;
        _mountain.Reset();
        _calls.Clear();

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
