using System;
using System.Collections.Generic;
using System.Linq;
using GAS.General;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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

        private static Type[] _instantTaskInspectorTypes;

        public static Type[] InstantTaskInspectorTypes =>
            _instantTaskInspectorTypes ??= TypeUtil.GetAllSonTypesOf(typeof(InstantAbilityTaskInspector));
        
        private static Dictionary<Type, Type> _instantTaskInspectorMap;
        private static Dictionary<Type, Type> InstantTaskInspectorMap
        {
            get
            {
                if (_instantTaskInspectorMap != null) return _instantTaskInspectorMap;
                _instantTaskInspectorMap = new Dictionary<Type, Type>();
                foreach (var inspectorType in InstantTaskInspectorTypes)
                {
                    var taskType = inspectorType.BaseType.GetGenericArguments()[0];
                    _instantTaskInspectorMap.Add(taskType, inspectorType);
                }

                return _instantTaskInspectorMap;
            }
        }
        
        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = "";
        }

        #region Inspector

        private VisualElement taskSonInspector;
        private ListView taskList;
        
        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{markData.startFrame}");
            inspector.Add(markFrame);
            
            taskSonInspector = TrackInspectorUtil.CreateSonInspector();
            inspector.Add(taskSonInspector);
            
            // task列表
            taskList = TrackInspectorUtil.CreateListView<InstantTaskData>("Task", MarkData.InstantTasks,
                MakeInstantTaskData,BindInstantTaskData, OnSelectionChanged);
            inspector.Add(taskList);
            taskList.SetSelection(0);

            // InstantTask面板渲染
            DrawTaskSonInspector(taskSonInspector);
            
            return inspector;
        }

        private void OnSelectionChanged(IEnumerable<object> obj)
        {
            DrawTaskSonInspector(taskSonInspector);
        }

        private void DrawTaskSonInspector(VisualElement parent)
        {
            parent.Clear();
            
            // 选择项：所有InstantAbilityTask子类
            var taskSonTypes= InstantTaskData.InstantTaskSonTypes;
            List<string> taskSons  = taskSonTypes.Select(sonType => sonType.FullName).ToList();
            var typeSelector =
                TrackInspectorUtil.CreateDropdownField("InstantTask", taskSons,
                    MarkData.InstantTasks[taskList.selectedIndex].TaskData.Type, (evt) =>
                    {
                        MarkDataForSave.InstantTasks[taskList.selectedIndex].TaskData.Type = evt.newValue;
                        MarkDataForSave.InstantTasks[taskList.selectedIndex].TaskData.Data = null;
                        AbilityTimelineEditorWindow.Instance.Save();
                        AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
                    });
            parent.Add(typeSelector);
            
            // 根据选择的InstantAbilityTask子类，显示对应的属性
            var task = MarkDataForSave.InstantTasks[taskList.selectedIndex].Load();
            if(InstantTaskInspectorMap.TryGetValue(task.GetType(), out var inspectorType))
            {
                var taskInspector = (InstantAbilityTaskInspector)Activator.CreateInstance(inspectorType, task);
                parent.Add(taskInspector.Inspector());
            }
            else
            {
                parent.Add(TrackInspectorUtil.CreateLabel($"{task.GetType()}'s Inspector not found!"));
            }
            
            parent.MarkDirtyRepaint();
        }
        
        private void BindInstantTaskData(VisualElement root, int i)
        {
            MarkData.InstantTasks[i] ??= new InstantTaskData();
            var taskValue = MarkData.InstantTasks[i];
            var label = (Label)root;
            var shotName = taskValue.TaskData.Type.Split('.').Last();
            label.text = shotName;

            // taskValue.Task.l
            //     
            // var textField = (TextField)e;
            // textField.value = list[i];
            // textField.RegisterValueChangedCallback(evt =>
            // {
            //     onItemValueChanged(i, evt);
            // });
        }
        
        private VisualElement MakeInstantTaskData()
        {
            return TrackInspectorUtil.CreateLabel("");
        }
        
        public override void Delete()
        {
            var success = TaskMarkEventTrack.InstantTaskEventTrackData.markEvents.Remove(MarkData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }
        
        #endregion
        
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

        public void SaveCurrentTask(InstantAbilityTask task)
        {
            MarkDataForSave.InstantTasks[taskList.selectedIndex].Save(task);
        }
    }
}