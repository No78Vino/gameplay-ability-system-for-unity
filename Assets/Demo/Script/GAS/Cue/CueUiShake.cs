using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Cue
{
    public class CueUiShake:GameplayCueInstant
    {
        [InfoBox("true = 大幅度震动，false = 小幅度震动")]
        [LabelText("UI振幅小/大")]
        [SerializeField] private bool bigOrSmall;
        
        public bool BigOrSmall => bigOrSmall;
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueUiShakeSpec(this, parameters);
        }
    }

    public class CueUiShakeSpec : GameplayCueInstantSpec
    {
        private readonly CueUiShake _cueUiShake;

        public CueUiShakeSpec(CueUiShake cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _cueUiShake = _cue as CueUiShake;
        }

        public override void Trigger()
        {
            XUI.M.VM<MainUIVM>().UIShake(_cueUiShake.BigOrSmall);
        }
    }
}