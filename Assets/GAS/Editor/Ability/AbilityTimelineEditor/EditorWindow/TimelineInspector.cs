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
                if (CurrentInspectorObject is TrackClipBase oldTrackItem) oldTrackItem.ClipVe.OnUnSelect();
                if (CurrentInspectorObject is TrackBase oldTrack) oldTrack.OnUnSelect();
                if (CurrentInspectorObject is TrackMarkBase oldMark) oldMark.OnUnSelect();
            }

            CurrentInspectorObject = target;
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