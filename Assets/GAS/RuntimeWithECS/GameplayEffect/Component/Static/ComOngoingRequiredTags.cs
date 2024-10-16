using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComOngoingRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}