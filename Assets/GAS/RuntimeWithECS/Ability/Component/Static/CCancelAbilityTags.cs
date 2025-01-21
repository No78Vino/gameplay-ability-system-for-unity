using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CCancelAbilityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}