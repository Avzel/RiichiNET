namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;
using System.Linq;

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

    public Player GetCurrentPlayer()
    {
        return _players[(int)_turn];
    }

    public Player GetCurrentDealer()
    {
        int seat =_round > 3 ? _round - 4 : _round;
        return _players[seat];
    }

    public bool Draw(Seat? turn=null)
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

    public void Discard(Tile tile, bool riichi=false)
    {
        State = State.Discard;
        if (riichi) GetCurrentPlayer().DeclareRiichi(tile);
        else GetCurrentPlayer().Discard(tile);
        _justDiscarded = tile;
    }

    void InitialDraw()
    {
        foreach (Seat seat in Enum.GetValues(typeof(Seat)))
        {
            for (int i = seat == _turn ? 14 : 13; i > 0; i--) Draw(seat);
        }
    }

    public List<Player> CanCall() // Need to check for yaku in order to agaru
    {
        List<Player> canCall = new List<Player>();

        if (!(State is State.Draw or State.Discard)) return canCall; // Kokushi Musou on a call

        foreach (Player player in _players)
        {
            if (player == GetCurrentPlayer()) continue;

            if (player.CallableValues.CanCall(value: _justDiscarded.value))
            {
                canCall.Add(player);
            }
        }
        return canCall;
    }

    private void PerformMelds(Meld meld, Seat caller)
    {
        if (meld.Naki is Naki.Riichi or Naki.Agari or Naki.None) return;
        State = State.Call;

        GetCurrentPlayer().AddMeld(meld);
        _calls.AddLast((Call)(_elapsed, _turn, meld.Naki));
        if (caller != _turn) ChangeTurn(caller);
    }

    internal void Rinshan()
    {
        State = State.Draw;
        Tile tile = _mountain.Rinshan();
        GetCurrentPlayer().Draw(tile);
    }

    internal bool StartRiichi(Tile tile)
    {
        Discard(tile, riichi: true);
        if (!CanCall().Any()) return true;
        else return false;
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

    internal bool Ryuukyoku() // Nagashi Mangan check
    {
        State = State.RyuuKyoku;

        // TODO: Point distribution based on Tenpai

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
        State = State.Agari;

        return GetCurrentDealer().IsWinner() ? false : true;
    }

    private Seat DetermineNextDealer()
    {
        return GetCurrentDealer().Seat.Next<Seat>();
    }

    internal void NextRound(bool overthrow)
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
