using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CCreatedByAbility : IComponentData
    {
        public Entity sourceAbility;
    }
}