namespace RiichiNET.Util.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

public class ObjectCounter<T> where T: notnull, IComparable<T>
{
    private SortedDictionary<T, int> _count { get; } = new SortedDictionary<T, int>();

    public ObjectCounter(ObjectCounter<T>? original=null)
    {
        if (original != null) foreach (T t in original.Held()) _count.Add(t, original[t]);
    }

    public ObjectCounter(ICollection<T> collection)
    {
        foreach (T t in collection) Draw(t);
    }

    public void Draw(T t)
        => this[t]++;

    public void Discard(T t)
    {
        if (this[t] > 1) this[t]--;
        else this.Eliminate(t);
    }

    public void Eliminate(T t)
        => _count.Remove(t);

    public bool Has(T t)
        => _count.ContainsKey(t);

    public void Clear()
        => _count.Clear();

    public int Count()
        => Held().Count();

    public int Length()
        => _count.Values.Sum();

    public T First()
        => Held().First();

    public IEnumerable<T> Held()
        => _count.Keys;

    public int this[T t]
    {
        get
        {
            if (Has(t)) return _count[t];
            else return 0;
        }
        protected set{}
    }
}
