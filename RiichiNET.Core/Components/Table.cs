namespace RiichiNET.Core.Components;

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

    internal void Draw()
    {
        // TODO
    }

    internal void Discard()
    {
        // TODO
    }

    internal void InitialDraw()
    {
        // TODO
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
        // TODO
    }

    internal void NextTurn()
    {
        // TODO
    }

    private bool RoundIsOver()
    {
        // TODO
        return false;
    }

    private void Ryuukyoku()
    {
        // TODO
    }

    private void Agari()
    {
        // TODO
    }

    internal void NextRound()
    {
        // TODO (Ryuukyoku or Agari)

        _state = default;
        _wind = _wind.Next<Wind>();
        _round++;
        _turn = default;
        _mountain.Reset();
        foreach (Player player in _players) player.NextRound();
        _calls.Clear();

        InitialDraw();
    }
}
