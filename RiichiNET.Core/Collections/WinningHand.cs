namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;

internal sealed class WinningHand
{
    private Dictionary<Mentsu, List<Meld>> _hand = new Dictionary<Mentsu, List<Meld>>()
    {
        {Mentsu.Jantou, new List<Meld>()},
        {Mentsu.Shuntsu, new List<Meld>()},
        {Mentsu.Koutsu, new List<Meld>()},
        {Mentsu.Kantsu, new List<Meld>()}
    };

    internal WinningHand(List<Meld>? melds=default)
    {
        if (melds != null) foreach (Meld meld in melds) Add(meld);
    }

    internal WinningHand(WinningHand original)
    {
        
    }

    internal void Add(Meld meld)
    {
        _hand[meld.Mentsu].Add(meld);
    }
    
    internal bool Contains(Mentsu mentsu)
    {
        return _hand[mentsu].Any();
    }

    internal IList<Meld> GetMelds(Mentsu mentsu)
    {
        return _hand[mentsu].AsReadOnly();
    }

    internal int Count(Mentsu mentsu)
    {
        if (_hand.ContainsKey(mentsu)) return _hand[mentsu].Count();
        else return 0;
    }

    internal int Doubles()
    {
        return this.Count(Mentsu.Jantou);
    }

    internal int Triples()
    {
        return 
            this.Count(Mentsu.Shuntsu) + 
            this.Count(Mentsu.Koutsu) + 
            this.Count(Mentsu.Kantsu);
    }
}
