using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class LeastRecentlyUsedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {
    private readonly uint capacity;

    private readonly IDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> items;

    private readonly LinkedList<KeyValuePair<TKey, TValue>> order = new LinkedList<KeyValuePair<TKey, TValue>>();

    private readonly uint toDrop;


    [CLSCompliant(false)]
    public LeastRecentlyUsedDictionary(uint capacity, ConcurrencyLevel concurrent = ConcurrencyLevel.NonConcurrent)
    {
      if (concurrent == ConcurrencyLevel.Concurrent) {
        items = new ConcurrentDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
      }
      else {
        items = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
      }
      this.capacity = capacity;
      toDrop = Math.Min(10, (uint)(capacity * 0.07));
    }
    public LeastRecentlyUsedDictionary(int capacity, ConcurrencyLevel concurrent = ConcurrencyLevel.NonConcurrent)
      : this((uint)capacity, concurrent)
    {
    }


    [CLSCompliant(false)]
    public uint Capacity
    {
      get
      {
        return capacity;
      }
    }
    public int Count
    {
      get
      {
        return items.Count;
      }
    }
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }
    public ICollection<TKey> Keys
    {
      get
      {
        return items.Keys;
      }
    }
    public ICollection<TValue> Values
    {
      get
      {
        return (from i in items.Values
                select i.Value.Value).ToList();
      }
    }


    public TValue this[TKey key]
    {
      get
      {
        return items[key].Value.Value;
      }
      [MethodImpl(MethodImplOptions.Synchronized)]
      set
      {
        Remove(key);
        Add(key, value);
      }
    }


    private void MaybeDropSome()
    {
      if (Count <= capacity) {
        return;
      }
      for (var i = 0; i < toDrop; ++i) {
        items.Remove(order.Last.Value.Key);
        order.RemoveLast();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return items.GetEnumerator();
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      var n = order.AddFirst(item);
      items.Add(item.Key, n);
      MaybeDropSome();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(TKey key, TValue value)
    {
      Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Clear()
    {
      items.Clear();
      order.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return items.ContainsKey(item.Key);
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
      foreach (var i in items) {
        yield return i.Value.Value;
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(TKey key)
    {
      LinkedListNode<KeyValuePair<TKey, TValue>> node;
      if (items.TryGetValue(key, out node)) {
        items.Remove(key);
        order.Remove(node);
        return true;
      }
      return false;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      LinkedListNode<KeyValuePair<TKey, TValue>> node;
      if (items.TryGetValue(item.Key, out node)) {
        items.Remove(item.Key);
        order.Remove(node);
        return true;
      }
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      LinkedListNode<KeyValuePair<TKey, TValue>> node;
      if (items.TryGetValue(key, out node)) {
        value = node.Value.Value;
        return true;
      }
      value = default(TValue);
      return false;
    }
  }
}
