using GAS.Runtime.Ability;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Ability
{
    [CustomEditor(typeof(GeneralSequentialAbilityAsset))]
    public class GeneralSequentialAbilityEditorWindow:OdinEditor
    {
        private GeneralSequentialAbilityAsset _asset => target as GeneralSequentialAbilityAsset;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("查看/编辑能力时间轴",GUILayout.Height(30),GUILayout.Width(300)))
            {
                EditAbilityTimeline();
            }
            EditorGUILayout.EndVertical();
        }
        
        void EditAbilityTimeline()
        {
            AbilityTimelineEditorWindow.Open(_asset);
        }
    }
}