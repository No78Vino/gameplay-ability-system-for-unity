using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Core;
using GAS.Editor.General;
using GAS.Editor.Tags;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Effect
{
    [CustomEditor(typeof(GameplayEffectAsset))]
    public class GameplayEffectAssetEditor:UnityEditor.Editor
    {
        private GameplayEffectAsset Asset => (GameplayEffectAsset)target;
        
        Vector2 scrollPos;

        private List<string> tagChoices=new List<string>();
        
        bool[] showTagGroups = new bool[5];
        string[] tagGroupNames = new string[5]
        {
            "Asset Tags",
            "Granted Tags",
            "Application Required Tags",
            "Ongoing Required Tags",
            "Remove GameplayEffects With Tags"
        };
            
        CustomReorderableList<GameplayEffectModifier> modifierList;
        
        private void OnEnable()
        {
            tagChoices.Clear();
            
            var tagAsset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GasDefine.GAS_TAG_ASSET_PATH);
            for (int i = 0; i < tagAsset.Tags.Count; i++)
            {
                tagChoices.Add(tagAsset.Tags[i].Name);
            }

            modifierList = CustomReorderableList<GameplayEffectModifier>.Create(Asset.Modifiers,OnEditModifier,OnModifierDrawGUI,GetModifierElementHeight);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", GUILayout.Width(100));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name);
            }

            EditorGUILayout.Space(5f);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(100));
                Asset.Description = EditorGUILayout.TextField("", Asset.Description);
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DurationPolicyGroup();
            EditorGUILayout.Space(5f);
            TagContainerGroup();
            EditorGUILayout.Space(5f);
            ModifierGroup();
            EditorGUILayout.Space(5f);
            CueGroup();
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save")) Save();

            EditorGUILayout.EndVertical();
        }

        void DurationPolicyGroup()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Apply Policy", EditorStyles.boldLabel);

            EditorGUILayout.Space(3);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Duration Policy", GUILayout.Width(100));
                Asset.DurationPolicy = (EffectsDurationPolicy)EditorGUILayout.EnumPopup("", Asset.DurationPolicy,
                    GUILayout.Width(70));

                if (Asset.DurationPolicy == EffectsDurationPolicy.Duration)
                {
                    EditorGUILayout.LabelField("-->", EditorStyles.miniBoldLabel ,GUILayout.Width(20));
                    EditorGUILayout.LabelField("Duration", GUILayout.Width(60));
                    Asset.Duration = EditorGUILayout.FloatField("", Asset.Duration, GUILayout.Width(70));
                }
            }

            EditorGUILayout.Space(5);
            
            if (Asset.DurationPolicy == EffectsDurationPolicy.Duration ||
                Asset.DurationPolicy == EffectsDurationPolicy.Infinite)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Period", GUILayout.Width(100));
                    Asset.Period = EditorGUILayout.FloatField("", Asset.Period, GUILayout.Width(70));
                    
                    if (Asset.Period > 0)
                    {
                        EditorGUILayout.LabelField("-->", GUILayout.Width(20));
                        
                        EditorGUILayout.LabelField("Period GE", GUILayout.Width(60));
                        Asset.PeriodExecution =
                            (GameplayEffectAsset)EditorGUILayout.ObjectField("", Asset.PeriodExecution,
                                typeof(GameplayEffectAsset), false);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        void TagContainerGroup()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("GameplayEffect Tags", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("  ", GUILayout.Width(10));

                EditorGUILayout.BeginVertical();
                for (int j = 0; j < showTagGroups.Length; j++)
                {
                    showTagGroups[j] = EditorGUILayout.Foldout(showTagGroups[j], tagGroupNames[j]);
                    if (showTagGroups[j])
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        if (GUILayout.Button("+", GUILayout.Width(20)))
                        {
                            switch (j)
                            {
                                case 0:
                                    Asset.AssetTags = Asset.AssetTags.Append(tagChoices[0]).ToArray();
                                    break;
                                case 1:
                                    Asset.GrantedTags = Asset.GrantedTags.Append(tagChoices[0]).ToArray();
                                    break;
                                case 2:
                                    Asset.ApplicationRequiredTags =
                                        Asset.ApplicationRequiredTags.Append(tagChoices[0]).ToArray();
                                    break;
                                case 3:
                                    Asset.OngoingRequiredTags =
                                        Asset.OngoingRequiredTags.Append(tagChoices[0]).ToArray();
                                    break;
                                case 4:
                                    Asset.RemoveGameplayEffectsWithTags =
                                        Asset.RemoveGameplayEffectsWithTags.Append(tagChoices[0]).ToArray();
                                    break;
                            }
                        }

                        var tagArray = j switch
                        {
                            0 => Asset.AssetTags,
                            1 => Asset.GrantedTags,
                            2 => Asset.ApplicationRequiredTags,
                            3 => Asset.OngoingRequiredTags,
                            4 => Asset.RemoveGameplayEffectsWithTags,
                            _ => Array.Empty<string>()
                        };

                        for (var i = 0; i < tagArray.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal(GUILayout.Width(300));

                            int indexOfTag = tagChoices.IndexOf(tagArray[i]);
                            int idx = EditorGUILayout.Popup("", indexOfTag, tagChoices.ToArray());
                            tagArray[i] = tagChoices[idx];

                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                switch (j)
                                {
                                    case 0:
                                        Asset.AssetTags = tagArray.Where((source, index) => index != i).ToArray();
                                        break;
                                    case 1:
                                        Asset.GrantedTags = tagArray.Where((source, index) => index != i).ToArray();
                                        break;
                                    case 2:
                                        Asset.ApplicationRequiredTags =
                                            tagArray.Where((source, index) => index != i).ToArray();
                                        break;
                                    case 3:
                                        Asset.OngoingRequiredTags =
                                            tagArray.Where((source, index) => index != i).ToArray();
                                        break;
                                    case 4:
                                        Asset.RemoveGameplayEffectsWithTags =
                                            tagArray.Where((source, index) => index != i).ToArray();
                                        break;
                                }

                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(2);
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(5);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
        
        void CueGroup()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("Gameplay Cues", EditorStyles.boldLabel);

            if (Asset.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cue On Execute:", EditorStyles.boldLabel,GUILayout.Width(100));
                if(GUILayout.Button("+",GUILayout.Width(20)))
                    Asset.CueOnExecute.Add(default);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(3);
                for (var i = 0; i < Asset.CueOnExecute.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
                    Asset.CueOnExecute[i] =
                        EditorGUILayout.ObjectField(Asset.CueOnExecute[i], typeof(GameplayCue), false) as
                            GameplayCue;

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        Asset.CueOnExecute.RemoveAt(i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(2);
                }
            }
            else
            {
                //  Cue On Add
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cue On Add:", EditorStyles.boldLabel,GUILayout.Width(100));
                if(GUILayout.Button("+",GUILayout.Width(20)))
                    Asset.CueOnAdd.Add(default);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(3);
                for (var i = 0; i < Asset.CueOnAdd.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
                    Asset.CueOnAdd[i] =
                        EditorGUILayout.ObjectField(Asset.CueOnAdd[i], typeof(GameplayCue), false) as
                            GameplayCue;

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        Asset.CueOnAdd.RemoveAt(i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(2);
                }
                
                // Cue On Remove
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Cue On Remove:", EditorStyles.boldLabel,GUILayout.Width(100));
                if(GUILayout.Button("+",GUILayout.Width(20)))
                    Asset.CueOnRemove.Add(default);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(3);
                for (var i = 0; i < Asset.CueOnRemove.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
                    Asset.CueOnRemove[i] =
                        EditorGUILayout.ObjectField(Asset.CueOnRemove[i], typeof(GameplayCue), false) as
                            GameplayCue;

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        Asset.CueOnRemove.RemoveAt(i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(2);
                }
            }
            EditorGUILayout.EndVertical();

        }

        void ModifierGroup()
        {
            modifierList.OnGUI();
        }
        
        void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void OnEditModifier(int index, GameplayEffectModifier mod)
        {
            ModifierConfigEditor.OpenWindow(mod,modifier => modifierList.UpdateItem(index,modifier));
        }

        private float GetModifierElementHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 10;
        }
        
        void OnModifierDrawGUI(Rect rect,GameplayEffectModifier mod)
        {
            string attributeName = string.IsNullOrEmpty(mod.AttributeName)? "None":mod.AttributeName;
            string operation = mod.Operation.ToString();
            float value = mod.ModiferMagnitude;
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 500, EditorGUIUtility.singleLineHeight), 
                $"Attribute :{attributeName} | Operation:{operation} | Value:{value}");

            string mmcType = mod.MMC==null? "Empty!!!" : AssetDatabase.GetAssetPath(mod.MMC);
            string mmcInfo = $"MMC:{mmcType}";
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, 
                500, EditorGUIUtility.singleLineHeight),mmcInfo);
        }
    }
}