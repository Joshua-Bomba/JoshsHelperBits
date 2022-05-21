using JoshsHelperBits.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT.TestData
{
    public class TestObserver : ObserverDictionary<Guid, TTDGuid>.IObserver
    {
        private TopologicalCluster<Guid, TTDGuid> _modelsBy;
        private ObserverDictionary<Guid, TTDGuid> _models;
        public TestObserver(TopologicalCluster<Guid, TTDGuid> modelsBy, ObserverDictionary<Guid, TTDGuid> models)
        {
            _modelsBy = modelsBy;
            this._models = models;
            foreach (KeyValuePair<Guid, TTDGuid> kv in models)
            {
                _modelsBy.Add(kv.Value);
            }
            models.AddObserver(this);
        }
        public void Add(Guid key, TTDGuid value) => _modelsBy.Add(value);

        public void Clear() => _modelsBy.Clear();

        public void Remove(Guid key)
        {
            _modelsBy.Clear();
            foreach (KeyValuePair<Guid, TTDGuid> kv in this._models)
            {
                _modelsBy.Add(kv.Value);
            }
        }

        public void Set(Dictionary<Guid, TTDGuid> collection)
        {
            _modelsBy.Clear();
            foreach (KeyValuePair<Guid, TTDGuid> kv in collection)
            {
                _modelsBy.Add(kv.Value);
            }
        }

        public void ValueUpdatedForKey(Guid key, TTDGuid value)
        {
            _modelsBy.Clear();
            foreach (KeyValuePair<Guid, TTDGuid> kv in this._models)
            {
                _modelsBy.Add(kv.Value);
            }
        }

        public static implicit operator TopologicalCluster<Guid, TTDGuid>(TestObserver v) { return v._modelsBy; }
    }
}
