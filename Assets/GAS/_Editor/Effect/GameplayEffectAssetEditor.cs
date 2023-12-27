using GAS.Runtime.Effects;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Effect
{
    [CustomEditor(typeof(GameplayEffectAsset))]
    public class GameplayEffectAssetEditor:UnityEditor.Editor
    {
        private GameplayEffectAsset Asset => (GameplayEffectAsset)target;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name",GUILayout.Width(100));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name);
            }
            
            EditorGUILayout.Space(5f);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Description",GUILayout.Width(100));
                Asset.Description = EditorGUILayout.TextField("", Asset.Description);
            }

            DurationPolicyGroup();
            
            
            
            
            GUILayout.FlexibleSpace();
            
            if(GUILayout.Button("Save")) Save();
            
            EditorGUILayout.EndVertical();
        }

        void DurationPolicyGroup()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            EditorGUILayout.LabelField("Apply Policy",EditorStyles.boldLabel);
            
            EditorGUILayout.Space(3);
            
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Duration Policy",GUILayout.Width(100));
                Asset.DurationPolicy = (EffectsDurationPolicy)EditorGUILayout.EnumPopup("", Asset.DurationPolicy);
                
                EditorGUILayout.Space(10);
                
                // if(Asset.DurationPolicy != EffectsDurationPolicy.Instant)
                //     Asset.Duration = 0;
                // else
                // {
                //     EditorGUILayout.LabelField("Duration",GUILayout.Width(100));
                //     Asset.Duration = EditorGUILayout.FloatField("", Asset.Duration);
                // }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}