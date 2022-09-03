namespace RiichiNET.Components;

using Enums;

public sealed class Player
{
    public Seat Seat { get; }
    public Wind Wind { get; internal set; }
    public int Score { get; private set; } = 25000;
    public int ScoreChange { get; internal set; } = 0;

    public SortedDictionary<Tile, int> Hand { get; } = new SortedDictionary<Tile, int>();
    public List<Meld> Melds { get; } = new List<Meld>();
    public Tile[] Graveyard { get; } = new Tile[20];
    public HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    public int RiichiTile { get; private set; } = -1;

    public int Shanten { get; private set; } = -2;
    public Dictionary<Naki, List<Value>> CallableValues { get; } = new Dictionary<Naki, List<Value>>();
    public HashSet<Flag> Flags { get; } = new HashSet<Flag>();
    public Value JustCalled { get; internal set; } = Value.None;

    public Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int) seat + 1);
    }

    public int HandLength()
    {
        return Hand.Values.Sum() + (3 * Melds.Count);
    }

    public bool IsOpen()
    {
        return this.Melds.Any();
    }

    public bool IsDefeated()
    {
        return this.Score <= 0;
    }

    public bool IsTenpai()
    {
        return 
            CallableValues.ContainsKey(Naki.Ron)
            || CallableValues.ContainsKey(Naki.Tsumo);
    }

    public bool IsFuriten()
    {
        return
            CallableValues.ContainsKey(Naki.Tsumo)
            && !CallableValues.ContainsKey(Naki.Ron);
    }

    public bool CanCallOnDraw()
    {
        return
            CallableValues.ContainsKey(Naki.AnKan)
            || CallableValues.ContainsKey(Naki.ShouMinKan)
            || CallableValues.ContainsKey(Naki.Riichi)
            || CallableValues.ContainsKey(Naki.Tsumo);
    }

    public bool CanCallOnDiscard()
    {
        return 
            CallableValues.ContainsKey(Naki.Ron)
            || CallableValues.ContainsKey(Naki.Pon)
            || CallableValues.ContainsKey(Naki.DaiMinKan)
            || CallableValues.ContainsKey(Naki.ChiiLower)
            || CallableValues.ContainsKey(Naki.ChiiUpper)
            || CallableValues.ContainsKey(Naki.ChiiMiddle);
    }
}
