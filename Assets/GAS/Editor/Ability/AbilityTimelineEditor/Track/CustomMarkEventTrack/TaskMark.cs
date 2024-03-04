using GAS.Runtime.Ability.TimelineAbility;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TaskMark : TrackMark<TaskMarkEventTrack>
    {
        private TaskMarkEvent MarkData => markData as TaskMarkEvent;

        private TaskMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = TaskMarkEventTrack.InstantTaskEventTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return TaskMarkEventTrack.InstantTaskEventTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = "";
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{markData.startFrame}");
            inspector.Add(markFrame);
            
            // TODO InstantTask面板渲染
            // var taskList = TrackInspectorUtil.CreateObjectListView<InstantAbilityTask>("Task", MarkData.InstantTasks,
            //     OnTaskAssetChanged);
            // inspector.Add(taskList);

            return inspector;
        }

        private void OnTaskAssetChanged(int index, ChangeEvent<Object> evt)
        {
            // var cue = evt.newValue as InstantAbilityTask;
            // MarkDataForSave.InstantTasks[index] = cue;
            // AbilityTimelineEditorWindow.Instance.Save();
            // RefreshShow(FrameUnitWidth);
        }
        
        public override void Delete()
        {
            var success = TaskMarkEventTrack.InstantTaskEventTrackData.markEvents.Remove(MarkData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void UpdateMarkDataFrame(int newStartFrame)
        {
            var updatedClip = MarkDataForSave;
            MarkDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            markData = updatedClip;
        }

        public override void OnTickView(int frameIndex)
        {
            // TODO
        }
    }
}