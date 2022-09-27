namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Collections.Melds;
using RiichiNET.Core.Enums;

internal sealed class WinningHand
{
    internal Dictionary<Mentsu, List<Meld>> _hand = new Dictionary<Mentsu, List<Meld>>()
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
        _hand = original._hand;
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

    internal void Clear()
    {
        foreach (List<Meld> list in _hand.Values) list.Clear();
    }

    internal int Doubles()
    {
        return this[Mentsu.Jantou];
    }

    internal int Triples()
    {
        return this[Mentsu.Shuntsu] + this[Mentsu.Koutsu];
    }

    internal int this[Mentsu mentsu]
    {
        get
        {
            if (_hand.ContainsKey(mentsu)) return _hand[mentsu].Count();
            else return 0;
        }
    }
}
