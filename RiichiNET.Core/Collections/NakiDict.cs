namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

internal sealed class NakiDict
{
    private IDictionary<Naki, ISet<Value>> _values = new Dictionary<Naki, ISet<Value>>()
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
        if (naki != Naki.None && value != Value.None) _values[naki].Add(value);
    }

    internal void Add(Naki naki, IEnumerable<Value> values)
    {
        if (naki != Naki.None) foreach (Value value in values)
        {
            _values[naki].Add(value);
        }
    }

    internal void Add(Naki naki, Tile tile)
    {
        if (naki != Naki.None && tile != Value.None) _values[naki].Add(tile);
    }

    internal void Remove(Naki naki, Value value)
    {
        if (naki != Naki.None) _values[naki].Remove(value);
    }

    internal bool Able(Value value=Value.None, Naki naki=Naki.None)
    {
        if (naki == Naki.None && value == Value.None) return false;
        else if (value == Value.None)
        {
            return _values[naki].Any();
        }
        else if (naki == Naki.None) foreach (HashSet<Value> values in _values.Values)
        {
            if (values.Contains(value)) return true;
        }
        else
        {
            return _values[naki].Contains(value);
        }
        return false;
    }

    internal void Clear(Naki naki=Naki.None)
    {
        if (naki == Naki.None)
        {
            foreach (HashSet<Value> set in _values.Values) set.Clear();
        }
        else _values[naki].Clear();
    }

    internal ISet<Value> this[Naki naki]
    {
        get {return _values[naki];}
    }
}
