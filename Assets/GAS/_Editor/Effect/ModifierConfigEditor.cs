using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Core;
using GAS.Editor.AttributeSet;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Effect
{
    public class ModifierConfigEditor : EditorWindow
    {
        private GameplayEffectModifier _sourceModifier;

        private static List<string> _attributeOptions;

        private Action<GameplayEffectModifier> _callback;

        private static List<string> AttributeOptions
        {
            get
            {
                if (_attributeOptions == null)
                {
                    _attributeOptions = new List<string>();
                    var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GasDefine.GAS_ATTRIBUTESET_ASSET_PATH);
                    foreach (var attributeSetConfig in asset.AttributeSetConfigs)
                    {
                        var config = attributeSetConfig;
                        foreach (var fullName in attributeSetConfig.AttributeNames.Select(shortName =>
                                     $"{config.Name}.{shortName}"))
                        {
                            _attributeOptions.Add(fullName);
                        }
                    }
                }

                return _attributeOptions;
            }
        }

        public static void OpenWindow(GameplayEffectModifier sourceModifier, Action<GameplayEffectModifier> callback)
        {
            var window = GetWindow<ModifierConfigEditor>();
            window.Init(sourceModifier, callback);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attribute Name:", GUILayout.Width(100));
            int indexOfTag = AttributeOptions.IndexOf(_sourceModifier.AttributeName);
            int idx = EditorGUILayout.Popup("", indexOfTag, AttributeOptions.ToArray());
            idx = Mathf.Clamp(idx, 0, AttributeOptions.Count - 1);
            _sourceModifier.AttributeName = AttributeOptions[idx];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Operation:", GUILayout.Width(100));
            _sourceModifier.Operation = (GEOperation)EditorGUILayout.EnumPopup("", _sourceModifier.Operation);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Value:", GUILayout.Width(100));
            _sourceModifier.ModiferMagnitude = EditorGUILayout.FloatField("", _sourceModifier.ModiferMagnitude);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Modifier Magnitude Calculation:", GUILayout.Width(200));
            _sourceModifier.MMC = (ModifierMagnitudeCalculation) EditorGUILayout.ObjectField("", _sourceModifier.MMC,
                typeof(ModifierMagnitudeCalculation), false);
            EditorGUILayout.EndVertical();

            if (_sourceModifier.MMC != null)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("MMC Info:");
                var editor = UnityEditor.Editor.CreateEditor(_sourceModifier.MMC);
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save")) Save();

            EditorGUILayout.EndVertical();
        }

        private void Init(GameplayEffectModifier sourceModifier, Action<GameplayEffectModifier> callback)
        {
            _sourceModifier = sourceModifier;
            _callback = callback;
        }

        private void Save()
        {
            _callback?.Invoke(_sourceModifier);
            Close();
        }
    }
}