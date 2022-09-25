namespace RiichiNET.Core.Components.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components.Collections.Melds;
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

    internal void Add(Mentsu mentsu, Meld meld)
    {
        _hand[mentsu].Add(meld);
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
}
