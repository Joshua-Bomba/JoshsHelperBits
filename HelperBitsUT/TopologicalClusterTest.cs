using HelperBitsUT.TestData;
using JoshsHelperBits.Collections;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT
{
    [TestFixture]
    public class TopologicalClusterTest : BaseTest, IDepedencyTesterMethods
    {
        private class TopologicalClusterAdapater<K, V> : IDependencySorter<K, V> where K : struct
        {
            private TopologicalCluster<K, V> _cluster;
            public TopologicalClusterAdapater(TopologicalCluster<K, V> c)
            {
                _cluster = c;
            }
            public void Add(V item) => _cluster.Add(item);

            public IEnumerable<V> Data() => _cluster.Data();

            public IEnumerator<V> GetEnumerator() => _cluster.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _cluster.GetEnumerator();
        }

        private DepedencyTester _dt; 

        [SetUp]
        public void Setup()
        {
            _dt = new DepedencyTester(this);
        }

        IDependencySorter<KProp, TProp> IDepedencyTesterMethods.GetDependencySorter<KProp, TProp>(Func<TProp, KProp?> keySelector1, Func<TProp, KProp?> keySelector2)
            => new TopologicalClusterAdapater<KProp, TProp>(new TopologicalCluster<KProp, TProp>(keySelector1, keySelector2));



        [Test]
        public void ToplogicalClusterTest() => _dt.BasicDepTest();


        [Ignore("Dual Dependency Sorting Does Not work properly at this time")]
        [Test]
        public void TopologicalClusterDependecnyTest() => _dt.DualDependencyTest();


        [Test]
        public void ToplogicalClusterExtendedDepedencyTest() => _dt.DepExtendedTest();

    }
}
