using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public struct CPlayImmunitedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}