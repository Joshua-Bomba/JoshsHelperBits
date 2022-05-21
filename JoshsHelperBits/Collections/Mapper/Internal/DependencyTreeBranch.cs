using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal class DependencyTreeBranch<K, V> : IDependencyTreeBranch<K, V> where V : class
    {
        private K/*?*/ _parent;
        private Dictionary<K, DependencyTreeNode<K, V>> _elements;

        IEnumerable<K> IReadOnlyDictionary<K, IDependencyTreeNode<K, V>>.Keys => _elements.Keys;

        IEnumerable<IDependencyTreeNode<K, V>> IReadOnlyDictionary<K, IDependencyTreeNode<K, V>>.Values => GetChildNodes();

        int IReadOnlyCollection<KeyValuePair<K, IDependencyTreeNode<K, V>>>.Count => _elements.Count;

        IDependencyTreeNode<K, V> IReadOnlyDictionary<K, IDependencyTreeNode<K, V>>.this[K key] => _elements[key];

        public bool ContainsKey(K key) => _elements.ContainsKey(key);

        bool IReadOnlyDictionary<K, IDependencyTreeNode<K, V>>.TryGetValue(K key, out IDependencyTreeNode<K, V> value)
        {
            if(_elements.TryGetValue(key, out DependencyTreeNode<K, V> v))
            {
                value = v;
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator<KeyValuePair<K, IDependencyTreeNode<K, V>>> IEnumerable<KeyValuePair<K, IDependencyTreeNode<K, V>>>.GetEnumerator()
            => new TreeEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new TreeEnumerator(this);

        internal DependencyTreeBranch(K/*?*/ parent)
        {
            _parent = parent;
            _elements = new Dictionary<K, DependencyTreeNode<K, V>>();
        }

        public DependencyTreeNode<K, V> GetNode(K key) => _elements[key];
        IDependencyTreeNode<K, V> IDependencyNodes<K, V>.GetNode(K key) => GetNode(key);

        public void RemoveNode(DependencyTreeNode<K, V> n)
        {
            RemoveNode(n.MapNode.Id);
        }

        public void RemoveNode(K id)
        {
            _elements.Remove(id);
        }

        public void Add(DependencyTreeNode<K, V> n)
        {
            _elements.Add(n.MapNode.Id, n);
        }

        public IEnumerable<IDependencyTreeNode<K, V>> GetAllChildNodes()
        {
            foreach (KeyValuePair<K, DependencyTreeNode<K, V>> childNode in _elements)
                yield return childNode.Value;

            foreach (KeyValuePair<K, DependencyTreeNode<K, V>> childNode  in _elements)
            {
                if (childNode.Value.Branch != null)
                {
                    IEnumerable<IDependencyTreeNode<K, V>> treeElements = childNode.Value.Branch.GetAllChildNodes();
                    foreach (var t in treeElements)
                        yield return t;

                }
            }
        }

        

        public IEnumerable<IDependencyTreeNode<K, V>> GetChildNodes() => _elements.Values;

        public IEnumerable<V> GetChildItems()
        {
            foreach(IDependencyTreeNode<K, V> n in GetChildNodes())
                yield return n.Value;
        }

        public IEnumerable<V> GetAllChildItems()
        {
            foreach (IDependencyTreeNode<K, V> n in GetAllChildNodes())
                yield return n.Value;
        }

        public bool Any() => _elements.Any();

        private class TreeEnumerator : IEnumerator<KeyValuePair<K, IDependencyTreeNode<K, V>>>
        {
            private IEnumerator<IDependencyTreeNode<K, V>> _allChildren;
            private KeyValuePair<K, IDependencyTreeNode<K, V>> _current;
            public TreeEnumerator(DependencyTreeBranch<K, V> root)
            {
                _allChildren = root.GetAllChildNodes().GetEnumerator();
                _current = default(KeyValuePair<K, IDependencyTreeNode<K, V>>);
            }
            public KeyValuePair<K, IDependencyTreeNode<K, V>> Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose() => _allChildren.Dispose();

            public bool MoveNext()
            {
                if(_allChildren.MoveNext())
                {
                    _current = new KeyValuePair<K, IDependencyTreeNode<K, V>>(_allChildren.Current.Id, _allChildren.Current);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _allChildren.Reset();
                _current = default(KeyValuePair<K, IDependencyTreeNode<K, V>>);
            }
        }

    }
}
