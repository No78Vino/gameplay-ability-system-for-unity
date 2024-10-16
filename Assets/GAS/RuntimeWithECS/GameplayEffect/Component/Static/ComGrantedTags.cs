using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComGrantedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}