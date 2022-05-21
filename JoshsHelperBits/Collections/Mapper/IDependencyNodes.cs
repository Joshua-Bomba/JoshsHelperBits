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

        IEnumerable<V> GetChildItems();
        //C# 8 code
        //{
        //    foreach (IDependencyTreeNode<K, V> n in GetChildNodes())
        //        yield return n.Value;
        //}

        IEnumerable<V> GetAllChildItems();
        //C# 8 code
        //{
        //    foreach (IDependencyTreeNode<K, V> n in GetAllChildNodes())
        //        yield return n.Value;
        //}

        IDependencyTreeNode<K, V> GetNode(K key);
    }
}
