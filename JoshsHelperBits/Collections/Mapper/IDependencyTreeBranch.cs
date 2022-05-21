using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IDependencyTreeBranch<K,V> : IDependencyNodes<K, V>, IReadOnlyDictionary<K, IDependencyTreeNode<K, V>> where V : class
    {
        bool Any();
        IDependencyTreeNode<K,V> GetNode(K key);
    }
}
