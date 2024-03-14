#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    using UnityEngine.UIElements;
    public class TimelineInspector
    {
        private VisualElement _root;
        private VisualElement _clipInspector;
        public object CurrentInspectorObject;
        public TimelineInspector(VisualElement root)
        {
            _root = root;
            _clipInspector = _root.Q<VisualElement>("ClipInspector");
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
                if (CurrentInspectorObject is TrackClipBase oldTrackItem) oldTrackItem.ClipVe.OnUnSelect();
                if (CurrentInspectorObject is TrackBase oldTrack) oldTrack.OnUnSelect();
                if (CurrentInspectorObject is TrackMarkBase oldMark) oldMark.OnUnSelect();
            }

            CurrentInspectorObject = target;
            _clipInspector.Clear();
            switch (CurrentInspectorObject)
            {
                case null:
                    UnityEditor.Selection.activeObject = null;
                    return;
                case TrackClipBase trackClip:
                    UnityEditor.Selection.activeObject = trackClip.DataInspector;
                    break;
                case TrackBase track:
                    UnityEditor.Selection.activeObject = track.DataInspector;
                    break;
                case TrackMarkBase mark:
                    UnityEditor.Selection.activeObject = mark.DataInspector;
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