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
        if (Count(meld.Mentsu) < 4) _hand[meld.Mentsu].Add(meld);
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
        => _hand[mentsu].AsReadOnly().OrderBy(x => x).ToList();

    internal IList<Meld> GetMelds(params Mentsu[] mentsu)
    {
        var list = GetMelds(mentsu[0]);
        for (int i = 1; i < mentsu.Length; i++)
        {
            list = list.Concat(GetMelds(mentsu[i])).ToList();
        }
        return list;
    }

    internal IEnumerable<Meld> GetAllMelds()
        => _hand.Values.SelectMany(x => x.AsEnumerable());



    internal int Count(Mentsu mentsu)
        => _hand.ContainsKey(mentsu) ? _hand[mentsu].Count() : 0;

    internal int Count(params Mentsu[] mentsu)
    {
        int count = 0;
        foreach (Mentsu specified in mentsu)
        {
            count += Count(specified);
        }
        return count;
    }

    internal int Count(Meld meld)
    {
        int count = 0;
        foreach (Meld here in _hand[meld.Mentsu])
        {
            if (here.Equals(meld)) count++;
        }
        return count;
    }
}
