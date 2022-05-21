using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal abstract class BaseMap<K,V> : ISetHandlers<V> where V : class
    {
        protected IMappedSet<V> _mappedSet;
        protected Func<V, K> _keySelector;
        public BaseMap(Func<V,K> keySelector)
        {
            _mappedSet = null;
            _keySelector = keySelector;
        }
        protected abstract void Add(K key, V item);

        void ISetHandlers<V>.Add(V item) => this.Add(_keySelector(item), item);

        protected abstract void Clear();

        void ISetHandlers<V>.Clear() => Clear();

        protected abstract void Remove(K key);

        void ISetHandlers<V>.Remove(V item)
        {
            K key = _keySelector(item);
            Remove(key);
        }

        void ISetHandlers<V>.Attach(IMappedSet<V> mappedSet) => _mappedSet = mappedSet;
    }
}
