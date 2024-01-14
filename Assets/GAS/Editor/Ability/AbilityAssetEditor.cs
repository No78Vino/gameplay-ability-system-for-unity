#if UNITY_EDITOR
namespace GAS.Editor.Ability
{
    using System;
    using System.Collections.Generic;
    using General;
    using Tags;
    using GAS.Runtime.Ability;
    using GAS.Runtime.Effects;
    using GAS.Runtime.Tags;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(AbilityAsset))]
    public class AbilityAssetEditor : Editor
    {
        private readonly string[] _tagGroupTitle = new string[6]
        {
            "AssetTag",
            "CancelAbilityTags",
            "BlockAbilityTags",
            "ActivationOwnedTag",
            "ActivationRequiredTags",
            "ActivationBlockedTags"
        };

        private readonly ArraySetFromChoicesAsset<GameplayTag>[] _tagGroupAsset =
            new ArraySetFromChoicesAsset<GameplayTag>[6];

        private ScriptableObjectReorderableList<GameplayEffectAsset> _usedGameplayEffects;

        private List<string> abilityChoices = new List<string>();
        
        private List<GameplayTag> tagChoices = new List<GameplayTag>();
        private AbilityAsset Asset => (AbilityAsset)target;

        private GUIStyle greenButtonStyle;
        private Vector2 scrollPos;

        private void Awake()
        {
            greenButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 24,
                normal =
                {
                    textColor = Color.green
                },
                fontStyle = FontStyle.Bold
            };
        }
        
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
                    _ => Array.Empty<GameplayTag>()
                };

                _tagGroupAsset[i] =
                    new ArraySetFromChoicesAsset<GameplayTag>(initTags, tagChoices, _tagGroupTitle[i], null);
            }

            abilityChoices = AbilityEditorUtil.GetAbilityClassNames();
        }

        private void OnValidate()
        {
            Save();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(600));

                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(300f));
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Name", GUILayout.Width(100));
                    Asset.Name = EditorGUILayout.TextField("", Asset.Name, GUILayout.Width(200f));
                }

                EditorGUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Description", GUILayout.Width(100));
                    Asset.Description = EditorGUILayout.TextField("", Asset.Description, GUILayout.Width(200f));
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(10);

                {
                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(300f));

                    EditorGUILayout.LabelField(
                        "<size=13><b><color=green>Unique Name is very important!" +
                        "GAS will use the unique name as a UID for the ability." +
                        "Therefore,you must keep this name unique." +
                        "Don't worry.When generating the code, the tool will check this.</color></b></size>",
                        new GUIStyle(GUI.skin.box) { richText = true ,alignment = TextAnchor.MiddleLeft}, 
                        GUILayout.Width(300));
                    
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(300f)))
                    {
                        EditorGUILayout.LabelField("<size=14><b><color=white>UniqueName</color></b></size>", new GUIStyle(GUI.skin.label) { richText = true },GUILayout.Width(100));
                        Asset.UniqueName = EditorGUILayout.TextField("", Asset.UniqueName, GUILayout.Width(200f));
                    }

                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(300f)))
                    {
                        if (abilityChoices.Count == 0)
                        {
                            EditorGUILayout.LabelField(
                                "<size=16><b><color=yellow>No Abilities Found!  Please,Create an ability first!</color></b></size>",
                                new GUIStyle(GUI.skin.label) { richText = true }, GUILayout.Width(300));
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Ability Class", GUILayout.Width(100));
                            var index = abilityChoices.IndexOf(Asset.InstanceAbilityClassFullName);
                            index = Mathf.Clamp(index, 0, abilityChoices.Count - 1);
                            index = EditorGUILayout.Popup(index, abilityChoices.ToArray(), GUILayout.Width(200));
                            Asset.InstanceAbilityClassFullName = abilityChoices[index];
                        }
                    }

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Cost", GUILayout.Width(100));
                    Asset.Cost =
                        (GameplayEffectAsset)EditorGUILayout.ObjectField("", Asset.Cost, typeof(GameplayEffectAsset),
                            false, GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Cooldown", GUILayout.Width(100));
                    Asset.Cooldown =
                        (GameplayEffectAsset)EditorGUILayout.ObjectField("", Asset.Cooldown,
                            typeof(GameplayEffectAsset),
                            false, GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorUtil.Separator();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUI.skin.box);
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(400)))
            {
                _usedGameplayEffects.OnGUI();
            }

            EditorUtil.Separator();
            
            EditorGUILayout.LabelField("<size=16><b><color=white>Tags</color></b></size>",
                new GUIStyle() { alignment = TextAnchor.MiddleCenter,richText = true}, GUILayout.Width(600));
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(600)))

            {
                for (var i = 0; i < 3; i++)
                {
                    var t = _tagGroupAsset[i];
                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
                    t.OnGUI();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope(GUI.skin.box,GUILayout.Width(600)))
            {
                for (var i = 3; i < 6; i++)
                {
                    var t = _tagGroupAsset[i];
                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250));
                    t.OnGUI();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("SAVE", greenButtonStyle,GUILayout.Height(60))) Save();
            
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
#endif