namespace RiichiNET.Components;

using Enums;

using Call = System.ValueTuple<int, Enums.Seat, Enums.Naki>;

public sealed class Table
{
    // Can be used to determine agari type
    private State _state = State.None;
    private Wind _wind = Wind.East;
    private int _round = 0;
    private int _turn = 0;
    
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
}
