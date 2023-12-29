using System;
using System.Collections.Generic;
using GAS.Editor.General;
using GAS.Editor.Tags;
using GAS.Runtime.Ability;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Ability
{
    [CustomEditor(typeof(AbilityAsset))]
    public class AbilityAssetEditor : UnityEditor.Editor
    {
        private readonly bool[] _tagGroupFoldout = new bool[10];

        private readonly string[] _tagGroupTitle = new string[10]
        {
            "AssetTag",
            "CancelAbilityTags",
            "BlockAbilityTags",
            "ActivationOwnedTag",
            "ActivationRequiredTags",
            "ActivationBlockedTags",
            "SourceRequiredTags",
            "SourceBlockedTags",
            "TargetRequiredTags",
            "TargetBlockedTags"
        };

        private readonly ArraySetFromChoicesAsset<GameplayTag>[] _tagGroupAsset =
            new ArraySetFromChoicesAsset<GameplayTag>[10];

        private ScriptableObjectReorderableList<GameplayEffectAsset> _usedGameplayEffects;

        private List<GameplayTag> tagChoices = new List<GameplayTag>();
        private AbilityAsset Asset => (AbilityAsset)target;

        private void OnEnable()
        {
            tagChoices = TagEditorUntil.GetTagChoice();

            _usedGameplayEffects = new ScriptableObjectReorderableList<GameplayEffectAsset>(
                Asset.UsedGameplayEffects, "UsedGameplayEffects");

            for (var i = 0; i < _tagGroupAsset.Length; i++)
            {
                var initTags = i switch
                {
                    0 => Asset.AssetTag,
                    1 => Asset.CancelAbilityTags,
                    2 => Asset.BlockAbilityTags,
                    3 => Asset.ActivationOwnedTag,
                    4 => Asset.ActivationRequiredTags,
                    5 => Asset.ActivationBlockedTags,
                    6 => Asset.SourceRequiredTags,
                    7 => Asset.SourceBlockedTags,
                    8 => Asset.TargetRequiredTags,
                    9 => Asset.TargetBlockedTags,
                    _ => Array.Empty<GameplayTag>()
                };

                _tagGroupAsset[i] =
                    new ArraySetFromChoicesAsset<GameplayTag>(initTags, tagChoices, _tagGroupTitle[i], null);
            }
        }

        private void OnValidate()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", GUILayout.Width(70f));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(70f));
                Asset.Description = EditorGUILayout.TextField("", Asset.Description);
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("GameplayEffect", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cost", GUILayout.Width(70f));
                Asset.Cost =
                    (GameplayEffectAsset)EditorGUILayout.ObjectField("", Asset.Cost, typeof(GameplayEffectAsset),
                        false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cooldown", GUILayout.Width(70f));
                Asset.Cooldown =
                    (GameplayEffectAsset)EditorGUILayout.ObjectField("", Asset.Cooldown, typeof(GameplayEffectAsset),
                        false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField("UsedGameplayEffect:");
                _usedGameplayEffects.OnGUI();
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" ", GUILayout.Width(20f));

                EditorGUILayout.BeginVertical();
                for (var i = 0; i < _tagGroupAsset.Length; i++)
                {
                    _tagGroupFoldout[i] = EditorGUILayout.Foldout(_tagGroupFoldout[i], _tagGroupTitle[i]);
                    if (_tagGroupFoldout[i]) _tagGroupAsset[i].OnGUI();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save")) Save();

            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            // Check Tags
            for (var i = 0; i < _tagGroupAsset.Length; i++)
            {
                var data = _tagGroupAsset[i].GetItemList();
                switch (i)
                {
                    case 0:
                        Asset.AssetTag = data.ToArray();
                        break;
                    case 1:
                        Asset.CancelAbilityTags = data.ToArray();
                        break;
                    case 2:
                        Asset.BlockAbilityTags = data.ToArray();
                        break;
                    case 3:
                        Asset.ActivationOwnedTag = data.ToArray();
                        break;
                    case 4:
                        Asset.ActivationRequiredTags = data.ToArray();
                        break;
                    case 5:
                        Asset.ActivationBlockedTags = data.ToArray();
                        break;
                    case 6:
                        Asset.SourceRequiredTags = data.ToArray();
                        break;
                    case 7:
                        Asset.SourceBlockedTags = data.ToArray();
                        break;
                    case 8:
                        Asset.TargetRequiredTags = data.ToArray();
                        break;
                    case 9:
                        Asset.TargetBlockedTags = data.ToArray();
                        break;
                }
            }

            // Check UsedGameplayEffects
            var usedGeData = _usedGameplayEffects.GetItemList();
            Asset.UsedGameplayEffects = usedGeData.ToArray();

            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }
    }
}