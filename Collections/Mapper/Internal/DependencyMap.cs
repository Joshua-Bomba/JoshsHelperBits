using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal class DependencyMap<K, V> : BaseMap<K, V>, IDependencyMap<K,V> where V : class
    {
        private Dictionary<K, DependencyMapNode<K,V>> _nodeMap;
        private List<DependencyTree<K, V>> _mappedDepedencies;

        IReadOnlyCollection<IDependencyTree<K, V>> IDependencyMap<K, V>.Trees => _mappedDepedencies;

        IMappedSet<V> IDependencyMap<K, V>.Set => _mappedSet;

        internal DependencyMap(Func<V, K> keySelector) : base(keySelector)
        {
            _mappedDepedencies = new List<DependencyTree<K, V>>();
            _nodeMap = new Dictionary<K, DependencyMapNode<K,V>>();
        }

        protected override void Add(K key, V item)
        {
            DependencyMapNode<K, V> newNode = new DependencyMapNode<K, V>(key,item);
            foreach(DependencyTree<K,V> mappedDepedencies in _mappedDepedencies)
                newNode.AttachTreeNode(mappedDepedencies);
            _nodeMap.Add(key, newNode);
        }

        protected override void Clear()
        {
            foreach(DependencyTree<K,V> tree in _mappedDepedencies)
            {
                tree.Reset();
            }
            //I don't think this step is nessary but i'm gonna do it anyway
            foreach(KeyValuePair<K, DependencyMapNode<K, V>> node in _nodeMap)
            {
                node.Value.Reset();
            }
            _nodeMap.Clear();
        }

        protected override void Remove(K key)
        {
            DependencyMapNode<K, V> n = _nodeMap[key];
            n.Reset();
            _nodeMap.Remove(key);
        }

        public void AddMappedDepedency(DependencyTree<K, V> mappedDepedencies)
        {
            _mappedDepedencies.Add(mappedDepedencies);
            foreach(KeyValuePair<K, DependencyMapNode<K,V>> kv in _nodeMap) {
                kv.Value.AttachTreeNode(mappedDepedencies);
            }
        }

        public IDependencyMapNode<K, V> GetMapNode(K key)
        {
            if (_nodeMap.ContainsKey(key))
            {
                return _nodeMap[key];
            }
            return null;
        }

        public IEnumerable<IDependencyTreeNode<K, V>> GetChildNodes()
        {
            //TODO: replace this with some merging code to handle both depedencies
            //We are going to take the last one for now
            DependencyTree<K,V> lastTree= _mappedDepedencies.LastOrDefault();
            if (lastTree != null)
            {
                return lastTree.GetChildNodes();
            }
            return new IDependencyTreeNode<K, V>[0];
        }

        public IEnumerable<IDependencyTreeNode<K, V>> GetAllChildNodes()
        {
            //TODO: replace this with some merging code to handle both depedencies
            //We are going to take the last one for now
            DependencyTree<K, V> lastTree = _mappedDepedencies.LastOrDefault();
            if (lastTree != null)
            {
                return lastTree.GetAllChildNodes();
            }
            return new IDependencyTreeNode<K, V>[0];
        }

        public bool HasDependencies(K key) => _nodeMap[key].HasDependencies();

        public V GetItemByKey(K key) => _nodeMap[key].Value;

        public bool ContainsKey(K key) => _nodeMap.ContainsKey(key);

        public bool ResortByPrimaryKey(IDependencyTreeNode<K, V> updatedNode)
        {
            if(updatedNode != null)
            {
                K oldKey = updatedNode.Id;
                K newKey = _keySelector(updatedNode.Value);
                if (!newKey.Equals(oldKey)&& _nodeMap.ContainsKey(oldKey))
                {
                    DependencyMapNode<K, V> mapNode = _nodeMap[oldKey];
                    //Detach Nodes
                    foreach (DependencyTree<K, V> mappedDepedencies in _mappedDepedencies)
                    {
                        mappedDepedencies.DetachTreeNode(mapNode.GetTreeNode(mappedDepedencies));
                    }
                    //Updated Key
                    _nodeMap.Remove(oldKey);
                    mapNode.UpdatePrimaryId(newKey);
                    _nodeMap.Add(newKey, mapNode);
                    //Reattach
                    foreach (DependencyTree<K, V> mappedDepedencies in _mappedDepedencies)
                    {
                        mappedDepedencies.AttachTreeNode(mapNode.GetTreeNode(mappedDepedencies));
                    }

                }
                return true;
            }
            
           
            return false;
        }
    }
}
