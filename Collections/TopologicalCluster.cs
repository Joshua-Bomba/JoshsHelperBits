using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace JoshsHelperBits.Collections
{
    /// <summary>
    /// This class is for dependency sorting root to children
    /// No Warenty, it's kinda crap
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Obsolete("Use The MapFactory")]
    public class TopologicalCluster<K, V> : ICollection<V>, IEnumerable<V>, IEnumerable, ICollection, IReadOnlyCollection<V> where K : struct/*, IComparable, IFormattable, IConvertible, IComparable<V>, IEquatable<V>*/
    {
        /// <summary>
        /// This will manage the logic to fetch data in the most efficent manner
        /// </summary>
        private class TopologicalCollection
        {
            private Dictionary<V, TopologicalElement> _collection;//The Reason we use the items value as a key is because it gives it a list like transperency
            private Dictionary<K?, TopologicalElement> _mappings;//these will store the key mappings for quick access
            public TopologicalCollection()
            {
                _collection = new Dictionary<V, TopologicalElement>();
                _mappings = new Dictionary<K?, TopologicalElement>();
            }

            public int Count => _collection.Count;

            public bool ContainsKey(K key) => _mappings.ContainsKey(key);

            public TopologicalElement GetElementByKey(K? id) => _mappings[id];

            public void Add(TopologicalElement item)
            {
                _collection.Add(item.Element, item);
                _mappings.Add(item.Id, item);
            }

            public void Clear()
            {
                _collection.Clear();
                _mappings.Clear();
            }

            public bool ContainsItem(V item) => _collection.ContainsKey(item);

            public void CopyTo(V[] array, int arrayIndex)
            {
                int index = arrayIndex;
                foreach (KeyValuePair<V, TopologicalElement> kv in _collection)
                {
                    array[index] = kv.Key;
                    index++;
                }
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                int index = arrayIndex;
                foreach (KeyValuePair<V, TopologicalElement> kv in _collection)
                {
                    array.SetValue(kv.Key, index);
                    index++;
                }
            }

            public TopologicalElement GetElementByItem(V item) => _collection[item];

            public bool RemoveByItem(V item)
            {
                TopologicalElement el = GetElementByItem(item);
                if (el.Tree == null || el.Tree.Any())
                    throw new Exception("Can't delete item because of dependence");
                _mappings.Remove(el.Id);//We need to double check what to do if there is no ID Specified
                bool removed = _collection.Remove(item);

                return removed;
            }
        }

        public interface INode
        {
            Nullable<K> Id { get; set; }
            Nullable<K> ParentId { get; set; }
            V Element { get; set; }
            IEnumerable<V> GetAllChildren();
        }

        private class TopologicalElement : INode
        {
            public Nullable<K> Id { get; set; }
            public Nullable<K> ParentId { get; set; }
            public V Element { get; set; }

            public TopologicalTree Tree { get; set; }
            public IEnumerable<V> GetAllChildren() => Tree.GetAllChildElements().Select(x => x.Element);
        }

        private class TopologicalTree
        {
            private List<TopologicalElement> _elements;

            public TopologicalTree()
            {
                _elements = new List<TopologicalElement>();
            }

            public void Add(TopologicalElement element)
            {
                _elements.Add(element);
            }

            public bool Remove(TopologicalElement element) => _elements.Remove(element);

            public int Count() => _elements.Count;

            public bool Any() => _elements.Any();

            public List<TopologicalElement> Elements => _elements;

            public IEnumerable<TopologicalElement> GetAllChildElements()
            {
                foreach (TopologicalElement e in _elements)
                    yield return e;

                foreach (TopologicalElement e in _elements)
                {
                    if (e.Tree != null)
                    {
                        IEnumerable<TopologicalElement> treeElements = e.Tree.GetAllChildElements();
                        foreach (var t in treeElements)
                            yield return t;

                    }
                }
            }
        }

        private TopologicalTree _rootTree;
        private Func<V, Nullable<K>> _idSelector;
        private Func<V, Nullable<K>> _parentSelector;
        private TopologicalCollection _collection;
        private Dictionary<K, TopologicalTree> _brokenTrees;


        private TopologicalTree RootTree
        {
            get
            {
                if (_rootTree == null)
                    _rootTree = new TopologicalTree();
                return _rootTree;
            }
        }


        public int Count => _collection.Count;

        public bool IsReadOnly => false;

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public TopologicalCluster(Func<V, Nullable<K>> idSelector, Func<V, Nullable<K>> parentSelector)
        {
            _collection = new TopologicalCollection();
            _brokenTrees = new Dictionary<K, TopologicalTree>();
            _idSelector = idSelector;
            _parentSelector = parentSelector;
        }

        private void LinkBrokenTrees(TopologicalElement element)
        {
            if (element.Id.HasValue)
            {
                if (_brokenTrees.ContainsKey(element.Id.Value))
                {
                    //let's grab the Tree and link it
                    element.Tree = _brokenTrees[element.Id.Value];


                    //foreach (TopologicalElement te in _brokenTrees[element.Id.Value])
                    //{
                    //    if (element.ParentTreeLevel == null)
                    //        element.ParentTreeLevel = new TopologicalTree();
                    //    element.ParentTreeLevel.Add(te);
                    //    Update(te);
                    //}
                    //will remove it from broken trees since it's no longer broken
                    _brokenTrees.Remove(element.Id.Value);
                }
            }
        }

        public bool HasDependencies(K key)
        {
            TopologicalElement element = ContainsKey(key);
            return element.Tree != null ? element.Tree.Any() : false;
        }

        public IEnumerable<V> Children(K key)
        {
            TopologicalElement element = ContainsKey(key);
            if (element != null && element.Tree != null)
                return element.Tree.Elements.Select(x => x.Element);
            else
                return new List<V>();
        }

        private TopologicalElement ContainsKey(K key) => _collection.ContainsKey(key) ? _collection.GetElementByKey(key) : null;

        public V GetItemByKey(K key) => _collection.ContainsKey(key) ? _collection.GetElementByKey(key).Element : default;

        public INode GetNodeByKey(K key) => _collection.ContainsKey(key) ? _collection.GetElementByKey(key) : default;

        public void Add(V item)
        {
            TopologicalElement element = new TopologicalElement { Id = _idSelector(item), ParentId = _parentSelector(item), Element = item };
            try
            {

                if (!element.ParentId.HasValue)
                {
                    RootTree.Add(element);
                    LinkBrokenTrees(element);//we will check if any elements have already been added where this element is the parent
                    return;
                }
                TopologicalElement parentNode = ContainsKey(element.ParentId.Value);


                if (parentNode != null)
                {
                    //we have a parentnode for this element, let's see if we have a tree
                    if (parentNode.Tree == null)
                        parentNode.Tree = new TopologicalTree();

                    //lets add the element to the tree
                    parentNode.Tree.Add(element);

                    LinkBrokenTrees(element);//we will check if any elements have already been added where this element is the parent
                }
                else
                {
                    K parentid = element.ParentId.Value;//lets the the parentId

                    //so if the parent element has not yet been added it's a broken tree
                    //let's first check if we have add any other elements that have the same parent
                    if (!_brokenTrees.ContainsKey(parentid))
                    {
                        //if not will add a new TopologicalTree
                        _brokenTrees[parentid] = new TopologicalTree();
                    }
                    //will add the element to the tree
                    _brokenTrees[parentid].Add(element);

                    LinkBrokenTrees(element);//we will check if any elements have already been added where this element is the parent
                }
            }
            finally
            {
                _collection.Add(element);
            }



        }

        public void Clear()
        {
            _collection.Clear();
            _brokenTrees.Clear();
            _rootTree = null;

        }

        public bool Contains(V item)
        {
            return _collection.ContainsItem(item);
        }

        public void CopyTo(V[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public IEnumerable<INode> GetAllNodes()
        {
            IEnumerable<TopologicalElement> elements = RootTree.GetAllChildElements();
            foreach (TopologicalElement el in elements)
                yield return el;

            //we will also output any nodes not connected to the root node

            foreach (KeyValuePair<K, TopologicalTree> ultimateShrubbery in _brokenTrees)
            {
                elements = ultimateShrubbery.Value.GetAllChildElements();
                foreach (TopologicalElement el in elements)
                    yield return el;
            }
        }

        public IEnumerable<V> Data() => GetAllNodes().Select(x => x.Element);

        public IEnumerator<V> GetEnumerator() => Data().GetEnumerator();

        public bool Remove(V item)
        {
            if (_collection.ContainsItem(item))
            {
                /*TopologicalElement element = _collection.GetElementByItem(item);
                if (element.ParentTreeLevel != null)
                {
                    element.ParentTreeLevel.Remove(element);
                }*/ //will throw an exception if there are children

                return _collection.RemoveByItem(item);
            }
            else
            {
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void CopyTo(Array array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
    }
}
