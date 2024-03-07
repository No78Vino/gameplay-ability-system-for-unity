using System.Collections.Generic;
using System.Linq;
using GAS.Runtime.Cue;
using UnityEditor.Animations;
using UnityEngine;
using GAS.General.Util;

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
            var unit = preview.GetComponent<FightUnit>();
            if (startFrame <= frame)
            {
                var animatorObject = unit.Animator.gameObject;
                var animator = unit.Animator;
                var stateMap = animator.GetAllAnimationState(0);
                if(stateMap.TryGetValue(animName, out var clip))
                {
                    float clipFrameCount = (int)(clip.frameRate * clip.length);
                    if (frame < clipFrameCount + startFrame)
                    {
                        var progress = (frame - startFrame) / clipFrameCount;
                        if (progress > 1 && clip.isLooping) progress -= (int)progress;
                        clip.SampleAnimation(animatorObject.gameObject, progress * clip.length);
                    }
                }
            }
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