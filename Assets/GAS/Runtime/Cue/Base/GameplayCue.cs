using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCue : ScriptableObject
    {
        public abstract GameplayCueSpec CreateSpec(GameplayEffectSpec sourceGameplayEffectSpec);
    }
}