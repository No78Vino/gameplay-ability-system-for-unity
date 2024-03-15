
#if UNITY_EDITOR
namespace GAS.Editor
{
    using Editor;
    using GAS.General;
    using Runtime;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Sirenix.OdinInspector;

    public class CatchAreaCircle2DInspector : TargetCatcherInspector<CatchAreaCircle2D>
    {
        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public Vector2 Offset;

        [BoxGroup] [Delayed] [OnValueChanged("OnCatcherChanged")]
        public float Radius;

        [BoxGroup] [Delayed] [LabelText("Detect Layer")] [OnValueChanged("OnCatcherChanged")]
        public LayerMask Layer;

        [BoxGroup] [Delayed] [LabelText("Center Type")] [OnValueChanged("OnCatcherChanged")]
        public EffectCenterType CenterType;

        public CatchAreaCircle2DInspector(CatchAreaCircle2D targetCatcherBase) : base(targetCatcherBase)
        {
            Offset = targetCatcherBase.offset;
            Radius = targetCatcherBase.radius;
            Layer = targetCatcherBase.checkLayer;
            CenterType = targetCatcherBase.centerType;
        }

        void OnCatcherChanged()
        {
            _targetCatcher.offset = Offset;
            _targetCatcher.radius = Radius;
            _targetCatcher.checkLayer = Layer;
            _targetCatcher.centerType = CenterType;
            Save();
        }

        // public override VisualElement Inspector()
        // {
        //     var inspector = TrackInspectorUtil.CreateSonInspector();
        //     inspector.Add(TrackInspectorUtil.CreateVector2Field("Offset", _targetCatcher.offset,
        //         (oldValue,newValue) =>
        //         {
        //             _targetCatcher.offset = newValue;
        //             Save();
        //         }));
        //     inspector.Add(TrackInspectorUtil.CreateFloatField("Radius", _targetCatcher.radius,
        //         (evt) =>
        //         {
        //             _targetCatcher.radius = evt.newValue;
        //             Save();
        //         }));
        //     inspector.Add(TrackInspectorUtil.CreateLayerMaskField("Layer", _targetCatcher.checkLayer,
        //         (evt) =>
        //         {
        //             _targetCatcher.checkLayer = evt.newValue;
        //             Save();
        //         }));
        //     
        //     var centerType = TrackInspectorUtil.CreateEnumField("CenterType", _targetCatcher.centerType, (evt) =>
        //     {
        //         _targetCatcher.centerType = (EffectCenterType)(evt.newValue);
        //         Save();
        //     });
        //     inspector.Add(centerType);
        //     
        //     return inspector;
        // }
    }
}
#endif