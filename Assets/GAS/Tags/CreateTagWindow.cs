using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace GAS.Editor.Tags
{
    public class CreateTagWindow : OdinEditorWindow
    {
        [LabelText("Tag:")][LabelWidth(100)]
        public string _tagName = "";
        private GameplayTagsScriptableObject _operatedScriptableObject;
        
        public static void OpenWindow(GameplayTagsScriptableObject gameplayTagsScriptableObject)
        {
            var window = GetWindow<CreateTagWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 30);
            window.titleContent = new GUIContent("Create Tag");
            window.ShowPopup();
            window.SetOperatedScriptableObject(gameplayTagsScriptableObject);
        }

        private void SetOperatedScriptableObject(GameplayTagsScriptableObject gameplayTagsScriptableObject)
        {
            _operatedScriptableObject = gameplayTagsScriptableObject;
        }
        
        [HorizontalGroup("Buttons")]
        [Button("Create")]
        void Create()
        {
            _operatedScriptableObject.CreateNewTag(_tagName);
            Close();
        }
        
        [HorizontalGroup("Buttons")]
        [Button("Close")]
        void CloseWindow()
        {
            Close();
        }
    }
}