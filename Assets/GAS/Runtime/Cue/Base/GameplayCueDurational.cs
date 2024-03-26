using UnityEngine;

namespace GAS.Runtime
{
    public abstract class GameplayCueDurational : GameplayCue<GameplayCueDurationalSpec>
    {
        public GameplayCueDurationalSpec ApplyFrom(GameplayEffectSpec gameplayEffectSpec)
        {
            if (!Triggerable(gameplayEffectSpec.Owner)) return null;
            var durationalCue = CreateSpec(new GameplayCueParameters
                { sourceGameplayEffectSpec = gameplayEffectSpec });
            return durationalCue;
        }
        
        public GameplayCueDurationalSpec ApplyFrom(AbilitySpec abilitySpec,params object[] customArguments)
        {
            if (!Triggerable(abilitySpec.Owner)) return null;
            var durationalCue = CreateSpec(new GameplayCueParameters
                { sourceAbilitySpec = abilitySpec, customArguments = customArguments});
            return durationalCue;
        }
        
#if UNITY_EDITOR
        public virtual void OnEditorPreview(GameObject previewObject,int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
    }

    public abstract class GameplayCueDurationalSpec : GameplayCueSpec
    {
        protected GameplayCueDurationalSpec(GameplayCueDurational cue, GameplayCueParameters parameters) : 
            base(cue, parameters)
        {
        }

        public abstract void OnAdd();
        public abstract void OnRemove();
        public abstract void OnGameplayEffectActivate();
        public abstract void OnGameplayEffectDeactivate();
        public abstract void OnTick();
    }

    public abstract class GameplayCueDurationalSpec<T> : GameplayCueDurationalSpec where T : GameplayCueDurational
    {
        public readonly T cue;

        protected GameplayCueDurationalSpec(T cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            this.cue = cue;
        }
    }
}