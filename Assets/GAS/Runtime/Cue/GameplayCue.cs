using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCue:ScriptableObject
    {
        protected GameplayEffectSpec gameplayEffectSpec;

        protected virtual void Init(GameplayEffectSpec sourceGameplayEffectSpec)
        {
            gameplayEffectSpec = sourceGameplayEffectSpec;
        }

        public virtual void Trigger(GameplayEffectSpec sourceGameplayEffectSpec)
        {
            Init(sourceGameplayEffectSpec);
        }
    }
}