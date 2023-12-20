using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace GAS.Editor.General
{
    public class InputStringWindow : OdinEditorWindow
    {
        [LabelText("Input:")] [LabelWidth(100)] public string content = "";
        
        private Action<string> _confirmCallback;

        public static void OpenWindow(string title,Action<string> confirmCallback)
        {
            var window = GetWindow<InputStringWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 30);
            window.titleContent = new GUIContent(title);
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
            _confirmCallback?.Invoke(content);
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