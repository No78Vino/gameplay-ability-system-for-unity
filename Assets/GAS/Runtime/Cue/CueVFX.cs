using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public class CueVFX : GameplayCueDurational
    {
        [BoxGroup] [LabelText(GASTextDefine.CUE_VFX_PREFAB)]
        public GameObject VfxPrefab;

        [BoxGroup] [LabelText(GASTextDefine.CUE_ATTACH_TO_OWNER)]
        public bool IsAttachToTarget = true;

        [BoxGroup] [LabelText(GASTextDefine.CUE_VFX_OFFSET)]
        public Vector3 Offset;

        [BoxGroup] [LabelText(GASTextDefine.CUE_VFX_ROTATION)]
        public Vector3 Rotation;
        
        [BoxGroup] [LabelText(GASTextDefine.CUE_VFX_SCALE)]
        public Vector3 Scale = Vector3.one;
        
        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueVFXSpec(this, parameters);
        }

#if UNITY_EDITOR
        private GameObject _effectPreviewInstance;
        public override void OnEditorPreview(GameObject preview,int frameIndex, int startFrame, int endFrame)
        {
            if (VfxPrefab == null) return;
            if(frameIndex>=startFrame && frameIndex <= endFrame)
            {
                if (_effectPreviewInstance != null && _effectPreviewInstance.name != VfxPrefab.name)
                {
                    DestroyImmediate(_effectPreviewInstance);
                    _effectPreviewInstance= null;
                }

                if (_effectPreviewInstance == null)
                {
                    _effectPreviewInstance = Instantiate(VfxPrefab, preview.transform);
                    _effectPreviewInstance.name = VfxPrefab.name;
                    _effectPreviewInstance.transform.localPosition = Offset;
                    _effectPreviewInstance.transform.localEulerAngles = Rotation;
                    _effectPreviewInstance.transform.localScale = Scale;
                }
                // 模拟例子的播放
                var particleSystems = _effectPreviewInstance.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in particleSystems)
                {
                    var t = (frameIndex - startFrame) / GASTimer.FrameRate;
                    ps.Simulate(t);
                }
            }
            else
            {
                if (_effectPreviewInstance != null)
                {
                    DestroyImmediate(_effectPreviewInstance);
                    _effectPreviewInstance = null;
                }
            }
        }
#endif
    }

    public class CueVFXSpec : GameplayCueDurationalSpec<CueVFX>
    {
        private GameObject _vfxInstance;

        public CueVFXSpec(CueVFX cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
        }
        
        public override void OnAdd()
        {
            _vfxInstance = cue.IsAttachToTarget
                ? Object.Instantiate(cue.VfxPrefab, Owner.transform)
                : Object.Instantiate(cue.VfxPrefab, Owner.transform.position, Quaternion.identity);

            _vfxInstance.transform.localPosition = cue.Offset;
            _vfxInstance.transform.localEulerAngles = cue.Rotation;
            _vfxInstance.transform.localScale = cue.Scale;
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