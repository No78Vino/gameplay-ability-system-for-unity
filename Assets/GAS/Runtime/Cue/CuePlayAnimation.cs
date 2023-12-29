using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CuePlayAnimation : GameplayCue
    {
        [SerializeField] private string _animationName;
        private Animator _animator;

        public override void Init(AbilitySystemComponent source)
        {
            base.Init(source);
            _animator = source.GetComponent<Animator>();
        }

        public override void Trigger()
        {
            _animator.Play(_animationName);
        }
    }
}