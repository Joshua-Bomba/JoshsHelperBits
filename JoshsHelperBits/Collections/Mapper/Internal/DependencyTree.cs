using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    //Provide a public interface for DependencyTree
    internal class DependencyTree<K,V> : IDependencyTree<K,V> where V : class
    {
        protected DependencyMap<K, V> _idMap;
        protected Func<V, K> _parentSelector;
        protected Dictionary<K, DependencyTreeBranch<K,V>> _fragmentedBranches;
        protected DependencyTreeBranch<K, V> _rootBranch;

        IDependencyMap<K, V> IDependencyTree<K, V>.Map => _idMap;

        IDependencyNodes<K, V> IDependencyTree<K, V>.Root => _rootBranch;

        internal DependencyTree(DependencyMap<K, V> idMap, Func<V,K> parentSelector)
        {
            _idMap = idMap;
            _parentSelector = parentSelector;
            _fragmentedBranches = new Dictionary<K, DependencyTreeBranch<K, V>>();
            _rootBranch = new DependencyTreeBranch<K, V>(default(K));
            _idMap.AddMappedDepedency(this);
        }

        private void FragmentBranch(DependencyTreeNode<K,V> node)
        {
            if(node.Branch != null)
            {
                if (node.Branch.Any())
                {
                    _fragmentedBranches.Add(node.MapNode.Id, node.Branch);
                }
            }
        }

        private void FixFragmentedBranches(DependencyTreeNode<K, V> node)
        {
            K id = node.MapNode.Id;
            if (_fragmentedBranches.ContainsKey(id))
            {
                node.Branch = _fragmentedBranches[id];
                _fragmentedBranches.Remove(id);
            }
        }

        public void Reset()
        {
            _fragmentedBranches.Clear();
            _rootBranch = new DependencyTreeBranch<K, V>(default(K));
        }

        public void DetachTreeNode(DependencyTreeNode<K, V> node)
        {
            if(node.ParentId == null)
            {
                _rootBranch.RemoveNode(node);
                FragmentBranch(node);
                return;
            }
            DependencyTreeNode<K, V>/*?*/ parentNode = _idMap.GetMapNode(node.ParentId)?.GetTreeNode(this);

            if(parentNode != null)
            {
                parentNode.Branch.RemoveNode(node);
                FragmentBranch(node);
            }
            else
            {
                K parentId = node.ParentId;
                if (_fragmentedBranches.ContainsKey(parentId))
                {
                    _fragmentedBranches.Remove(parentId);
                }
                FragmentBranch(node);
            }
        }

        public void AttachTreeNode(DependencyTreeNode<K, V> node)
        {
            if (node.ParentId == null)
            {
                _rootBranch.Add(node);
                FixFragmentedBranches(node);//we will check if any elements have already been added where this element is the parent
                return;
            }
            //We did change the logic here a bit so I don't think a simple copy paste job will sufice
            DependencyTreeNode<K, V>/*?*/ parentNode = _idMap.GetMapNode(node.ParentId)?.GetTreeNode(this);
                

            if (parentNode != null)
            {
                if (parentNode.Branch == null)
                    parentNode.Branch = new DependencyTreeBranch<K, V>(node.ParentId);

                //lets add the element to the tree
                parentNode.Branch.Add(node);

                FixFragmentedBranches(node);//we will check if any elements have already been added where this element is the parent
            }
            else
            {
                K parentid = node.ParentId;//lets the the parentId

                //so if the parent element has not yet been added it's a broken tree
                //let's first check if we have add any other elements that have the same parent
                if (!_fragmentedBranches.ContainsKey(parentid))
                {
                    //if not will add a new TopologicalTree
                    _fragmentedBranches[parentid] = new DependencyTreeBranch<K, V>(parentid);
                }
                //will add the element to the tree
                _fragmentedBranches[parentid].Add(node);

                FixFragmentedBranches(node);//we will check if any elements have already been added where this element is the parent
            }
        }

        public DependencyTreeNode<K, V> CreateTreeNode( V item) => new DependencyTreeNode<K, V>(_parentSelector(item));

        public IEnumerable<IDependencyTreeNode<K, V>> GetChildNodes()
        {
            IEnumerable<IDependencyTreeNode<K, V>> elements = _rootBranch.GetChildNodes();
            foreach (IDependencyTreeNode<K, V> el in elements)
                yield return el;

            //we will also output any nodes not connected to the root node

            foreach (KeyValuePair<K, DependencyTreeBranch<K, V>> ultimateShrubbery in _fragmentedBranches)
            {
                elements = ultimateShrubbery.Value.GetChildNodes();
                foreach (IDependencyTreeNode<K, V> el in elements)
                    yield return el;
            }
        }

        public IEnumerable<IDependencyTreeNode<K, V>> GetAllChildNodes()
        {
            IEnumerable<IDependencyTreeNode<K,V>> elements = _rootBranch.GetAllChildNodes();
            foreach (IDependencyTreeNode<K, V> el in elements)
                yield return el;

            //we will also output any nodes not connected to the root node

            foreach (KeyValuePair<K, DependencyTreeBranch<K, V>> ultimateShrubbery in _fragmentedBranches)
            {
                elements = ultimateShrubbery.Value.GetAllChildNodes();
                foreach (IDependencyTreeNode<K, V> el in elements)
                    yield return el;
            }
        }

        public IDependencyTreeNode<K, V> GetNode(K key) => _idMap.GetMapNode(key).GetTreeNode(this);
    }
}
