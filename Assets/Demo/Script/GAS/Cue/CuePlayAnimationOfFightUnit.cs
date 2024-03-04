using GAS.Runtime.Cue;
using UnityEngine;

namespace GAS.Cue
{
    public class CuePlayAnimationOfFightUnit: GameplayCueInstant
    {
        [SerializeField] private string animName;
        public string AnimName => animName;
        
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CuePlayAnimationOfFightUnitSpec(this, parameters);
        }

        public override void OnEditorPreview(GameObject preview,int frame, int startFrame)
        {
        }
    }
    
    public class CuePlayAnimationOfFightUnitSpec: GameplayCueInstantSpec
    {
        private readonly CuePlayAnimationOfFightUnit _cuePlayAnimationOfFightUnit;
        public CuePlayAnimationOfFightUnitSpec(CuePlayAnimationOfFightUnit cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _cuePlayAnimationOfFightUnit = _cue as CuePlayAnimationOfFightUnit;
        }
        
        public override void Trigger()
        {
            var unit = Owner.GetComponent<FightUnit>();
            unit.Animator.Play(_cuePlayAnimationOfFightUnit.AnimName);
        }
    }
}