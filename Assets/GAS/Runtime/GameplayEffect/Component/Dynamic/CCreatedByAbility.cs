using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCreatedByAbility : IComponentData
    {
        public Entity sourceAbility;
    }
}