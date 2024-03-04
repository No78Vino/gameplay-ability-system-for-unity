using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public enum InstantCueApplyTarget
    {
        Owner,
        Source
    }

    public abstract class GameplayCueInstant : GameplayCue<GameplayCueInstantSpec>
    {
        public InstantCueApplyTarget applyTarget;

        public virtual void ApplyFrom(GameplayEffectSpec gameplayEffectSpec)
        {
            if (Triggerable(gameplayEffectSpec.Owner))
            {
                var instantCue = CreateSpec(new GameplayCueParameters
                    { sourceGameplayEffectSpec = gameplayEffectSpec });
                instantCue?.Trigger();
            }
        }

        public virtual void ApplyFrom(AbilitySpec abilitySpec,params object[] customArguments)
        {
            if (Triggerable(abilitySpec.Owner))
            {
                var instantCue = CreateSpec(new GameplayCueParameters
                    { sourceAbilitySpec = abilitySpec, customArguments = customArguments});
                instantCue?.Trigger();
            }
        }
        
        public virtual void OnEditorPreview(GameObject previewObject,int frame,int startFrame)
        {
        }
    }

    public abstract class GameplayCueInstantSpec : GameplayCueSpec
    {
        public GameplayCueInstantSpec(GameplayCueInstant cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
        }
        
        private GameplayCueInstant instantCue => _cue as GameplayCueInstant;

        public abstract void Trigger();
    }
}