using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT.TestData
{

    public class TTDInt : TopologicalTestData<int> { }

    public class TTDGuid : TopologicalTestData<Guid> { }


    public class TopologicalTestData<T> where T: struct
    {
        public T Id { get; set; }

        public Nullable<T> DepId1 { get; set; }

        public Nullable<T> DepId2 { get; set; }

        public string? Content { get; set; }

        public override string ToString()
        {
            return $"Id:{Id}, DepId1:{DepId1}, DepId2: {DepId2}, Context: {Content}";
        }
    }
}
