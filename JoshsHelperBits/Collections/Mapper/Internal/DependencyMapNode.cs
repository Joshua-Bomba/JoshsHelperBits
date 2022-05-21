using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal class DependencyMapNode<K,V> : IDependencyMapNode<K, V> where V : class
    {
        private K _key;
        private Dictionary<DependencyTree<K, V>, DependencyTreeNode<K, V>> _mappedTreeNodes;
        internal DependencyMapNode(K key, V value)
        {
            _key = key;
            Value = value;
            _mappedTreeNodes = new Dictionary<DependencyTree<K, V>, DependencyTreeNode<K, V>>();
        }
        public K Id => _key;
        public V Value { get; set; }

        public void UpdatePrimaryId(K newKey)
        {
            _key = newKey;
        }


        public void Remove()
        {
            foreach (KeyValuePair<DependencyTree<K, V>, DependencyTreeNode<K, V>> kv in _mappedTreeNodes)
            {
                kv.Key.DetachTreeNode(kv.Value);
                kv.Value.Reset();
            }
            _mappedTreeNodes.Clear();
        }

        public void Reset()
        {
            _mappedTreeNodes = null;
            Value = null;
        }

        public bool HasDependencies()
        {
            foreach(KeyValuePair<DependencyTree<K,V>,DependencyTreeNode<K,V>> kv in _mappedTreeNodes)
            {
                if (kv.Value.Branch.Any())
                    return true;
            }
            return false;
        }

        public void AttachTreeNode(DependencyTree<K, V> md)
        {
            DependencyTreeNode<K, V> treeNode = md.CreateTreeNode(Value);
            treeNode.MapNode = this;
            md.AttachTreeNode(treeNode);
            _mappedTreeNodes.Add(md, treeNode);
        }

        public DependencyTreeNode<K, V> GetTreeNode(DependencyTree<K, V> dependencyMap)
        {
            if(_mappedTreeNodes.ContainsKey(dependencyMap))
            {
                return _mappedTreeNodes[dependencyMap];
            }
            return null;
        }
    }
}
