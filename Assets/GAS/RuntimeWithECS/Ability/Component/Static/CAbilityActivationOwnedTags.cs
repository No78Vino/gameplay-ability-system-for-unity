using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityActivationOwnedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
}