using Cysharp.Threading.Tasks;
using GAS.Runtime.Cue;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Cue
{
    public class CueOneShotVFX : GameplayCueInstant
    {
        [BoxGroup]
        [LabelText("特效预制体")]
        [LabelWidth(100)]
        [AssetSelector]
        [SerializeField] 
        private GameObject vfxPrefab;
        public GameObject VfxPrefab => vfxPrefab;

        [BoxGroup]
        [LabelText("是否附着到拥有者")]
        [LabelWidth(100)]
        [SerializeField] 
        private bool isAttachToOwner;
        public bool IsAttachToOwner => isAttachToOwner;
        
        [BoxGroup]
        [LabelText("偏移")]
        [LabelWidth(100)]
        [SerializeField] 
        private Vector3 offset;
        public Vector3 Offset => offset;
        
        [BoxGroup]
        [LabelText("旋转")]
        [LabelWidth(100)]
        [Range(0, 360)]
        [SerializeField] 
        private float rotation;
        public float Rotation => rotation;
        
        [BoxGroup]
        [LabelText("持续时间")]
        [LabelWidth(100)]
        [Unit(Units.Second)]
        [SerializeField] 
        private float duration;
        public float Duration => duration;
        
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueOneShotVFXSpec(this, parameters);
        }
    }

    public class CueOneShotVFXSpec : GameplayCueInstantSpec
    {
        CueOneShotVFX _cueOneShotVfx;
        public CueOneShotVFXSpec(CueOneShotVFX cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _cueOneShotVfx = cue;
        }

        public override void Trigger()
        {
            PlayVfx().Forget();
        }
        
        async UniTask PlayVfx()
        {
            var vfx = Object.Instantiate(_cueOneShotVfx.VfxPrefab);
            if (_cueOneShotVfx.IsAttachToOwner)
            {
                vfx.transform.SetParent(Owner.transform);
                vfx.transform.localPosition = _cueOneShotVfx.Offset;
                vfx.transform.localEulerAngles = new Vector3(0, 0, _cueOneShotVfx.Rotation);
            }
            else
            {
                vfx.transform.position = Owner.transform.position + _cueOneShotVfx.Offset;
                vfx.transform.localEulerAngles = new Vector3(0, 0, _cueOneShotVfx.Rotation);
            }
            await UniTask.Delay((int)(_cueOneShotVfx.Duration * 1000));
            Object.Destroy(vfx);
        }
    }
}