using GAS.Runtime;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public struct GameplayCueParameters
    {
        public GameplayEffectSpec sourceGameplayEffectSpec;
        
        public AbilitySpec sourceAbilitySpec;
        
        public object[] customArguments;
        // AggregatedSourceTags
        // AggregatedTargetTags
        // EffectContext
        // Magnitude
    }
}