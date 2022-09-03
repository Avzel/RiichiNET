namespace OpenRiichi.Components;

using Enums;

public sealed class Player
{
    public Seat Seat { get; }
    public Wind Wind { get; internal set; }
    public int Score { get; private set; } = 25000;
    public int ScoreChange { get; internal set; } = 0;

    public List<Tile> Hand { get; } = new List<Tile>();
    public Dictionary<Value, int> HandCount { get; } = new Dictionary<Value, int>();
    public List<Meld> OpenHand { get; } = new List<Meld>();
    public Tile[] Graveyard { get; } = new Tile[20];
    public HashSet<Tile> GraveyardContents { get; } = new HashSet<Tile>();
    public int RiichiTile { get; private set; } = -1;

    public int Shanten { get; private set; } = -2;
    public Dictionary<Naki, Dictionary<Value, int>> CallableValuesToIndex { get; } = new Dictionary<Naki, Dictionary<Value, int>>();
    public HashSet<Flag> Flags { get; } = new HashSet<Flag>();
    public int KanCount { get; internal set; } = 0;
    public Value JustCalled { get; internal set; } = Value.None;

    public Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int) seat + 1);
    }

    public void SortHand()
    {
        Hand.Sort();
    }

    public bool IsOpen()
    {
        return this.OpenHand.Any();
    }

    public bool IsDefeated()
    {
        return this.Score <= 0;
    }

    public bool IsTenpai()
    {
        return 
            CallableValuesToIndex.ContainsKey(Naki.Ron)
            || CallableValuesToIndex.ContainsKey(Naki.Tsumo);
    }

    public bool IsFuriten()
    {
        return
            CallableValuesToIndex.ContainsKey(Naki.Tsumo)
            && !CallableValuesToIndex.ContainsKey(Naki.Ron);
    }

    public bool CanCallOnDraw()
    {
        return
            CallableValuesToIndex.ContainsKey(Naki.AnKan)
            || CallableValuesToIndex.ContainsKey(Naki.ShouMinKan)
            || CallableValuesToIndex.ContainsKey(Naki.Riichi)
            || CallableValuesToIndex.ContainsKey(Naki.Tsumo);
    }

    public bool CanCallOnDiscard()
    {
        return 
            CallableValuesToIndex.ContainsKey(Naki.Ron)
            || CallableValuesToIndex.ContainsKey(Naki.Pon)
            || CallableValuesToIndex.ContainsKey(Naki.DaiMinKan)
            || CallableValuesToIndex.ContainsKey(Naki.ChiiLower)
            || CallableValuesToIndex.ContainsKey(Naki.ChiiUpper)
            || CallableValuesToIndex.ContainsKey(Naki.ChiiMiddle);
    }
}
