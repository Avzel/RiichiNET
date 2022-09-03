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
    public List<Tile> Graveyard { get; } = new List<Tile>();
    public HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    public int? RiichiTile { get; private set; } = null;

    public Dictionary<Naki, List<Value>> CallableValues { get; } = new Dictionary<Naki, List<Value>>();
    public Value JustCalled { get; private set; } = Value.None;

    public int? Shanten { get; private set; } = null;
    public HashSet<Value> WinningTiles { get; } = new HashSet<Value>();
    public bool Furiten { get; internal set; } = false;

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
        foreach (Meld meld in Melds)
        {
            if (meld.naki != Naki.AnKan) return true;
        }

        return false;
    }

    public bool IsDefeated()
    {
        return this.Score <= 0;
    }

    public bool IsTenpai()
    {
        return 
            CallableValues.ContainsKey(Naki.Agari);
    }

    public bool CanCallOnDraw()
    {
        return
            CallableValues.ContainsKey(Naki.AnKan)
            || CallableValues.ContainsKey(Naki.ShouMinKan)
            || CallableValues.ContainsKey(Naki.Riichi)
            || CallableValues.ContainsKey(Naki.Agari);
    }

    public bool CanCallOnDiscard()
    {
        return 
            CallableValues.ContainsKey(Naki.Agari)
            || CallableValues.ContainsKey(Naki.Pon)
            || CallableValues.ContainsKey(Naki.DaiMinKan)
            || CallableValues.ContainsKey(Naki.ChiiLower)
            || CallableValues.ContainsKey(Naki.ChiiUpper)
            || CallableValues.ContainsKey(Naki.ChiiMiddle);
    }

    public void Draw(Tile tile)
    {
        // TODO
    }

    public void Discard(Tile tile)
    {
        // TODO
    }

    public void CreateMeld(Meld meld)
    {
        // TODO
    }

    public void DeclareRiichi()
    {
        // TODO
    }

    public void CalculateShanten()
    {
        // TODO
    }

    public void NextRound()
    {
        // TODO
    }
}
