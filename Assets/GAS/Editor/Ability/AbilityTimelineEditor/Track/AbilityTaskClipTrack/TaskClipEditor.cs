using System.Collections;
using System.Linq;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class TaskClipEditor : OdinEditorWindow
    {
        private const string GRP_BOX = "AbilityTask";
        private const string GRP_BOX_TASK = "AbilityTask/Task";

        private static IEnumerable _abilityTaskTypes =
            EditorAbilityHelper.GetCachedAbilityTaskTypes().Select(t => t.Name).ToArray();


        [BoxGroup(GRP_BOX)] [Delayed] [LabelText("任务名[展示用]")] [OnValueChanged(nameof(OnNameChange))]
        public string Name;

        [Delayed] [BoxGroup(GRP_BOX)] [LabelText("起始帧(f)")] [OnValueChanged(nameof(OnClipStartFrameChanged))]
        public int StartTime;

        [Delayed] [BoxGroup(GRP_BOX)] [LabelText("结束帧(f)")] [OnValueChanged(nameof(OnClipEndFrameChanged))]
        public int EndTime;

        [Delayed]
        [BoxGroup(GRP_BOX_TASK)]
        [LabelText("Task类型")]
        [ValueDropdown(nameof(_abilityTaskTypes))]
        [OnValueChanged(nameof(OnTaskTypeChanged))]
        public string TaskType;

        [BoxGroup(GRP_BOX_TASK)] [HideLabel] [ShowInInspector] [HideReferenceObjectPicker]
        public IExParameterBase Parameter;

        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;

        private TaskClip _clip;
        
        public static TaskClipEditor Create(TaskClip clip)
        {
            var window = CreateInstance<TaskClipEditor>();
            window._clip = clip;
            window.Refresh();
            return window;
        }

        [BoxGroup(GRP_BOX)]
        [Button("删除")]
        [GUIColor(0.9f, 0.2f, 0.2f)]
        private void Delete()
        {
            _clip.Delete();
        }


        private void Refresh()
        {
            Name = _clip.TaskClipData.Name;
            StartTime = _clip.TaskClipData.StartTime;
            EndTime = _clip.TaskClipData.EndTime;
            TaskType = _clip.TaskClipData.TaskType;
            Parameter = _clip.TaskClipData.Parameter;
        }

        private void OnClipStartFrameChanged()
        {
            // 钳制
            StartTime = Mathf.Clamp(StartTime, 0, EndTime);
            _clip.UpdateClipDataStartFrame(StartTime);
            _clip.RefreshShow(_clip.FrameUnitWidth);
        }
        
        private void OnClipEndFrameChanged()
        {
            // 钳制
            EndTime = Mathf.Clamp(EndTime, StartTime, AbilityConfig.LifeTime);
            _clip.UpdateClipDataEndFrame(EndTime);
            _clip.RefreshShow(_clip.FrameUnitWidth);
        }

        private void OnTaskTypeChanged()
        {
            Parameter = EditorAbilityHelper.CreateAbilityTaskParameter(TaskType);
            AbilityTimelineEditorWindow.Instance.Save();
        }

        private void OnNameChange()
        {
            _clip.TaskClipData.Name = Name;
            _clip.RefreshShow(_clip.FrameUnitWidth);
        }
    }

    [CustomEditor(typeof(TaskClipEditor))]
    public class TaskClipInspector : OdinEditorWithoutHeader
    {
    }
}
#endif