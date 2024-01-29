using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCue : ScriptableObject
    {
        //public abstract GameplayCueSpec CreateSpec(GameplayEffectSpec sourceGameplayEffectSpec);
    }
    
    public abstract class GameplayCue<T> : GameplayCue where T : GameplayCueSpec
    {
        public abstract T CreateSpec(GameplayCueParameters parameters);
        
        public GameplayCueSpec CreateSpecReturnBase(GameplayCueParameters parameters)
        {
            return CreateSpec(parameters);
        }
    }
}