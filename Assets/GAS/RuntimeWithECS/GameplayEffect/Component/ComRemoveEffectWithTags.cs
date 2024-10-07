using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComRemoveEffectWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}