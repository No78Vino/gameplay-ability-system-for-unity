using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CueVFX: GameplayCue
    {
        [SerializeField] GameObject _vfx;

        protected override void Init(AbilitySystemComponent source)
        {
            base.Init(source);
        }
        
        public override void Trigger(AbilitySystemComponent source)
        {
            base.Trigger(source);
            if (_vfx != null)
            {
                Instantiate(_vfx, _source.transform);
            }
        }
    }
}