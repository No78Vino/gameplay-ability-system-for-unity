using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CuePlayAnimation : GameplayCue
    {
        [SerializeField] private string _animationName;
        private Animator _animator;

        protected override void Init(GameplayEffectSpec source)
        {
            base.Init(source);
            _animator = gameplayEffectSpec.Owner.GetComponent<Animator>();
        }

        public override void Trigger(GameplayEffectSpec source)
        {
            base.Trigger(source);
            _animator.Play(_animationName);
        }
    }
}