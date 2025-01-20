using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    [InternalBufferCapacity(GASParameterSetting.ASC_MAX_GAMEPLAY_EFFECT_COUNT)]
    public struct BEGameplayEffect : IBufferElementData
    {
        public Entity GameplayEffect;
    }
}