using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT.TestData
{
    public interface IDependencySorter<K,V> : IEnumerable<V> where K : struct
    {
        void Add(V item);

        IEnumerable<V> Data();
    }

    public interface IDepedencyTesterMethods
    {
        IDependencySorter<KProp, TProp> GetDependencySorter<KProp, TProp>(Func<TProp, KProp?> keySelector1, Func<TProp, KProp?> keySelector2) where KProp : struct;
    }
}
