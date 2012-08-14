using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NMaier.sdlna.Thumbnails
{
  class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>
  {

    private readonly uint capacity;
    private readonly uint toDrop;
    private readonly IDictionary<TKey, TValue> items = new Dictionary<TKey, TValue>();
    private readonly List<TKey> order = new List<TKey>();



    public LRUCache(uint aCapacity)
    {
      capacity = aCapacity;
      toDrop = Math.Min(10, (uint)(capacity * 0.07));
    }



    public uint Capacity
    {
      get { return capacity; }
    }

    public int Count
    {
      get { return items.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public ICollection<TKey> Keys
    {
      get { return items.Keys; }
    }

    public TValue this[TKey key]
    {
      get { return items[key]; }
      set
      {
        if (items.ContainsKey(key)) {
          items[key] = value;
        }
        items.Add(key, value);
      }
    }

    public ICollection<TValue> Values
    {
      get { return items.Values; }
    }




    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(TKey key, TValue value)
    {
      items.Add(key, value);
      order.Add(key);
      MaybeDropSome();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      items.Add(item);
      order.Add(item.Key);
      MaybeDropSome();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Clear()
    {
      items.Clear();
      order.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return items.Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
      return items.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return items.GetEnumerator();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(TKey key)
    {
      if (items.Remove(key)) {
        order.Remove(key);
        return true;
      }
      return false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      if (items.Remove(item)) {
        order.Remove(item.Key);
        return true;
      }
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return items.TryGetValue(key, out value);
    }

    private void MaybeDropSome()
    {
      if (Count <= capacity) {
        return;
      }
      for (var i = 0; i < toDrop; ++i) {
        var key = order[0];
        order.RemoveAt(0);
        items.Remove(key);
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return items.GetEnumerator();
    }
  }
}
