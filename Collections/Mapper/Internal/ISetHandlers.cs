using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    internal interface ISetHandlers<V> where V : class
    {
        void Attach(IMappedSet<V> mappedSet);
        void Add(V item);
        void Clear();
        void Remove(V item);
    }
}
