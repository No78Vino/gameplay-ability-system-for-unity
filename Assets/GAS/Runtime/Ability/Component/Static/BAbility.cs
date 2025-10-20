using Unity.Entities;

namespace GAS.Runtime
{
    [InternalBufferCapacity(GASParameterSetting.ASC_MAX_ABILITY_COUNT)]
    public struct BAbility : IBufferElementData
    {
        public Entity Ability;
    }
}