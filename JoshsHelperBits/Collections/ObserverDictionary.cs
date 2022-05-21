using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoshsHelperBits.Collections
{
    /// <summary>
    /// If you have multiple Collection with the save reference but diffrent keys 
    /// This will help you keep the keys uptodate
    /// But if you make any changes to just the reference then nothing will be updated
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObserverDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        public interface IObserver
        {
            void ValueUpdatedForKey(TKey key, TValue value);

            void Add(TKey key, TValue value);

            void Clear();

            void Remove(TKey key);

            void Set(Dictionary<TKey, TValue> collection);
        }

        private class Observer : IObserver
        {
            private LinkedList<IObserver> _observing;
            public Observer()
            {
                _observing = new LinkedList<IObserver>();
            }

            public int Count => _observing.Count;

            public void ValueUpdatedForKey(TKey key, TValue value)
            {
                foreach (IObserver observer in _observing)
                {
                    observer.ValueUpdatedForKey(key, value);
                }
            }
            public void AddObserver(IObserver observer) =>
                _observing.AddLast(observer);

            public void Add(TKey key, TValue value)
            {
                foreach (IObserver observer in _observing)
                {
                    observer.Add(key, value);
                }
            }

            public void Clear()
            {
                foreach (IObserver observer in _observing)
                {
                    observer.Clear();
                }
            }

            public void Remove(TKey key)
            {
                foreach (IObserver observer in _observing)
                {
                    observer.Remove(key);
                }
            }

            public void Set(Dictionary<TKey, TValue> collection)
            {
                foreach (IObserver observer in _observing)
                {
                    observer.Set(collection);
                }
            }
        }




        private Dictionary<TKey, TValue> _wrapperDictionary;
        private Observer _observer;

        public ObserverDictionary(Dictionary<TKey, TValue> dictionary)
        {
            this.SetDictionary(dictionary);
            _observer = new Observer();
        }

        public Dictionary<TKey, TValue> ToDictionary() => _wrapperDictionary;

        public void SetDictionary(Dictionary<TKey, TValue> dictionary)
        {
            this._wrapperDictionary = dictionary;
            if (_observer != null)
            {
                _observer?.Clear();
                if (_observer.Count > 0)
                    _observer.Set(dictionary);
            }

        }

        public void AddObserver(IObserver observer) => _observer.AddObserver(observer);

        public TValue this[TKey key]
        {
            get => _wrapperDictionary[key];
            set
            {
                _wrapperDictionary[key] = value;
                _observer.ValueUpdatedForKey(key, value);
            }
        }
        public object this[object key]
        {
            get => ((IDictionary)_wrapperDictionary)[key];
            set
            {
                ((IDictionary)_wrapperDictionary)[key] = value;
                if (key is TKey k && value is TValue v)
                    _observer.ValueUpdatedForKey(k, v);
            }
        }

        public ICollection<TKey> Keys => _wrapperDictionary.Keys;

        public ICollection<TValue> Values => _wrapperDictionary.Values;//might be manipulated manual do this

        public int Count => _wrapperDictionary.Count;

        bool IDictionary.IsReadOnly => ((IDictionary)_wrapperDictionary).IsReadOnly;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_wrapperDictionary).IsReadOnly;

        object ICollection.SyncRoot => ((ICollection)_wrapperDictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)_wrapperDictionary).IsSynchronized;

        bool IDictionary.IsFixedSize => ((IDictionary)_wrapperDictionary).IsFixedSize;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _wrapperDictionary.Keys;

        ICollection IDictionary.Keys => _wrapperDictionary.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _wrapperDictionary.Values;

        ICollection IDictionary.Values => _wrapperDictionary.Values;//might be manipulated manual do this

        public void Add(TKey key, TValue value)
        {
            _wrapperDictionary.Add(key, value);
            _observer.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)_wrapperDictionary).Add(key, value);
            if (key is TKey k && value is TValue v)
                _observer.Add(k, v);
        }

        public void Clear()
        {
            _wrapperDictionary.Clear();
            _observer.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _wrapperDictionary.Contains(item);

        bool IDictionary.Contains(object key) => ((IDictionary)_wrapperDictionary).Contains(key);

        public bool ContainsKey(TKey key) => _wrapperDictionary.ContainsKey(key);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_wrapperDictionary).CopyTo(array, arrayIndex);

        void ICollection.CopyTo(Array array, int index) => ((ICollection)_wrapperDictionary).CopyTo(array, index);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _wrapperDictionary.GetEnumerator();

        public bool Remove(TKey key)
        {
            bool v = _wrapperDictionary.Remove(key);
            _observer.Remove(key);
            return v;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool v = ((ICollection<KeyValuePair<TKey, TValue>>)_wrapperDictionary).Remove(item);
            _observer.Remove(item.Key);
            return v;
        }

        public void Remove(object key)
        {
            ((IDictionary)_wrapperDictionary).Remove(key);
            if (key is TKey k)
                _observer.Remove(k);
        }

        public bool TryGetValue(TKey key, out TValue value) => _wrapperDictionary.TryGetValue(key, out value);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _wrapperDictionary.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => _wrapperDictionary.GetEnumerator();
    }
}
