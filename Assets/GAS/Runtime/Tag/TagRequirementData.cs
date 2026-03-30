using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct TagRequirementData
    {
        public NativeArray<int> all;
        public NativeArray<int> any;
        public NativeArray<int> none;
    }
}
