using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CueVFX: GameplayCue
    {
        [SerializeField] GameObject _vfx;
        
        public override void Init(AbilitySystemComponent source)
        {
            base.Init(source);
        }
        
        public override void Trigger()
        {
            if (_vfx != null)
            {
                Instantiate(_vfx, _source.transform);
            }
        }
    }
}