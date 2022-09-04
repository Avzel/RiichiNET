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

    internal Dictionary<Naki, List<Value>> CallableValues { get; } = new Dictionary<Naki, List<Value>>();
    internal Value JustCalled { get; private set; } = Value.None;

    internal int? Shanten { get; private set; } = null;
    internal HashSet<Value> WinningTiles { get; } = new HashSet<Value>();
    internal bool Furiten { get; set; } = false;

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
            if (meld.naki != Naki.AnKan) return true;
        }

        return false;
    }

    internal bool IsDefeated()
    {
        return this.Score <= 0;
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
        // TODO
    }

    internal void Discard(Tile tile)
    {
        // TODO
    }

    internal void CreateMeld(Meld meld)
    {
        // TODO
    }

    internal void DeclareRiichi()
    {
        // TODO
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
