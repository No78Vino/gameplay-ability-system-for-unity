using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueSpec
    {
        protected readonly GameplayCue _cue;
        protected readonly GameplayCueParameters _parameters;

        public GameplayCueSpec(GameplayCue cue, GameplayCueParameters cueParameters)
        {
            _cue = cue;
            _parameters = cueParameters;
        }
    }
}