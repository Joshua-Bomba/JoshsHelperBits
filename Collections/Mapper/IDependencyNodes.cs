using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IDependencyNodes<K, V> where V : class
    {
        IEnumerable<IDependencyTreeNode<K, V>> GetChildNodes();

        IEnumerable<IDependencyTreeNode<K, V>> GetAllChildNodes();

        IDependencyTreeNode<K, V> GetNode(K key);
    }
}
