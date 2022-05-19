using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal class Map<K, V> : BaseMap<K,V> where V : class
    {
        protected Dictionary<K, V> _map;
        public Map(Func<V, K> keySelector) : base(keySelector)
        {
            _map = new Dictionary<K, V>();
        }

        protected override void Add(K key, V item) => _map[key] = item;

        protected override void Clear() => _map.Clear();

        protected override void Remove(K key)
        {
            _map.Remove(key);
        }
    }
}
