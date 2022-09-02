namespace OpenRiichi.Components;

using Value = Enums.Value;
using Wind = Enums.Wind;
using Seat = Enums.Seat;

public sealed class Player
{
    public Seat seat { get; }
    public Wind wind { get; internal set;}
    public int score { get; internal set;} = 25000;

    private readonly Tile[] hand = new Tile[14];
    private readonly Dictionary<Value, int> handCount = new Dictionary<Value, int>();

    

    public Player(Seat seat)
    {
        this.seat = seat;
        this.wind = (Wind) Enum.ToObject(typeof(Wind), (int) seat + 1);
    }

    
}
