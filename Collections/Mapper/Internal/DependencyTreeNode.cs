using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal class DependencyTreeNode<K, V> : IDependencyTreeNode<K, V> where V : class
    {
        internal DependencyTreeNode(K? parentId)
        {
            ParentId = parentId;
        }

        public void Reset()
        {
            ParentId = default(K);
            MapNode = null;
            Branch = null;
        }

        public K? ParentId { get; set; }
        public DependencyMapNode<K, V> MapNode { get; set; }
        public DependencyTreeBranch<K, V> Branch { get; set; }
        K IDependencyTreeNode<K, V>.Id => MapNode.Id;
        IDependencyTreeBranch<K, V> IDependencyTreeNode<K, V>.Branch => Branch;
        V IDependencyTreeNode<K, V>.Value => MapNode.Value;
    }
}
