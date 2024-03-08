using System.Linq;
using GAS.General.Util;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CueAnimationOneShot : GameplayCueInstant
    {
        [BoxGroup]
        [InfoBox("为空表示物体根节点,示例：'A'=根节点下名为'A'的子物体,'A/B'='A'节点下名为'B'的子物体")]
        [LabelText("动画机相对路径")]
        [SerializeField] private string _animatorRelativePath;
        public string AnimatorRelativePath => _animatorRelativePath;
        
        [BoxGroup]
        [LabelText("Animation State名")]
        [SerializeField] private string _stateName;
        public string StateName => _stateName;
        
        
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueAnimationOneShotSpec(this, parameters);
        }

        public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame)
        {
            if (startFrame <= frame)
            {
                var animatorObject = previewObject.transform.Find(AnimatorRelativePath);
                var animator = animatorObject.GetComponent<Animator>();
                var stateMap = animator.GetAllAnimationState(0);
                if(stateMap.TryGetValue(StateName, out var clip))
                {
                    float clipFrameCount = (int)(clip.frameRate * clip.length);
                    if (frame <= clipFrameCount + startFrame)
                    {
                        var progress = (frame - startFrame) / clipFrameCount;
                        if (progress > 1 && clip.isLooping) progress -= (int)progress;
                        clip.SampleAnimation(animatorObject.gameObject, progress * clip.length);
                    }
                }
            }
        }
    }
    
    public class CueAnimationOneShotSpec : GameplayCueInstantSpec
    {
        private CueAnimationOneShot cue => _cue as CueAnimationOneShot;
        private readonly Animator _animator;
        
        public CueAnimationOneShotSpec(CueAnimationOneShot cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            var animatorTransform = Owner.transform.Find(cue.AnimatorRelativePath);
            _animator = animatorTransform.GetComponent<Animator>();
        }

        public override void Trigger()
        {
            _animator.Play(cue.StateName);
        }
    }
}