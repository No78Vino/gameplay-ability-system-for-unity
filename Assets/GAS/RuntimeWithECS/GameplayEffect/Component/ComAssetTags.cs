using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComAssetTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}