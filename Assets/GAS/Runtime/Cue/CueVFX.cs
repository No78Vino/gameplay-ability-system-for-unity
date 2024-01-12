using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CueVFX : GameplayCueDurational
    {
        [SerializeField] public GameObject VfxPrefab;

        public override GameplayCueSpec CreateSpec(GameplayEffectSpec sourceGameplayEffectSpec)
        {
            return new CueVFXSpec(this, sourceGameplayEffectSpec);
        }
    }

    public class CueVFXSpec : GameplayCueDurationalSpec
    {
        private GameObject _vfxInstance;

        public CueVFXSpec(GameplayCue cue, GameplayEffectSpec sourceGameplayEffectSpec) : base(cue,
            sourceGameplayEffectSpec)
        {
        }

        private CueVFX cue => _cue as CueVFX;

        public override void OnGameplayEffectAdd()
        {
            _vfxInstance = Object.Instantiate(cue.VfxPrefab, _targetASC.transform);
        }

        public override void OnGameplayEffectRemove()
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
    }
}