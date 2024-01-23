using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueDurational : GameplayCue<GameplayCueDurationalSpec>
    {
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