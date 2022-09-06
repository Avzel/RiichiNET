namespace RiichiNET.Components;

using Enums;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = 25000;
    internal int ScoreChange { get; set; } = 0;

    internal SortedDictionary<Tile, int> Hand { get; } = new SortedDictionary<Tile, int>();
    internal List<Meld> Melds { get; } = new List<Meld>();
    internal List<Tile> Graveyard { get; } = new List<Tile>();
    internal HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    internal int? RiichiTile { get; private set; } = null;

    internal Dictionary<Naki, HashSet<Value>> CallableValues { get; } = new Dictionary<Naki, HashSet<Value>>();
    internal Value JustCalled { get; private set; } = Value.None;

    internal int? Shanten { get; private set; } = null;
    internal bool Furiten { get; set; } = false;
    internal Dictionary<Mentsu, List<List<Tile>>> WinningHand = new Dictionary<Mentsu, List<List<Tile>>>();

    internal Player(Seat seat)
    {
        this.Seat = seat;
        this.Wind = (Wind) Enum.ToObject(typeof(Wind), (int) seat + 1);
    }

    internal int HandLength()
    {
        return Hand.Values.Sum() + (3 * Melds.Count);
    }

    internal bool IsOpen()
    {
        foreach (Meld meld in Melds)
        {
            if (meld.origin != Seat) return true;
        }

        return false;
    }

    internal bool IsDefeated()
    {
        return Score <= 0;
    }

    internal bool IsTenpai()
    {
        return 
            CallableValues.ContainsKey(Naki.Agari);
    }

    internal bool CanCallOnDraw()
    {
        return
            CallableValues.ContainsKey(Naki.AnKan)
            || CallableValues.ContainsKey(Naki.ShouMinKan)
            || CallableValues.ContainsKey(Naki.Riichi)
            || CallableValues.ContainsKey(Naki.Agari);
    }

    internal bool CanCallOnDiscard()
    {
        return 
            CallableValues.ContainsKey(Naki.Agari)
            || CallableValues.ContainsKey(Naki.Pon)
            || CallableValues.ContainsKey(Naki.DaiMinKan)
            || CallableValues.ContainsKey(Naki.ChiiLower)
            || CallableValues.ContainsKey(Naki.ChiiUpper)
            || CallableValues.ContainsKey(Naki.ChiiMiddle);
    }

    internal void Draw(Tile tile)
    {
        if (Hand.ContainsKey(tile)) Hand[tile] ++;
        else Hand[tile] = 1;
    }

    internal void Discard(Tile tile)
    {
        if (!Hand.ContainsKey(tile)) return;
        else if (Hand[tile] == 1) Hand.Remove(tile);
        else Hand[tile] --;

        Graveyard.Add(tile);
        GraveyardContents.Add(tile.value);
    }

    internal void AddMeld(Meld meld)
    {
        Melds.Add(meld);
    }

    internal void DeclareRiichi(Tile tile)
    {
        RiichiTile = Graveyard.Count - 1;
    }

    internal void CalculateShanten()
    {
        // TODO
    }

    internal void NextRound()
    {
        // TODO
    }
}
