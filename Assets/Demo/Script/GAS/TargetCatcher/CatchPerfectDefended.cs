using System.Collections.Generic;
using Demo.Script.GAS.TargetCatcher;
using GAS.Editor;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using GAS.General;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.Script.GAS.TargetCatcher
{
    public class CatchPerfectDefended : CatchUndefending
    {
        /// <summary>
        ///     完美防御,自身收到效果
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <returns></returns>
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var targets = CatchDefaultTargets(mainTarget);

            foreach (var target in targets)
                if (IsDefendSuccess(target) && target.HasTag(GTagLib.Event_PerfectDefending))
                    return new List<AbilitySystemComponent> { Owner };
            return new List<AbilitySystemComponent>();
        }

#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject obj)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            var color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.transform;
            var center = offset;
            var angle = rotation + relativeTransform.eulerAngles.z;
            switch (centerType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? offset.y : -offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? offset.x : -offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }

            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
#endif
    }
#if UNITY_EDITOR
    public class CatchPerfectDefendedInspector : TargetCatcherInspector<CatchPerfectDefended>
    {
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Offset;
        
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Size;
        
        [BoxGroup] [Delayed] [LabelText("Rotation")] [OnValueChanged("OnCatcherChanged")]
        public float Rotation;
        
        [BoxGroup] [Delayed] [LabelText("Detect Layer")] [OnValueChanged("OnCatcherChanged")]
        public LayerMask Layer;
        
        [BoxGroup] [Delayed] [LabelText("Center Type")] [OnValueChanged("OnCatcherChanged")]
        public EffectCenterType CenterType;
        
        public CatchPerfectDefendedInspector(CatchPerfectDefended targetCatcherBase) : base(targetCatcherBase)
        {
            Offset = targetCatcherBase.offset;
            Size = targetCatcherBase.size;
            Rotation = targetCatcherBase.rotation;
            Layer = targetCatcherBase.checkLayer;
            CenterType = targetCatcherBase.centerType;
        }
        
        public void OnCatcherChanged()
        {
            _targetCatcher.offset = Offset;
            _targetCatcher.size = Size;
            _targetCatcher.rotation = Rotation;
            _targetCatcher.checkLayer = Layer;
            _targetCatcher.centerType = CenterType;
            Save();
        }
    }
#endif
}