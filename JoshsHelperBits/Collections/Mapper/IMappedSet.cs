using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public interface IMappedSet<V> : ICollection<V> where V : class
    {
        bool Any();
    }
}
