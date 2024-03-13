using System.Collections.Generic;
#if UNITY_EDITOR
using GAS.Editor.Ability;
using GAS.Editor.Ability.AbilityTimelineEditor;
#endif
using GAS.General.Util;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.UIElements;

namespace Demo.Script.GAS.TargetCatcher
{
    public class CatchUnperfectDefend : CatchUndefending
    {
        /// <summary>
        /// 成功防御，但是没有完美防御
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <returns></returns>
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            var targets = CatchDefaultTargets(mainTarget);

            var result = new List<AbilitySystemComponent>();
            foreach (var target in targets)
                if (IsDefendSuccess(target) && !target.HasTag(GTagLib.Event_PerfectDefending))
                    result.Add(target);
            return result;
        }
    }
#if UNITY_EDITOR
    public class CatchUnperfectDefendInspector : TargetCatcherInspector<CatchUnperfectDefend>
    {
        public CatchUnperfectDefendInspector(CatchUnperfectDefend targetCatcherBase) : base(targetCatcherBase)
        {

        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateSonInspector();
            inspector.Add(TrackInspectorUtil.CreateVector2Field("Offset", _targetCatcher.offset,
                (oldValue, newValue) =>
                {
                    _targetCatcher.offset = newValue;
                    Save();
                }));
            inspector.Add(TrackInspectorUtil.CreateVector2Field("Size", _targetCatcher.size,
                (oldValue, newValue) =>
                {
                    _targetCatcher.size = newValue;
                    Save();
                }));
            inspector.Add(TrackInspectorUtil.CreateFloatField("Rotation", _targetCatcher.rotation,
                (evt) =>
                {
                    _targetCatcher.rotation = evt.newValue;
                    Save();
                }));
            inspector.Add(TrackInspectorUtil.CreateLayerMaskField("Layer", _targetCatcher.checkLayer,
                (evt) =>
                {
                    _targetCatcher.checkLayer = evt.newValue;
                    Save();
                }));

            var centerType = TrackInspectorUtil.CreateEnumField("CenterType", _targetCatcher.centerType, (evt) =>
            {
                _targetCatcher.centerType = (EffectCenterType)(evt.newValue);
                Save();
            });
            inspector.Add(centerType);

            return inspector;
        }

        public override void OnTargetCatcherPreview(GameObject obj)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.transform;
            var center = _targetCatcher.offset;
            var size = _targetCatcher.size;
            var angle = _targetCatcher.rotation + relativeTransform.eulerAngles.z;
            switch (_targetCatcher.centerType)
            {
                case EffectCenterType.SelfOffset:
                    center = relativeTransform.position;
                    center.y += relativeTransform.lossyScale.y > 0 ? _targetCatcher.offset.y : -_targetCatcher.offset.y;
                    center.x += relativeTransform.lossyScale.x > 0 ? _targetCatcher.offset.x : -_targetCatcher.offset.x;
                    break;
                case EffectCenterType.WorldSpace:
                    center = _targetCatcher.offset;
                    break;
                case EffectCenterType.TargetOffset:
                    //center = _spec.Target.transform.position + (Vector3)_task.Offset;
                    break;
            }

            DebugExtension.DebugBox(center, size, angle, color, showTime);
        }
    }
#endif
}