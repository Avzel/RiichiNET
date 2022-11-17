namespace RiichiNET.Core.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;

internal sealed class WinningHand
{
    private IDictionary<Mentsu, List<Meld>> _hand = new Dictionary<Mentsu, List<Meld>>()
    {
        {Mentsu.Jantou, new List<Meld>()},
        {Mentsu.Shuntsu, new List<Meld>()},
        {Mentsu.Koutsu, new List<Meld>()},
        {Mentsu.Kantsu, new List<Meld>()}
    };

    internal WinningHand(IList<Meld>? melds=default)
    {
        if (melds != null) foreach (Meld meld in melds) Add(meld);
    }

    internal WinningHand(WinningHand original)
    {
        foreach (Mentsu mentsu in Enum.GetValues<Mentsu>())
        {
            foreach (Meld meld in original.GetMelds(mentsu))
            {
                _hand[mentsu].Add(meld);
            }
        }
    }

    internal void Add(Meld meld)
    {
        _hand[meld.Mentsu].Add(meld);
    }
    
    internal bool Contains(Mentsu mentsu, Value value=Value.None)
    {
        if (value == Value.None) return _hand[mentsu].Any();
        else foreach (Meld meld in _hand[mentsu])
        {
            if (meld.Contains(value)) return true;
        }
        return false;
    }

    internal IList<Meld> GetMelds(Mentsu mentsu)
    {
        return _hand[mentsu].AsReadOnly();
    }

    internal IEnumerable<Meld> GetAllMelds()
    {
        return _hand.Values.SelectMany(x => x.AsEnumerable());
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
