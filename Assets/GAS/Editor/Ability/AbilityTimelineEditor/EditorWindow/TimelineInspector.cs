#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEngine.UIElements;
    public class TimelineInspector
    {
        private VisualElement _root;
        public object CurrentInspectorObject;
        public TimelineInspector(VisualElement root)
        {
            _root = root;
            SetInspector();
        }

        public void SetInspector(object target=null)
        {
            UpdateInspector(false,target);
        }

        private void UpdateInspector(bool force = false, object target = null)
        {
            if (CurrentInspectorObject == target && !force) return;

            if (CurrentInspectorObject != null && !force)
            {
                switch (CurrentInspectorObject)
                {
                    case TaskClip oldTrackItem:
                        oldTrackItem.ClipVe.OnUnSelect();
                        break;
                    case AbilityTimelineTrack oldTrack:
                        oldTrack.OnUnSelect();
                        break;
                }
            }

            CurrentInspectorObject = target;
            switch (CurrentInspectorObject)
            {
                case null:
                    UnityEditor.Selection.activeObject = null;
                    return;
                case TaskClip trackClip:
                    UnityEditor.Selection.activeObject = trackClip.DataInspector;
                    break;
                case AbilityTimelineTrack track:
                    UnityEditor.Selection.activeObject = track.DataInspector;
                    break;
            }
        }
        
        public void RefreshInspector()
        {
            UpdateInspector(true,CurrentInspectorObject);
        }
    }
}
#endif