using HelperBitsUT.TestData;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoshsHelperBits.Collections.Mapper;

namespace HelperBitsUT
{
    public class DepedencyMapTester : BaseTest,  IDepedencyTesterMethods
    {
        private class DepedencyMapAdapter<K, V> : IDependencySorter<K, V> where K : struct where V: class
        {
            private IDependencyTree<K, V> _tree;
            public DepedencyMapAdapter(IDependencyTree<K, V> tree)
            {
                _tree = tree;
            }
            public void Add(V item) => _tree.Map.Set.Add(item);

            public IEnumerable<V> Data() => _tree.GetAllChildItems();

            public IEnumerator<V> GetEnumerator() => Data().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        //Looks like we need to fix the Constraint stuff
        static Func<TProp, KProp> ConstraintHack<KProp,TProp>(Func<TProp, KProp?> selector) where KProp : struct => (x) =>
            {
                KProp?r = selector(x);
                if (r != null)
                {
                    return (KProp)r;
                }
                else 
                    return default(KProp);
            };

        IDependencySorter<KProp, TProp> IDepedencyTesterMethods.GetDependencySorter<KProp, TProp>(Func<TProp, KProp?> keySelector1, Func<TProp, KProp?> keySelector2)
            => new DepedencyMapAdapter<KProp,TProp>(MapFactory.CreateDependencyTree<KProp, TProp>(ConstraintHack(keySelector1), ConstraintHack( keySelector2)));

        private DepedencyTester _dt;

        [SetUp]
        public void Setup()
        {
            _dt = new DepedencyTester(this);
        }

        [Test]
        public void DependencyMapClusterTest() => _dt.BasicDepTest();


        [Test]
        public void DependencyMapDependencyTest() => _dt.DualDependencyTest();


        [Test]
        public void DependencyMapExtendedDepedencyTest() => _dt.DepExtendedTest();

    }
}
