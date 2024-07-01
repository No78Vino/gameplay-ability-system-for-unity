using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public sealed class CueAnimationSpeedModifier : GameplayCueDurational
    {
        const int LabelWidth = 120;

        [TabGroup("Data", "Data", SdfIconType.Gear, TextColor = "#FF7F00")]
        [InfoBox(GASTextDefine.CUE_ANIMATION_PATH_TIP, InfoMessageType.None)]
        [LabelText(GASTextDefine.CUE_ANIMATION_PATH), LabelWidth(LabelWidth)]
        public string animatorRelativePath;

        [TabGroup("Data", "Data")]
        [InfoBox(GASTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN_ANIMATOR_TIP, InfoMessageType.None)]
        [LabelText(GASTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN), LabelWidth(LabelWidth)]
        public bool includeChildrenAnimator;

        [TabGroup("Data", "Data")]
        [LabelText("播放速度"), LabelWidth(LabelWidth)]
        [Range(0, 5f)]
        public float speed = 1f;

        [TabGroup("Data", "Data")]
        [InfoBox("结束时会设置的值, 如果有其它需求, 需要另外实现. ^_^", InfoMessageType.None)]
        [LabelText("默认播放速度"), LabelWidth(LabelWidth)]
        [Range(0, 5f)]
        public float defaultSpeed = 1f;

        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new GCS_ChangeAnimationSpeed(this, parameters);
        }
    }


    public sealed class GCS_ChangeAnimationSpeed : GameplayCueDurationalSpec<CueAnimationSpeedModifier>
    {
        private readonly Animator _animator;

        public GCS_ChangeAnimationSpeed(CueAnimationSpeedModifier cue, GameplayCueParameters parameters)
            : base(cue, parameters)
        {
            var transform = Owner.transform.Find(cue.animatorRelativePath);
            if (transform != null)
            {
                _animator = cue.includeChildrenAnimator
                    ? transform.GetComponentInChildren<Animator>()
                    : transform.GetComponent<Animator>();
            }

            if (_animator == null)
            {
                Debug.LogError(
                    $"Animator is null. Please check the cue asset: {cue.name}, AnimatorRelativePath: {cue.animatorRelativePath}, IncludeChildrenAnimator: {cue.includeChildrenAnimator}");
            }
        }

        public override void OnAdd()
        {
        }

        public override void OnRemove()
        {
        }

        public override void OnGameplayEffectActivate()
        {
            if (_animator != null)
            {
                _animator.speed = cue.speed;
            }
        }

        public override void OnGameplayEffectDeactivate()
        {
            if (_animator != null)
            {
                _animator.speed = cue.defaultSpeed;
            }
        }

        public override void OnTick()
        {
        }
    }
}