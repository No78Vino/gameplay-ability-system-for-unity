using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
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
        public virtual void OnEditorPreviewTick(int frameIndex,int startFrame,int endFrame)
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
}