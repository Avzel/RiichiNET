namespace RiichiNET.Core.Components;

using System;
using System.Collections.Generic;

using RiichiNET.Core.Collections;
using RiichiNET.Core.Enums;
using RiichiNET.Util.Extensions;

using Call = System.ValueTuple<int, Enums.Seat, Enums.Naki>;

internal sealed class Table
{
    // Can be used to determine agari type
    private State _state = State.None;
    private Wind _wind = Wind.East;
    private int _round = 0;
    private Seat _turn = 0;
    private int _pool = 0;

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

    internal bool Draw(Seat? turn=null)
    {
        Tile tile = _mountain.Draw();
        if (tile.value != Value.None)
        {
            if (turn == null) _players[(int)_turn].Draw(tile);
            else _players[(int)turn].Draw(tile);
            return true;
        }
        else return false;
    }

    internal void Discard(Tile tile)
    {
        _players[(int)_turn].Discard(tile);
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

    internal void HandleCallsDraw()
    {
        // TODO
    }

    internal void HandleCallsDiscard()
    {
        // TODO
    }

    internal void Rinshan()
    {
        Tile tile = _mountain.Rinshan();
        _players[(int)_turn].Draw(tile);
    }

    internal void NextTurn()
    {
        _turn = _turn.Next<Seat>();
    }

    internal void ChangeTurn(Seat seat)
    {
        _turn = seat;
    }

    private bool RoundIsOver()
    {
        return _mountain.IsEmpty();
    }

    private void Ryuukyoku()
    {
        // TODO
    }

    private void Agari()
    {
        // TODO
    }

    private Seat DetermineNextDealer()
    {
        int seat =_round > 3 ? _round - 4 : _round;
        return (Seat) Enum.ToObject(typeof(Seat), seat);
    }

    internal void NextRound()
    {
        // TODO (Ryuukyoku or Agari)

        _state = default;
        _wind = _wind.Next<Wind>();
        _round++;
        _turn = DetermineNextDealer();
        _mountain.Reset();
        foreach (Player player in _players) player.NextRound();
        _calls.Clear();

        InitialDraw();
    }
}
