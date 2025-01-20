using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    [InternalBufferCapacity(GASParameterSetting.ASC_MAX_ABILITY_COUNT)]
    public struct BEAbility : IBufferElementData
    {
        public Entity Ability;
    }
}