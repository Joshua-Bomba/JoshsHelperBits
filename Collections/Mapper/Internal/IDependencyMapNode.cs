using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal interface IDependencyMapNode<K, V> where V : class
    {
        K Id { get; }

        V Value { get; set; }

        DependencyTreeNode<K, V> GetTreeNode(DependencyTree<K, V> dependencyMap);

    }
}
