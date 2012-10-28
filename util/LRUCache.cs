using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NMaier.sdlna.Util
{
  public sealed class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>
  {

    private readonly uint capacity;
    private readonly uint toDrop;
    private readonly IDictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> items = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
    private readonly LinkedList<KeyValuePair<TKey, TValue>> order = new LinkedList<KeyValuePair<TKey, TValue>>();



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
      get { return items[key].Value.Value; }
      [MethodImpl(MethodImplOptions.Synchronized)]
      set
      {
        Remove(key);
        Add(key, value);
      }
    }

    public ICollection<TValue> Values
    {
      get { return (from i in items.Values select i.Value.Value).ToList(); }
    }




    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(TKey key, TValue value)
    {
      Add(new KeyValuePair<TKey,TValue>(key, value));
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      var n = order.AddFirst(item);
      items.Add(item.Key, n);
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
  }
}
