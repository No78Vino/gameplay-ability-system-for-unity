using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityAssetTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}