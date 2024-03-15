

#if UNITY_EDITOR
namespace GAS.Editor
{
    using Editor;
    using GAS.General;
    using Runtime;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Sirenix.OdinInspector;
    
    public class CatchAreaBox2DInspector:TargetCatcherInspector<CatchAreaBox2D>
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
        
        public CatchAreaBox2DInspector(CatchAreaBox2D targetCatcherBase) : base(targetCatcherBase)
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
}
#endif