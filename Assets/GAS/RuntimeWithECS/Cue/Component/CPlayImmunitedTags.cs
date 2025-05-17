using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CPlayImmunitedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}