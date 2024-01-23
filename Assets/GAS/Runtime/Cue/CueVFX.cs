using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CueVFX : GameplayCueDurational
    {
        [SerializeField] public GameObject VfxPrefab;
        [SerializeField] public bool IsAttachToTarget = true;

        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueVFXSpec(this, parameters);
        }
    }

    public class CueVFXSpec : GameplayCueDurationalSpec
    {
        private GameObject _vfxInstance;

        public CueVFXSpec(CueVFX cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
        }

        private CueVFX cue => _cue as CueVFX;

        public override void OnAdd()
        {
            _vfxInstance = cue.IsAttachToTarget ? 
                Object.Instantiate(cue.VfxPrefab, Owner.transform) : 
                Object.Instantiate(cue.VfxPrefab, Owner.transform.position, Quaternion.identity);
        }

        public override void OnRemove()
        {
            Object.Destroy(_vfxInstance);
        }

        public override void OnGameplayEffectActivate()
        {
            _vfxInstance.SetActive(true);
        }

        public override void OnGameplayEffectDeactivate()
        {
            _vfxInstance.SetActive(false);
        }

        public override void OnTick()
        {
        }
        
        public void SetVisible(bool visible)
        {
            _vfxInstance?.SetActive(visible);
        }
    }
}