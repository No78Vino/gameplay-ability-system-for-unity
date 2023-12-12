using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace GAS.Editor.Tags
{
    public class CreateTagWindow : OdinEditorWindow
    {
        [LabelText("Tag:")] [LabelWidth(100)] public string _tagName = "";

        private Action<string> _confirmCallback;

        public static void OpenWindow(Action<string> confirmCallback)
        {
            var window = GetWindow<CreateTagWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 30);
            window.titleContent = new GUIContent("Create Tag");
            window.ShowPopup();
            window.SetConfirmCallback(confirmCallback);
        }

        private void SetConfirmCallback(Action<string> confirmCallback)
        {
            _confirmCallback = confirmCallback;
        }

        [HorizontalGroup("Buttons")]
        [Button("Create")]
        private void Create()
        {
            _confirmCallback?.Invoke(_tagName);
            Close();
        }

        [HorizontalGroup("Buttons")]
        [Button("Close")]
        private void CloseWindow()
        {
            Close();
        }
    }
}