using UnityEngine;

namespace GAS.Runtime
{
    public class CueLogging: GameplayCueBase<XParamLogging>
    {
        private float _startTime;
        
        public override void OnActivate(float time)
        {
            base.OnActivate(time);
            _startTime = time;
            Debug.Log($"CueLogging activated. Value:{Parameter.Value}");
        }

        public override void OnTick(float time)
        {
            base.OnTick(time);
            Debug.Log($"CueLogging tick at time {time}. Value:{Parameter.Value}");

            if (!(time - _startTime > Parameter.Duration)) return;
            RemoveSelf();
            KillSelf();
        }
    }
}