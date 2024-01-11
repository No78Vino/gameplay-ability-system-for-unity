using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CueVFX : GameplayCue
    {
        [SerializeField] private GameObject _vfx;

        protected override void Init(GameplayEffectSpec source)
        {
            base.Init(source);
        }

        public override void Trigger(GameplayEffectSpec source)
        {
            base.Trigger(source);
            if (_vfx != null) Instantiate(_vfx, gameplayEffectSpec.Owner.transform);
        }
    }
}