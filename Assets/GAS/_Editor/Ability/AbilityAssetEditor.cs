using System;
using System.Linq;
using GAS.Editor.General;
using GAS.Runtime.Ability;
using GAS.Runtime.Effects;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Ability
{
    [CustomEditor(typeof(AbilityAsset))]
    public class AbilityAssetEditor:UnityEditor.Editor
    {
        AbilityAsset Asset  => (AbilityAsset)target;
        
        CustomReorderableList<GameplayEffectAsset> _usedGameplayEffects;
        
        CustomReorderableList<string>[] _tagGroup = new CustomReorderableList<string>[10];
        bool[] _tagGroupFoldout = new bool[10];
        string[] _tagGroupTitle = new string[10]
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
            "TargetBlockedTags",
        };
        
        


        private void OnEnable()
        {
            _usedGameplayEffects =
                CustomReorderableList<GameplayEffectAsset>.Create(Asset.UsedGameplayEffects, null, OnGEGUIDraw);
            
            _tagGroup[0] = CustomReorderableList<string>.Create(Asset.AssetTag, null, OnTagGUIDraw);
            _tagGroup[1] = CustomReorderableList<string>.Create(Asset.CancelAbilityTags, null, OnTagGUIDraw);
            _tagGroup[2] = CustomReorderableList<string>.Create(Asset.BlockAbilityTags, null, OnTagGUIDraw);
            _tagGroup[3] = CustomReorderableList<string>.Create(Asset.ActivationOwnedTag, null, OnTagGUIDraw);
            _tagGroup[4] = CustomReorderableList<string>.Create(Asset.ActivationRequiredTags, null, OnTagGUIDraw);
            _tagGroup[5] = CustomReorderableList<string>.Create(Asset.ActivationBlockedTags, null, OnTagGUIDraw);
            _tagGroup[6] = CustomReorderableList<string>.Create(Asset.SourceRequiredTags, null, OnTagGUIDraw);
            _tagGroup[7] = CustomReorderableList<string>.Create(Asset.SourceBlockedTags, null, OnTagGUIDraw);
            _tagGroup[8] = CustomReorderableList<string>.Create(Asset.TargetRequiredTags, null, OnTagGUIDraw);
            _tagGroup[9] = CustomReorderableList<string>.Create(Asset.TargetBlockedTags, null, OnTagGUIDraw);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", GUILayout.Width(70f));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name);
            }

            using (new EditorGUILayout.FadeGroupScope(0.5f))
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
                for (int i = 0; i < _tagGroup.Length; i++)
                {
                    _tagGroupFoldout[i] = EditorGUILayout.Foldout(_tagGroupFoldout[i], _tagGroupTitle[i]);
                    if (_tagGroupFoldout[i])
                    {
                        _tagGroup[i].OnGUI();
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            // GUILayout.Space(5f);
            // ToolBar();
            // GUILayout.Space(3f);
            //
            // for (var i = 0; i < Asset.UsedGameplayEffects.Length; i++)
            // {
            //     EditorGUILayout.BeginHorizontal();
            //
            //     // Show the string
            //     EditorGUILayout.LabelField($"No.{i} : {Asset.UsedGameplayEffects[i].name}");
            //
            //     // Edit button to modify the selected string
            //     if (GUILayout.Button("Edit", GUILayout.Width(50)))
            //     {
            //         _selectedIndex = i;
            //         OpenEditPopup();
            //     }
            //
            //     if (GUILayout.Button("Remove", GUILayout.Width(100)))
            //     {
            //         _selectedIndex = i;
            //         Remove(Asset.UsedGameplayEffects[i]);
            //     }
            //     
            //     EditorGUILayout.EndHorizontal();
            // }
        }

        void OnGEGUIDraw(Rect rect, GameplayEffectAsset item,int index)
        {
            item = EditorGUI.ObjectField(new Rect(rect.x, rect.y,
                    300, EditorGUIUtility.singleLineHeight), item, typeof(GameplayEffectAsset),
                false) as GameplayEffectAsset;
            
            _usedGameplayEffects.UpdateItem(index,item);
        }
        
        void OnTagGUIDraw(Rect rect, string item,int index)
        {
            item = EditorGUI.TextField(new Rect(rect.x, rect.y,
                    300, EditorGUIUtility.singleLineHeight), item);
            
            _tagGroup[index].UpdateItem(index,item);
        }
    }
}