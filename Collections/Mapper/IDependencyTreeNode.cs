using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IDependencyTreeNode<K,V> where V : class
    {
        K Id { get; }
        V Value { get;}

        K? ParentId { get; }

        IDependencyTreeBranch<K,V> Branch { get; }

    }
}
