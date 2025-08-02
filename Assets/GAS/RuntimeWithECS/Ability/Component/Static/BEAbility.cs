using GAS.Runtime;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Static
{
    [InternalBufferCapacity(GASParameterSetting.ASC_MAX_ABILITY_COUNT)]
    public struct BEAbility : IBufferElementData
    {
        public Entity Ability;
    }
}