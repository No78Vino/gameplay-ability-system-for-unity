using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public class CueAnimation : GameplayCueDurational
    {
        [BoxGroup]
        [InfoBox(GASTextDefine.CUE_ANIMATION_PATH_TIP)]
        [LabelText(GASTextDefine.CUE_ANIMATION_PATH)]
        [SerializeField]
        private string _animatorRelativePath;

        [BoxGroup]
        [InfoBox(GASTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN_ANIMATOR_TIP)]
        [LabelText(GASTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN)]
        [SerializeField]
        private bool _includeChildrenAnimator;

        [BoxGroup]
        [LabelText(GASTextDefine.CUE_ANIMATION_STATE)]
        [SerializeField]
        private string _stateName;

        public string AnimatorRelativePath => _animatorRelativePath;
        public bool IncludeChildrenAnimator => _includeChildrenAnimator;
        public string StateName => _stateName;


        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueAnimationSpec(this, parameters);
        }

#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame, int endFrame)
        {
            if (startFrame <= frame && frame <= endFrame)
            {
                var transform = previewObject.transform.Find(AnimatorRelativePath);
                var animator = IncludeChildrenAnimator ? transform.GetComponentInChildren<Animator>() : transform.GetComponent<Animator>();
                if (animator != null)
                {
                    var stateMap = animator.GetAllAnimationState();
                    if (stateMap.TryGetValue(StateName, out var clip))
                    {
                        float clipFrameCount = (int)(clip.frameRate * clip.length);
                        if (frame <= clipFrameCount + startFrame)
                        {
                            var progress = (frame - startFrame) / clipFrameCount;
                            if (progress > 1 && clip.isLooping) progress -= (int)progress;
                            clip.SampleAnimation(animator.gameObject, progress * clip.length);
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Animator is null. Please check the cue asset: {name}, AnimatorRelativePath: {AnimatorRelativePath}, IncludeChildrenAnimator: {IncludeChildrenAnimator}");
                }
            }
        }
#endif
    }

    public class CueAnimationSpec : GameplayCueDurationalSpec<CueAnimation>
    {
        private readonly Animator _animator;

        public CueAnimationSpec(CueAnimation cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            var transform = Owner.transform.Find(cue.AnimatorRelativePath);
            _animator = cue.IncludeChildrenAnimator ? transform.GetComponentInChildren<Animator>() : transform.GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError($"Animator is null. Please check the cue asset: {cue.name}, AnimatorRelativePath: {cue.AnimatorRelativePath}, IncludeChildrenAnimator: {cue.IncludeChildrenAnimator}");
            }
        }

        public override void OnAdd()
        {
            if (_animator != null)
            {
                _animator.Play(cue.StateName);
            }
        }

        public override void OnRemove()
        {
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
        }
    }
}