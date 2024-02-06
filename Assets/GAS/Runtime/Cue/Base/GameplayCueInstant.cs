using GAS.Runtime.Component;
using GAS.Runtime.Effects;

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