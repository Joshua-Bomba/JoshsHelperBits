using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT.TestData
{
    public class DepedencyTester
    {
        private IDepedencyTesterMethods _m;
        public DepedencyTester(IDepedencyTesterMethods methods)
        {
            _m = methods;
        }

        public void BasicDepTest()
        {
            IDependencySorter<int, TTDInt> test = _m.GetDependencySorter<int, TTDInt>(x => x.Id, y => y.DepId1);
            for (int i = 0; i < 20; i++)
            {
                test.Add(new TTDInt
                {
                    Id = i,
                    DepId1 = null,
                    Content = $"test {i}"
                });
            }


            test.Add(new TTDInt { Id = 20, DepId1 = 1, Content = "test 20" });
            test.Add(new TTDInt { Id = 22, DepId1 = 20, Content = "test 22" });
            test.Add(new TTDInt { Id = 24, DepId1 = 21, Content = "test 24" });

            test.Add(new TTDInt { Id = 21, DepId1 = 2, Content = "test 21" });
            test.Add(new TTDInt { Id = 25, DepId1 = 3, Content = "test 22" });


            IEnumerable<TTDInt> result = test.Data().ToArray();

            //this test is not finish and does not really test anything
        }

        public void DualDependencyTest()
        {
            IDependencySorter<int, TTDInt> tc = _m.GetDependencySorter<int, TTDInt>(x => x.Id, y => y.DepId1);
            //what happends when we have mutliple diffrent trees   
            TTDInt[] data = new TTDInt[]
            {
                new TTDInt { Id = 1, DepId1 = null},
                new TTDInt { Id = 2, DepId1 = 1},
                new TTDInt {Id = 10, DepId1 = 1, DepId2 = 6},
                new TTDInt { Id = 3, DepId1 = 6, DepId2 = 4},
                new TTDInt { Id = 4, DepId1 = 10, DepId2 = 7},
                new TTDInt { Id = 5, DepId1 = 4, DepId2 = 8},
                new TTDInt { Id = 6, DepId1 = 1},
                new TTDInt { Id = 7, DepId1 = 1},
                new TTDInt {Id =  8, DepId1 = 1, DepId2 = 7},
                new TTDInt {Id = 9, DepId1 = 8, DepId2= 8},
                new TTDInt {Id = 20, DepId1 = 21, DepId2 = 22},
                new TTDInt {Id = 21, DepId1 = 17, DepId2 = 23},
                new TTDInt {Id = 22, DepId1 = 18, DepId2 = 24}
            };

            foreach (TTDInt t in data)
            {
                tc.Add(t);
            }


            TTDInt[] output1 = tc.Data().ToArray();
            Assert.IsTrue(data.Length == output1.Length);
            Dictionary<int, bool> idRegisted = new Dictionary<int, bool>();

            //We need to test what happens when we split broken trees
            //Were assuming these were registed previously
            idRegisted[17] = true;
            idRegisted[18] = true;
            idRegisted[23] = true;
            idRegisted[24] = true;

            foreach (TTDInt d in output1)
            {
                idRegisted[d.Id] = true;

                if (d.DepId1.HasValue)
                    Assert.IsTrue(idRegisted[d.DepId1.Value]);
            }


            tc = _m.GetDependencySorter<int, TTDInt>(x => x.Id, y => y.DepId2);
            foreach (TTDInt t in output1)
            {
                tc.Add(t);
            }

            TTDInt[] output2 = tc.Data().ToArray();
            Assert.IsTrue(data.Length == output2.Length);

            idRegisted = new Dictionary<int, bool>();
            idRegisted[17] = true;
            idRegisted[18] = true;
            idRegisted[23] = true;
            idRegisted[24] = true;

            foreach (TTDInt d in output2)
            {
                idRegisted[d.Id] = true;

                if (d.DepId1.HasValue)
                    Assert.IsTrue(idRegisted[d.DepId1.Value]);

                if (d.DepId2.HasValue)
                    Assert.IsTrue(idRegisted[d.DepId2.Value]);
            }
        }

        public void DepExtendedTest()
        {
            TTDGuid rootEl = new TTDGuid { Id = Guid.NewGuid() };

            TTDGuid el1 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = rootEl.Id };
            TTDGuid el2 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = rootEl.Id };
            TTDGuid el3 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = rootEl.Id };


            TTDGuid el4 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = el1.Id };
            TTDGuid el5 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = el2.Id, DepId1 = el4.Id };
            TTDGuid el6 = new TTDGuid { Id = Guid.NewGuid(), DepId2 = el5.Id, DepId1 = el5.Id };


            TTDGuid[] els = new TTDGuid[]
            {
                new TTDGuid {Id = Guid.NewGuid(), DepId2 = el3.Id},

            };


            Func<LinkedList<TTDGuid>> PullDefaultDepTestGuid = (() =>
            {
                LinkedList<TTDGuid> moreEls = new LinkedList<TTDGuid>();

                moreEls.AddLast(rootEl);
                moreEls.AddLast(el1);
                moreEls.AddLast(el2);
                moreEls.AddLast(el3);
                moreEls.AddLast(el4);
                moreEls.AddLast(el5);
                moreEls.AddLast(el6);

                foreach (TTDGuid c in els)
                {
                    moreEls.AddLast(c);
                }
                return moreEls;
            });


            //this should work fine
            {
                LinkedList<TTDGuid> elRes = PullDefaultDepTestGuid();
                int length = elRes.Count();
                IDependencySorter<Guid, TTDGuid> elClusterA = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId2);
                foreach (TTDGuid c in elRes)
                    elClusterA.Add(c);
                IDependencySorter<Guid, TTDGuid> elClusterB = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId1);
                foreach (TTDGuid c in elClusterA)
                    elClusterB.Add(c);

                //let's assert the order
                Assert.IsTrue(elClusterB.Data().Count() == length);

            }


            {
                LinkedList<TTDGuid> elRes = PullDefaultDepTestGuid();

                TTDGuid tEl1 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = Guid.NewGuid(), DepId2 = el1.Id };

                TTDGuid tEl2 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = rootEl.Id };

                TTDGuid tEl3 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };
                TTDGuid tEl4 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };

                elRes.AddLast(tEl1);
                elRes.AddLast(tEl2);
                elRes.AddLast(tEl3);
                elRes.AddLast(tEl4);


                int length = elRes.Count();
                IDependencySorter<Guid, TTDGuid> elClusterA = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId2);
                foreach (TTDGuid c in elRes)
                    elClusterA.Add(c);
                IDependencySorter<Guid, TTDGuid> elClusterB = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId1);
                foreach (TTDGuid c in elClusterA)
                    elClusterB.Add(c);

                Assert.IsTrue(elClusterB.Data().Count() == length);
            }

            {
                LinkedList<TTDGuid> elRes = PullDefaultDepTestGuid();



                TTDGuid tEl1 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = el4.Id, DepId2 = Guid.NewGuid() };
                TTDGuid tEl2 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = rootEl.Id };

                TTDGuid tEl3 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };
                TTDGuid tEl4 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };

                elRes.AddLast(tEl1);
                elRes.AddLast(tEl2);
                elRes.AddLast(tEl3);
                elRes.AddLast(tEl4);

                int length = elRes.Count();
                IDependencySorter<Guid, TTDGuid> elClusterA = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId2);
                foreach (TTDGuid c in elRes)
                    elClusterA.Add(c);
                IDependencySorter<Guid, TTDGuid> elClusterB = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId1);
                foreach (TTDGuid c in elClusterA)
                    elClusterB.Add(c);

                Assert.IsTrue(elClusterB.Data().Count() == length);
            }


            {
                LinkedList<TTDGuid> elRes = PullDefaultDepTestGuid();


                TTDGuid tEl1 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = Guid.NewGuid(), DepId2 = Guid.NewGuid() };
                TTDGuid tEl2 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = rootEl.Id };

                TTDGuid tEl3 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };
                TTDGuid tEl4 = new TTDGuid { Id = Guid.NewGuid(), DepId1 = tEl1.Id, DepId2 = tEl2.Id };

                elRes.AddLast(tEl1);
                elRes.AddLast(tEl2);
                elRes.AddLast(tEl3);
                elRes.AddLast(tEl4);


                int length = elRes.Count();
                IDependencySorter<Guid, TTDGuid> elClusterA = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId2);
                foreach (TTDGuid c in elRes)
                    elClusterA.Add(c);
                IDependencySorter<Guid, TTDGuid> elClusterB = _m.GetDependencySorter<Guid, TTDGuid>(x => x.Id, y => y.DepId1);
                foreach (TTDGuid c in elClusterA)
                    elClusterB.Add(c);

                Assert.IsTrue(elClusterB.Data().Count() == length);
            }
        }

    }
}
