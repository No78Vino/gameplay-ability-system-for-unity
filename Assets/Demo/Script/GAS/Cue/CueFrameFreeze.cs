using Cysharp.Threading.Tasks;
using GAS.Runtime.Cue;
using UnityEngine;

namespace GAS.Cue
{
    public class CueFrameFreeze:GameplayCueInstant
    {
        [SerializeField] private int frameCount;
        public int FrameCount => frameCount;
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueFrameFreezeSpec(this, parameters);
        }
    }
    
    public class CueFrameFreezeSpec : GameplayCueInstantSpec
    {
        CueFrameFreeze _cueFrameFreeze;
        FightUnit _unit;
        public CueFrameFreezeSpec(CueFrameFreeze cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _cueFrameFreeze = cue;
            _unit = Owner.GetComponent<FightUnit>();
        }

        public override void Trigger()
        {
            DoFrameFreeze().Forget();
        }

        async UniTask DoFrameFreeze()
        {
            Time.timeScale = 0;
            await UniTask.DelayFrame(_cueFrameFreeze.FrameCount);
            Time.timeScale = 1;
        }
    }
}