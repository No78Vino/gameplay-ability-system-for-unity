using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTimeline;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    [CustomEditor(typeof(TimelineAbilityAsset))]
    public class TimelineAbilityEditorWindow:OdinEditor
    {
        private TimelineAbilityAsset _asset => target as TimelineAbilityAsset;

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
            AbilityTimelineEditorWindow.ShowWindow(_asset);
        }
    }
}