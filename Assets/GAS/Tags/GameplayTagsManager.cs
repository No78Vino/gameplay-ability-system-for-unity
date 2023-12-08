using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Tags
{
    public class GameplayTagsManager:OdinEditorWindow
    {
        private const string KeyNameOfTagManagerAutoSave = "TagManagerAutoSave";
        
        //[HorizontalGroup("Bridge",250,5,5)]
        [InfoBox("Draws the toggle button before the label for a bool property.")]
        [LabelText("自动保存"),LabelWidth(100)]
        [ToggleLeft][OnValueChanged("OnSwitchAutoSave")]
        public bool autoSave;

        [MenuItem("GAS/Gameplay Tags Manager")]
        public static void Open()
        {
            var window = GetWindow<GameplayTagsManager>("Gameplay Tags Manager");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        void OnSwitchAutoSave()
        {
            EditorPrefs.SetBool(KeyNameOfTagManagerAutoSave, autoSave);
        }

        protected override void OnImGUI()
        {
            base.OnImGUI();
            
        }
    }
}