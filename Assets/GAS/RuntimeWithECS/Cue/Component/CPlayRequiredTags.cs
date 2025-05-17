using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CPlayRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}