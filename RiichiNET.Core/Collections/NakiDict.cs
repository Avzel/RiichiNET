namespace RiichiNET.Core.Collections;

using System.Collections.Generic;
using System.Linq;

using RiichiNET.Core.Components;
using RiichiNET.Core.Enums;

public sealed class NakiDict
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

    internal void Add(Naki naki, IEnumerable<Value> values)
    {
        if (naki == Naki.None) return;
        else foreach (Value value in values) _values[naki].Add(value);
    }

        internal void Add(Naki naki, Tile tile)
    {
        if (naki == Naki.None) return;
        else _values[naki].Add(tile.value);
    }

    public bool CanCall(Value value=Value.None, Naki naki=Naki.None)
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

    public HashSet<Value> this[Naki naki]
    {
        get {return _values[naki];}
    }
}
