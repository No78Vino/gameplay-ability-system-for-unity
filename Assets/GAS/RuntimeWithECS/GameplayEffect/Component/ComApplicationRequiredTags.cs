using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComApplicationRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}