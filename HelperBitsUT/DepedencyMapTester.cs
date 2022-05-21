using HelperBitsUT.TestData;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT
{
    public class DepedencyMapTester : BaseTest,  IDepedencyTesterMethods
    {
        private class DepedencyMapAdapter<K, V> : IDependencySorter<K, V> where K : struct
        {
            public void Add(V item)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<V> Data()
            {
                throw new NotImplementedException();
            }

            public IEnumerator<V> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        IDependencySorter<KProp, TProp> IDepedencyTesterMethods.GetDependencySorter<KProp, TProp>(Func<TProp, KProp?> keySelector1, Func<TProp, KProp?> keySelector2)
        {
            throw new NotImplementedException();
        }

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
