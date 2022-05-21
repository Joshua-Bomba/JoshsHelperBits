using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IDependencyTree<K, V> : IDependencyNodes<K, V> where V : class
    {
        IDependencyMap<K, V> Map { get; }
        IDependencyNodes<K, V> Root { get; }
    }
}
