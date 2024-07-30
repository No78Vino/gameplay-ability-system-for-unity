using GAS.Runtime;
using UnityEngine;

namespace GAS.Editor
{
    public class TaskMark : TrackMark<TaskMarkEventTrack>
    {
        public new TaskMarkEvent MarkData => markData as TaskMarkEvent;

        public TaskMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = track.InstantTaskEventTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return track.InstantTaskEventTrackData.markEvents[i];
                return null;
            }
        }

        public override Object DataInspector => TaskMarkEditor.Create(this);

        public override void Duplicate()
        {
            // 添加Mark数据
            var startFrame = markData.startFrame < AbilityAsset.FrameCount
                ? markData.startFrame + 1
                : markData.startFrame - 1;
            startFrame = Mathf.Clamp(startFrame, 0, AbilityAsset.FrameCount);
            var markEvent = new TaskMarkEvent
            {
                startFrame = startFrame,
                InstantTasks = (markData as TaskMarkEvent)?.InstantTasks
            };
            track.InstantTaskEventTrackData.markEvents.Add(markEvent);

            // 刷新显示
            var mark = new TaskMark();
            mark.InitTrackMark(track, track.Track, FrameUnitWidth, markEvent);
            track.TrackItems.Add(mark);
            mark.OnSelect();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = "";
        }

        // #region Inspector
        //
        // private VisualElement taskSonInspector;
        // private ListView taskList;
        //
        // public override VisualElement Inspector()
        // {
        //     var inspector = TrackInspectorUtil.CreateTrackInspector();
        //     var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{markData.startFrame}");
        //     inspector.Add(markFrame);
        //     
        //     taskSonInspector = TrackInspectorUtil.CreateSonInspector();
        //     inspector.Add(taskSonInspector);
        //     
        //     // task列表
        //     taskList = TrackInspectorUtil.CreateListView<InstantTaskData>("Task", MarkData.InstantTasks,
        //         MakeInstantTaskData,BindInstantTaskData, OnSelectionChanged,OnItemAdded,OnItemRemoved);
        //     inspector.Add(taskList);
        //     taskList.SetSelection(0);
        //
        //     // InstantTask面板渲染
        //     DrawTaskSonInspector(taskSonInspector);
        //     
        //     return inspector;
        // }
        //
        // private void OnItemRemoved(IEnumerable<int> obj)
        // {
        //     if (taskList.childCount == 0)
        //     {
        //         taskSonInspector.Clear();
        //     }
        // }
        //
        // private void OnItemAdded(IEnumerable<int> obj)
        // {
        //     if (taskList.childCount == 1)
        //     {
        //         taskList.SetSelection(0);
        //         DrawTaskSonInspector(taskSonInspector);
        //     }
        // }
        //
        // private void OnSelectionChanged(IEnumerable<object> obj)
        // {
        //     DrawTaskSonInspector(taskSonInspector);
        // }
        //
        // private void DrawTaskSonInspector(VisualElement parent)
        // {
        //     parent.Clear();
        //     
        //     // 选择项：所有InstantAbilityTask子类
        //     if (taskList.selectedIndex < MarkData.InstantTasks.Count && taskList.selectedIndex >= 0)
        //     {
        //         taskList.SetSelection(0);
        //         var taskSonTypes = InstantTaskData.InstantTaskSonTypes;
        //         List<string> taskSons = taskSonTypes.Select(sonType => sonType.FullName).ToList();
        //         var initValue = MarkData.InstantTasks[taskList.selectedIndex].TaskData.Type;
        //         var typeSelector =
        //             TrackInspectorUtil.CreateDropdownField("", taskSons, initValue, (evt) =>
        //             {
        //                 MarkDataForSave.InstantTasks[taskList.selectedIndex].TaskData.Type = evt.newValue;
        //                 MarkDataForSave.InstantTasks[taskList.selectedIndex].TaskData.Data = null;
        //                 AbilityTimelineEditorWindow.Instance.Save();
        //                 AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        //             });
        //         parent.Add(typeSelector);
        //         
        //         // 根据选择的InstantAbilityTask子类，显示对应的属性
        //         var task = MarkDataForSave.InstantTasks[taskList.selectedIndex].Load();
        //         if(InstantTaskInspectorMap.TryGetValue(task.GetType(), out var inspectorType))
        //         {
        //             var taskInspector = (InstantTaskInspector)Activator.CreateInstance(inspectorType, task);
        //             parent.Add(taskInspector.Inspector());
        //         }
        //         else
        //         {
        //             parent.Add(TrackInspectorUtil.CreateLabel($"{task.GetType()}'s Inspector not found!"));
        //         }
        //     }
        //     
        //     parent.MarkDirtyRepaint();
        // }
        //
        // private void BindInstantTaskData(VisualElement root, int i)
        // {
        //     MarkData.InstantTasks[i] ??= new InstantTaskData();
        //     var taskValue = MarkData.InstantTasks[i];
        //     var label = (Label)root;
        //     var shotName = taskValue.TaskData.Type.Split('.').Last();
        //     label.text = shotName;
        //
        //     // taskValue.Task.l
        //     //     
        //     // var textField = (TextField)e;
        //     // textField.value = list[i];
        //     // textField.RegisterValueChangedCallback(evt =>
        //     // {
        //     //     onItemValueChanged(i, evt);
        //     // });
        // }
        //
        // private VisualElement MakeInstantTaskData()
        // {
        //     return TrackInspectorUtil.CreateLabel("");
        // }
        //

        //
        // #endregion

        public override void Delete()
        {
            var success = track.InstantTaskEventTrackData.markEvents.Remove(MarkData);
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
            if (frameIndex == StartFrameIndex)
            {
                foreach (var t in MarkData.InstantTasks)
                {
                    var task = t.Load() as InstantAbilityTask;
                    task?.OnEditorPreview();
                }
            }
        }

        public void SaveCurrentTask(InstantAbilityTask task)
        {
            //MarkDataForSave.InstantTasks[taskList.selectedIndex].Save(task);
        }
    }
}