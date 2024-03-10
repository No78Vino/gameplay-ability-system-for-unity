using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.General.Util;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TargetCatcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public class CatchAreaCircle2DInspector:TargetCatcherInspector<CatchAreaCircle2D>
    {
        public CatchAreaCircle2DInspector(CatchAreaCircle2D targetCatcherBase) : base(targetCatcherBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateSonInspector();
            inspector.Add(TrackInspectorUtil.CreateVector2Field("Offset", _targetCatcher.offset,
                (oldValue,newValue) =>
                {
                    _targetCatcher.offset = newValue;
                    Save();
                }));
            inspector.Add(TrackInspectorUtil.CreateFloatField("Radius", _targetCatcher.radius,
                (evt) =>
                {
                    _targetCatcher.radius = evt.newValue;
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

        public override void OnTargetCatcherPreview(GameObject previewObject)
        {
            // 使用Debug 绘制box预览
            float showTime = 1;
            Color color = Color.green;
            var relativeTransform = AbilityTimelineEditorWindow.Instance.PreviewObject.transform;
            var center = _targetCatcher.offset;
            var radius = _targetCatcher.radius;
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
                    //center = _targetCatcher.Target.transform.position;
                    break;
            }

            DebugExtension.DebugDrawCircle(center, radius, color, showTime);
        }
    }
}