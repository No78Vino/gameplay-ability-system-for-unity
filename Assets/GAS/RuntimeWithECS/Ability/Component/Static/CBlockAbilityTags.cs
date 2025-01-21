using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CBlockAbilityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}