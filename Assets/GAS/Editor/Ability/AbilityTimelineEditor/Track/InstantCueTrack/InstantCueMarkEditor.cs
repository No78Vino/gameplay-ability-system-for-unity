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
        }
    }

    [CustomEditor(typeof(InstantCueMarkEditor))]
    public class InstantCueMarkInspector : OdinEditorWithoutHeader
    {
    }
}