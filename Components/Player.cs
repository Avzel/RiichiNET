namespace RiichiNET.Components;

using Enums;
using Groups;

internal sealed class Player
{
    internal Seat Seat { get; }
    internal Wind Wind { get; set; }
    internal int Score { get; private set; } = 25000;
    internal int ScoreChange { get; set; } = 0;

    internal SortedDictionary<Tile, int> Hand { get; } = new SortedDictionary<Tile, int>();
    internal List<Group> Melds { get; } = new List<Group>();
    internal List<Tile> Graveyard { get; } = new List<Tile>();
    internal HashSet<Value> GraveyardContents { get; } = new HashSet<Value>();
    internal int? RiichiTile { get; private set; } = null;

    internal Dictionary<Naki, HashSet<Value>> CallableValues { get; } = new Dictionary<Naki, HashSet<Value>>();
    internal Value JustCalled { get; private set; } = Value.None;

    internal int? Shanten { get; private set; } = null;
    internal bool Furiten { get; set; } = false;
    internal Dictionary<Mentsu, List<Group>> WinningHand = new Dictionary<Mentsu, List<Group>>()
    {
        {Mentsu.Jantou, new List<Group>()},
        {Mentsu.Shuntsu, new List<Group>()},
        {Mentsu.Koutsu, new List<Group>()},
        {Mentsu.Kantsu, new List<Group>()}
    };

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
        foreach (Group group in Melds)
        {
            if (group.Naki != Naki.AnKan) return true;
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

    internal void AddMeld(Group meld)
    {
        Melds.Add(meld);

        // TODO (add to winning hand)
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
