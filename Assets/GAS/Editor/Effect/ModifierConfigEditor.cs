#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using Editor;
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    public class ModifierConfigEditor : EditorWindow
    {
        private List<string> _attributeOptions;

        private Action<GameplayEffectModifier> _callback;
        private GameplayEffectModifier _sourceModifier;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attribute Name:", GUILayout.Width(100));
            var indexOfTag = _attributeOptions.IndexOf(_sourceModifier.AttributeName);
            var idx = EditorGUILayout.Popup("", indexOfTag, _attributeOptions.ToArray());
            idx = Mathf.Clamp(idx, 0, _attributeOptions.Count - 1);
            _sourceModifier.AttributeName = _attributeOptions[idx];
            var nameSplit = _sourceModifier.AttributeName.Split('.');
            _sourceModifier.AttributeSetName = nameSplit[0];
            _sourceModifier.AttributeShortName = nameSplit[1];
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
            _sourceModifier.MMC = (ModifierMagnitudeCalculation)EditorGUILayout.ObjectField("", _sourceModifier.MMC,
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

        public static void OpenWindow(GameplayEffectModifier sourceModifier, Action<GameplayEffectModifier> callback)
        {
            var window = GetWindow<ModifierConfigEditor>();
            window.Init(sourceModifier, callback);
            window.Show();
        }

        private void Init(GameplayEffectModifier sourceModifier, Action<GameplayEffectModifier> callback)
        {
            _attributeOptions = AttributeEditorUtil.GetAttributeNameChoices();
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
#endif