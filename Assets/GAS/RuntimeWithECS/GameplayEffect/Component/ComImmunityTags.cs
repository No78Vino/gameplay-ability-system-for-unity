using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComImmunityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}