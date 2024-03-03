using GAS.Runtime.Ability.TargetCatcher;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public class CatchAreaBox2DInspector:TargetCatcherInspector<CatchAreaBox2D>
    {
        public CatchAreaBox2DInspector(CatchAreaBox2D targetCatcherBase) : base(targetCatcherBase)
        {
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTargetCatcherInspector();
            inspector.Add(TrackInspectorUtil.CreateVector2Field("Offset", _targetCatcher.offset,
                (evt) =>
                {
                    _targetCatcher.offset = evt.newValue;
                    Save();
                }));
            inspector.Add(TrackInspectorUtil.CreateVector2Field("Size", _targetCatcher.size,
                (evt) =>
                {
                    _targetCatcher.size = evt.newValue;
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
            return inspector;
        }
    }
}