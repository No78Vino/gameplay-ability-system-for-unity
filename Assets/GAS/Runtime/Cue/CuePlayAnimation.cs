using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CuePlayAnimation : GameplayCueInstant
    {
        [SerializeField] private string _animationName;
        public string AnimationName => _animationName;
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CuePlayAnimationSpec(this, parameters);
        }
    }
    
    public class CuePlayAnimationSpec : GameplayCueInstantSpec
    {
        private CuePlayAnimation cue => _cue as CuePlayAnimation;
        private readonly Animator _animator;
        
        public CuePlayAnimationSpec(CuePlayAnimation cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            _animator = _targetASC.GetComponent<Animator>();
        }

        public override void Trigger()
        {
            _animator.Play(cue.AnimationName);
        }
    }
}