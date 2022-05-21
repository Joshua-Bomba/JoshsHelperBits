using JoshsHelperBits.Collections.Mapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoshsHelperBits.Collections.Mapper
{
    public static class MapFactory
    {
        internal static SetMapper<TProp> ResolveMapSet<TProp>(IMappedSet<TProp> set) where TProp : class
        {
            SetMapper<TProp> mappedSet;
            if (set != null && set is SetMapper<TProp> sm)
                mappedSet = sm;
            else
                mappedSet = new SetMapper<TProp>();
            return mappedSet;
        }

        internal static SetMapper<TProp> ResolveEnumerableCollection<TProp>(IEnumerable<TProp> en) where TProp : class
            => new SetMapper<TProp>(en);

        public static IMappedSet<TProp> CreateMappedSet<TProp>(IEnumerable<TProp> en) where TProp : class
            => ResolveEnumerableCollection(en);


        internal static DependencyTree<KProp, TProp> CreateDependencyTree<KProp, TProp>(SetMapper<TProp> mappedSet, Func<TProp, KProp> idSelector, Func<TProp, KProp> parentSelector) where TProp : class
        {
            DependencyMap<KProp, TProp> dm = new DependencyMap<KProp, TProp>(idSelector);
            DependencyTree<KProp, TProp> dp = new DependencyTree<KProp, TProp>(dm, parentSelector);
            mappedSet.AddDepedencyCondition(dm);
            return dp;
        }

        public static IDependencyTree<KProp, TProp> CreateDependencyTree<KProp, TProp>(IMappedSet<TProp> initalCollection, Func<TProp, KProp> idSelector, Func<TProp, KProp> parentSelector) where TProp : class
            =>CreateDependencyTree<KProp, TProp>(ResolveMapSet(initalCollection), idSelector, parentSelector);
        public static IDependencyTree<KProp, TProp> CreateDependencyTree<KProp, TProp>(IEnumerable<TProp> initalCollection, Func<TProp, KProp> idSelector, Func<TProp, KProp> parentSelector) where TProp : class
            => CreateDependencyTree(ResolveEnumerableCollection(initalCollection), idSelector, parentSelector);
        public static IDependencyTree<KProp, TProp> CreateDependencyTree<KProp, TProp>(Func<TProp, KProp> idSelector, Func<TProp, KProp> parentSelector) where TProp : class
            => CreateDependencyTree(new SetMapper<TProp>(), idSelector, parentSelector);

        internal static DependencyTree<KProp,TProp>[] CreateDependencyTrees<KProp,TProp>(SetMapper<TProp> mappedSet, Func<TProp, KProp> idSelector, params Func<TProp, KProp>[] parentSelectors) where TProp : class
        {
            DependencyTree<KProp, TProp>[] trees = new DependencyTree<KProp, TProp>[parentSelectors.Length];
            DependencyMap<KProp, TProp> dm = new DependencyMap<KProp, TProp>(idSelector);

            for (int i = 0; i < parentSelectors.Length; i++) 
            { 
                trees[i] = new DependencyTree<KProp, TProp>(dm,parentSelectors[i]);
            }
            mappedSet.AddDepedencyCondition(dm);
            return trees;
        }

        public static IDependencyTree<KProp, TProp>[] CreateDependencyTrees<KProp, TProp>(IMappedSet<TProp> initalCollection, Func<TProp, KProp> idSelector, params Func<TProp, KProp>[] parentSelectors) where TProp : class
            => CreateDependencyTrees<KProp, TProp>(ResolveMapSet(initalCollection), idSelector, parentSelectors);
        public static IDependencyTree<KProp, TProp>[] CreateDependencyTrees<KProp, TProp>(IEnumerable<TProp> initalCollection, Func<TProp, KProp> idSelector, params Func<TProp, KProp>[] parentSelectors) where TProp : class
            => CreateDependencyTrees<KProp, TProp>(ResolveEnumerableCollection(initalCollection), idSelector, parentSelectors);
        public static IDependencyTree<KProp, TProp>[] CreateDependencyTrees<KProp, TProp>(Func<TProp, KProp> idSelector, params Func<TProp, KProp>[] parentSelectors) where TProp : class
            => CreateDependencyTrees<KProp, TProp>(new SetMapper<TProp>(), idSelector, parentSelectors);


    }
}
