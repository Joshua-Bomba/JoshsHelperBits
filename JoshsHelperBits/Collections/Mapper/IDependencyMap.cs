using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IDependencyMap<K,V> : IDependencyNodes<K, V> where V : class
    {
        IMappedSet<V> Set { get;}

        IReadOnlyCollection<IDependencyTree<K, V>> Trees { get;}

        bool HasDependencies(K key);
        bool ContainsKey(K key);
        V GetItemByKey(K key);
        bool ResortByPrimaryKey(IDependencyTreeNode<K, V> updatedNode);
    }
}
