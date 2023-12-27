using GAS.Core;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.GameplayAbilitySystem
{
    [CustomEditor(typeof(GASSettingAsset))]
    public class GASSettingAssetEditor : UnityEditor.Editor
    {
        private GASSettingAsset Asset => (GASSettingAsset)target;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200));
            
            EditorGUILayout.LabelField("Version:",EditorStyles.boldLabel);
            EditorGUILayout.LabelField(GasDefine.GAS_VERSION);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(500));

            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            Asset.GASConfigAssetPath =
                EditorGUILayout.TextField("Config Asset Path", Asset.GASConfigAssetPath);
            
            GUILayout.Space(5f);

            Asset.CodeGeneratePath =
                EditorGUILayout.TextField("Code Gen Path", Asset.CodeGeneratePath);
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10f);
            
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Gameplay Local Lib",EditorStyles.boldLabel);
            GUILayout.Space(3f);
            
            Asset.GameplayAbilityLibPath =
                EditorGUILayout.TextField("Gameplay Ability Lib Path", Asset.GameplayAbilityLibPath);
            
            GUILayout.Space(3f);
            
            Asset.GameplayEffectLibPath =
                EditorGUILayout.TextField("Gameplay Effect Lib Path", Asset.GameplayEffectLibPath);
            
            GUILayout.Space(3f);
            
            Asset.GameplayCueLibPath =
                EditorGUILayout.TextField("Gameplay Cue Lib Path", Asset.GameplayCueLibPath);
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(20f);

            if (GUILayout.Button("Save")) Save();

            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}