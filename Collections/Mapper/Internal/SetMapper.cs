using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper.Internal
{
    //Add ISetMapper Interface
    //Add SetMapperFactory
    internal class SetMapper<V> : IMappedSet<V> where V : class
    {
        private HashSet<V> _allElements;
        private List<ISetHandlers<V>> _conditions;
        internal SetMapper() 
        { 
            _allElements = new HashSet<V>();
            _conditions = new List<ISetHandlers<V>>();
        }

        internal SetMapper(IEnumerable<V> initalElements)
        {

            //_allElements = initalElements.ToHashSet();
            _allElements = new HashSet<V>(initalElements);
            _conditions = new List<ISetHandlers<V>>();
        }

        public void AddDepedencyCondition(ISetHandlers<V> c)
        {
            c.Attach(this);
            foreach(V v in _allElements)
                c.Add(v);
            _conditions.Add(c);
        }

        public int Count => _allElements.Count;

        public bool IsReadOnly => false;

        public void Add(V item)
        {
            if(item != null)
            {
                _allElements.Add(item);
                foreach(ISetHandlers<V> v in _allElements)
                    v.Add(item);
            }
            else
            {
                throw new NullReferenceException();
            }

        } 

        public void Clear()
        {
            _allElements.Clear();
            foreach (ISetHandlers<V> v in _allElements)
                v.Clear();
        } 

        public bool Contains(V item) => item != null ? _allElements.Contains(item) : false;

        public void CopyTo(V[] array, int arrayIndex)
        {
            if(array.Any(x=>x == null))
            {
                throw new NullReferenceException();
            }
            _allElements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<V> GetEnumerator() => _allElements.GetEnumerator();

        public bool Remove(V item)
        {
            bool ret = false;
            if (item != null)
            {
                ret = _allElements.Remove(item);
                foreach (ISetHandlers<V> v in _allElements)
                    v.Remove(item);
            }
            return ret;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Any() => _allElements.Any();

        #region Internal Classes

        #endregion
    }
}
