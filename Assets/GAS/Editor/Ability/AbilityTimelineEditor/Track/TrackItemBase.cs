using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackItemBase
    {
        protected abstract string ItemAssetPath { get; }
        protected VisualElement Item;
        
        protected ClipEventBase ClipEventBase;
        protected FrameEventBase FrameEventBase;
        
        public virtual void Init(object data)
        {
            if(data is ClipEventBase clipEventBase)
            {
                ClipEventBase = clipEventBase;
            }
            else if(data is FrameEventBase frameEventBase)
            {
                FrameEventBase = frameEventBase;
            }
            else
            {
                throw new System.Exception("data is not ClipEventBase or FrameEventBase");
            }
            Item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ItemAssetPath).Instantiate().Query().ToList()[1];
        }

        public abstract VisualElement Inspector();
        public abstract void Delete();
    }
}
#endif