namespace RiichiNET.Core.Components.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Enums;

internal sealed class NakiDict
{
    private Dictionary<Naki, HashSet<Value>> _values = new Dictionary<Naki, HashSet<Value>>()
    {
        {Naki.ChiiShimo, new HashSet<Value>()},
        {Naki.ChiiNaka, new HashSet<Value>()},
        {Naki.ChiiKami, new HashSet<Value>()},
        {Naki.Pon, new HashSet<Value>()},
        {Naki.ShouMinKan, new HashSet<Value>()},
        {Naki.DaiMinKan, new HashSet<Value>()},
        {Naki.AnKan, new HashSet<Value>()},
        {Naki.Riichi, new HashSet<Value>()},
        {Naki.Agari, new HashSet<Value>()}
    };

    internal void Add(Naki naki, Value value)
    {
        if (naki == Naki.None) return;
        else _values[naki].Add(value);
    }

    internal bool Contains(Naki naki, Value value)
    {
        if (naki == Naki.None) return false;
        else return _values[naki].Contains(value);
    }

    internal bool CanCall(Naki naki, Value value=Value.None)
    {
        if (naki == Naki.None) return false;
        if (value != Value.None) return _values[naki].Contains(value);
        else return _values[naki].Any();
    }

    internal void Clear()
    {
        foreach (HashSet<Value> set in _values.Values) set.Clear();
    }
}
