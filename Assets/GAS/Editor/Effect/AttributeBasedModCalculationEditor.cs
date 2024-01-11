
#if UNITY_EDITOR
namespace GAS.Editor.Effect
{
    using System.Collections.Generic;
    using System.Linq;
    using GAS.Core;
    using GAS.Runtime.AttributeSet;
    using GAS.Runtime.Effects.Modifier;
    using UnityEditor;
    using UnityEngine;
    using GAS.Editor.GameplayAbilitySystem;

    [CustomEditor(typeof(AttributeBasedModCalculation))]
    public class AttributeBasedModCalculationEditor : UnityEditor.Editor
    {
        private static List<string> _attributeOptions;
        private AttributeBasedModCalculation Asset => (AttributeBasedModCalculation)target;

        private static List<string> AttributeOptions
        {
            get
            {
                if (_attributeOptions == null)
                {
                    _attributeOptions = new List<string>();
                    var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
                    foreach (var attributeSetConfig in asset.AttributeSetConfigs)
                    {
                        var config = attributeSetConfig;
                        foreach (var fullName in attributeSetConfig.AttributeNames.Select(shortName =>
                                     $"AS_{config.Name}.{shortName}"))
                            _attributeOptions.Add(fullName);
                    }
                }

                return _attributeOptions;
            }
        }

        private void OnValidate()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attribute Name:", GUILayout.Width(100));
            var indexOfTag = AttributeOptions.IndexOf(Asset.attributeName);
            var idx = EditorGUILayout.Popup("", indexOfTag, AttributeOptions.ToArray());
            idx = Mathf.Clamp(idx, 0, AttributeOptions.Count - 1);
            Asset.attributeName = AttributeOptions[idx];
            if (!string.IsNullOrEmpty(Asset.attributeName))
            {
                var split = Asset.attributeName.Split('.');
                Asset.attributeSetName = split[0];
                Asset.attributeShortName = split[1];
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attribute From:", GUILayout.Width(100));
            Asset.attributeFromType =
                (AttributeBasedModCalculation.AttributeFrom)EditorGUILayout.EnumPopup(Asset.attributeFromType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Capture Type:", GUILayout.Width(100));
            Asset.captureType =
                (AttributeBasedModCalculation.GEAttributeCaptureType)EditorGUILayout.EnumPopup(Asset.captureType);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("K:", GUILayout.Width(30));
            Asset.k = EditorGUILayout.FloatField(Asset.k,GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("B:", GUILayout.Width(30));
            Asset.b = EditorGUILayout.FloatField(Asset.b,GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif