using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoshsHelperBits.Collections
{
    /// <summary>
    /// Like a Lookup expect it behaves like both a dictionary and lookup
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DuplicatesDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, ILookup<TKey, TValue>
        where TKey : class
    {
        private Dictionary<TKey, IList<TValue>> _lookup;
        private IList<TValue> _null;
        public DuplicatesDictionary()
        {
            _lookup = new Dictionary<TKey, IList<TValue>>();
        }


        public TValue this[TKey key]
        {
            get => key == null ? _null.First() : _lookup[key].First();
            set
            {
                if (key == null)
                {
                    if (_null == null)
                    {
                        _null = new List<TValue> { value };
                    }
                    else
                    {
                        _null.Add(value);
                    }
                }
                else if (!_lookup.ContainsKey(key))
                {
                    _lookup.Add(key, new List<TValue> { value });
                }
                else
                {
                    _lookup[key][0] = value;
                }
            }
        }

        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key] => key == null ? _null : _lookup[key];

        public ICollection<TKey> Keys =>
            _null != null ? _lookup.Keys.Concat(null).ToList() : _lookup.Keys as ICollection<TKey>;

        public ICollection<TValue> Values =>
            _null != null ? _lookup.Values.Select(x => x.First()).Concat(_null).ToList() : _lookup.Values.Select(x => x.First()).ToList();

        public int Count => _null != null ? _lookup.Values.Count + 1 : _lookup.Values.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                if (_null == null)
                {
                    _null = new List<TValue> { value };
                }
                else
                {
                    _null.Add(value);
                }
            }
            else if (_lookup.ContainsKey(key))
            {
                _lookup[key].Add(value);
            }
            else
            {
                _lookup.Add(key, new List<TValue> { value });
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        public void Clear()
        {
            _null = null;
            _lookup.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => Contains(item.Key);

        public bool Contains(TKey key)
        {
            if (key == null)
                return _null != null;
            return _lookup.ContainsKey(key);
        }

        public bool ContainsKey(TKey key)
            => Contains(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            KeyValuePair<TKey, IList<TValue>> thisEl;
            for (int i = arrayIndex; i < array.Length; i++)
            {
                thisEl = _lookup.ElementAt(i);
                array[i] = new KeyValuePair<TKey, TValue>(thisEl.Key, thisEl.Value.First());
            }
            if (_null != null)
            {
                array[array.Length] = new KeyValuePair<TKey, TValue>(null, _null.First());
            }
        }

        public static IEnumerable<TSource> Concat<TSource>(IEnumerable<TSource> source, TSource singleEl)
        {
            foreach (TSource el in source)
            {
                yield return el;
            }
            yield return singleEl;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => _null != null ?
            Concat(_lookup.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value.First())), new KeyValuePair<TKey, TValue>(null, _null.First())).GetEnumerator()
            : _lookup.Select(x => new KeyValuePair<TKey, TValue>(x.Key, x.Value.First())).GetEnumerator();

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                if (_null != null)
                {
                    _null = null;
                    return true;
                }
                return false;
            }
            return _lookup.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                if (_null != null && _null.Any())
                {
                    value = _null.First();
                    return true;
                }
            }
            else if (_lookup.TryGetValue(key, out IList<TValue> e) && e.Any())
            {
                value = e.First();
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<TKey, IList<TValue>>> col;
            if (_null != null)
            {
                col = Concat(_lookup, new KeyValuePair<TKey, IList<TValue>>(null, _null));
            }
            else
            {
                col = _lookup;
            }
            return col.SelectMany(x => x.Value.Select(y => new KeyValuePair<TKey, TValue>(x.Key, y))).GroupBy(x => x.Key, x => x.Value).GetEnumerator();
        }
    }
}