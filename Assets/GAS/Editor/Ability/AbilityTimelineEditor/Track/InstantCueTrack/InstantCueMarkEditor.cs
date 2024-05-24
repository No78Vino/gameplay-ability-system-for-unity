using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class InstantCueMarkEditor : OdinEditorWindow
    {
        private InstantCueMark _mark;

        public static InstantCueMarkEditor Create(InstantCueMark mark)
        {
            var window = CreateInstance<InstantCueMarkEditor>();
            window._mark = mark;

            window.UpdateMarkInfo();
            return window;
        }

        [BoxGroup]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left, true)]
        public string RunInfo;

        [Delayed]
        [BoxGroup]
        [AssetSelector]
        [ListDrawerSettings(ShowFoldout = true, DraggableItems = true)]
        [OnValueChanged("OnCueListChanged")]
        public List<GameplayCueInstant> Cues;

        [BoxGroup]
        [Button]
        [GUIColor(0.9f, 0.2f, 0.2f)]
        void Delete()
        {
            _mark.Delete();
        }

        void UpdateMarkInfo()
        {
            RunInfo = $"<b>Trigger(f):{_mark.InstantCueMarkData.startFrame}</b>";
            Cues = _mark.InstantCueMarkData.cues;
        }

        void OnCueListChanged()
        {
            _mark.MarkDataForSave.cues = Cues;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }

    [CustomEditor(typeof(InstantCueMarkEditor))]
    public class InstantCueMarkInspector : OdinEditorWithoutHeader
    {
    }
}