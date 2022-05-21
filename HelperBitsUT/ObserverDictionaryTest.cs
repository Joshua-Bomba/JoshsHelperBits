using HelperBitsUT.TestData;
using JoshsHelperBits.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT
{
    [TestFixture]
    public class ObserverDictionaryTest : BaseTest
    {
        private ObserverDictionary<Guid, TTDGuid> _byStandardId;

        private TestObserver _byId1;

        protected TopologicalCluster<Guid, TTDGuid> ById1
        {
            get
            {

                if (_byId1 == null)
                {
                    if (_byStandardId == null)
                        _ = StandardCollection;
                    _byId1 = new TestObserver(new TopologicalCluster<Guid, TTDGuid>(x => x.Id, y => y.DepId1), _byStandardId);
                }
                return _byId1;

            }
        }

        private TestObserver _byId2;

        protected TopologicalCluster<Guid, TTDGuid> ById2
        {
            get
            {

                if (_byId2 == null)
                {
                    if (_byStandardId == null)
                        _ = StandardCollection;
                    _byId2 = new TestObserver(new TopologicalCluster<Guid, TTDGuid>(x => x.Id, y => y.DepId2), _byStandardId);
                }
                return _byId2;

            }
        }

        protected IDictionary<Guid, TTDGuid> StandardCollection
        {
            get
            {
                if (_byStandardId == null)
                {
                    _byStandardId = new ObserverDictionary<Guid, TTDGuid>(new Dictionary<Guid, TTDGuid>());
                }
                return _byStandardId;
            }
            set
            {
                _byStandardId.Clear();
                _byStandardId.SetDictionary(value as Dictionary<Guid, TTDGuid>);
            }
        }
    }
}
