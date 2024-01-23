#if UNITY_EDITOR
namespace GAS.Editor.GameplayAbilitySystem
{
    using System;
    using System.Collections.Generic;
    using GAS.Runtime.Ability;
    using GAS.Runtime.Effects;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;
    
    public class GASWatcher : OdinEditorWindow
    {
        private const string BOXGROUP_TIPS = "Tips";
        private const string BOXGROUP_TIPS_RUNNINGTIP = "Tips/Running tip";
        private const string BOXGROUP_ASC = "Ability System Components";

        private const int WIDTH_SELECTOR_MENU = 125;
        private const int WIDTH_SEPARATOR = 5;

        private const int WIDTH_IID = 100;
        private const int WIDTH_ATTRIBUTE = 250;
        private const int WIDTH_ATTRIBUTE_NAME = 150;
        private const int WIDTH_ATTRIBUTE_VALUE = 100;
        private const int WIDTH_ABILITY = 100;
        private const int WIDTH_GAMEPLAYEFFECT = 200;
        private const int WIDTH_FIXED_TAG = 200;
        private const int WIDTH_DYNAMIC_TAG = 200;


        [HideLabel] [DisplayAsString(TextAlignment.Center, true)]
        public string windowTitle = "<size=18><b>EX Gameplay Ability System Watcher</b></size>";

        [BoxGroup(BOXGROUP_TIPS)] [HideLabel] [DisplayAsString(TextAlignment.Left, true)]
        public string tips = "This window is used to monitor the runtime state of the Gameplay Ability System. \n" +
                             "It is recommended to open this window in the editor when debugging the Gameplay Ability System. ";

        [BoxGroup(BOXGROUP_TIPS_RUNNINGTIP, false)]
        [HideLabel]
        [DisplayAsString(TextAlignment.Center, true)]
        [HideIf("IsPlaying")]
        public string onlyForGameRunning =
            "<size=16><b><color=yellow>This monitor is only available when the game is running.</color></b></size>";

        [ShowIf("IsPlaying")] [OnInspectorGUI("OnDrawGasHostGUI")] [DisplayAsString(TextAlignment.Center)] [HideLabel]
        public string GASHost = "";

        private Vector2 ascScrollPos;
        private Vector2 menuScrollPos;
        private GUIStyle propertyTitleStyle;
        private List<Rect> ascScrollPosList = new List<Rect>();

        private bool IsPlaying => Application.isPlaying;

        private void Awake()
        {
            propertyTitleStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
        }

        private void Update()
        {
            if (IsPlaying) Repaint();
        }

        [MenuItem("EX-GAS/GAS Runtime Watcher", priority = 3)]
        private static void OpenWindow()
        {
            var window = GetWindow<GASWatcher>();
            window.Show();
        }

        [Obsolete("Obsolete")]
        private void OnDrawGasHostGUI()
        {
            if (!IsPlaying) return;
            ascScrollPosList.Clear();
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            PropertyBar();

            ascScrollPos = EditorGUILayout.BeginScrollView(ascScrollPos, GUI.skin.box);
            foreach (var iasc in Core.GameplayAbilitySystem.GAS.AbilitySystemComponents)
            {
                var asc = (Runtime.Component.AbilitySystemComponent)iasc;

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.BeginHorizontal(
                        GUILayout.Width(
                            WIDTH_IID + WIDTH_ATTRIBUTE + WIDTH_FIXED_TAG + WIDTH_DYNAMIC_TAG +
                            WIDTH_ABILITY + WIDTH_GAMEPLAYEFFECT + WIDTH_SEPARATOR * 6));
                    // IID & GameObject
                    DrawIidAndGameObject(asc);
                    EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
                    // Attributes
                    DrawAttribute(asc);
                    EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
                    // FixedTags
                    DrawFixedTags(asc);
                    EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
                    // DynamicAddedTags
                    DrawDynamicTags(asc);
                    EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
                    // Abilities
                    DrawAbility(asc);
                    EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
                    // ActiveGameplayEffect
                    DrawGameplayEffect(asc);

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(
                        "//////////////////////////////////////////////////////////////////////////" +
                        "//////////////////////////////////////////////////////////////////////////" +
                        "//////////////////////////////////////////////////////////////////////////" +
                        "//////////////////////////////////////////////////////////////////////////" +
                        "//////////////////////////////////////////////////////////////////////////",
                        GUILayout.Width(WIDTH_IID + WIDTH_ATTRIBUTE + WIDTH_FIXED_TAG + WIDTH_DYNAMIC_TAG +
                                        WIDTH_ABILITY + WIDTH_GAMEPLAYEFFECT + WIDTH_SEPARATOR * 6));
                }

                ascScrollPosList.Add(GUILayoutUtility.GetLastRect());
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            AscMenuBar();
            EditorGUILayout.EndHorizontal();
        }

        private void PropertyBar()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            // IID & GameObject
            EditorGUILayout.LabelField(
                "<color=white><size=13><b>IID</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_IID));
            EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
            // Attributes
            EditorGUILayout.LabelField("<color=white><size=13><b>Attributes</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_ATTRIBUTE));
            EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
            // FixedTags
            EditorGUILayout.LabelField("<color=white><size=13><b>FixedTags</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_FIXED_TAG));
            EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
            // DynamicAddedTags
            EditorGUILayout.LabelField("<color=white><size=13><b>DynamicAddedTags</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_DYNAMIC_TAG));
            EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
            // Abilities
            EditorGUILayout.LabelField("<color=white><size=13><b>Abilities</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_ABILITY));
            EditorGUILayout.LabelField("|", GUILayout.Width(WIDTH_SEPARATOR));
            // ActiveGameplayEffect
            EditorGUILayout.LabelField("<color=white><size=13><b>ActiveGameplayEffect</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_GAMEPLAYEFFECT));
            EditorGUILayout.EndHorizontal();
        }

        void AscMenuBar()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_SELECTOR_MENU));
            menuScrollPos = EditorGUILayout.BeginScrollView(menuScrollPos, GUI.skin.box);
            EditorGUILayout.LabelField("<color=white><size=13><b>NAVI</b></size></color>",
                propertyTitleStyle, GUILayout.Width(WIDTH_SELECTOR_MENU - 10));

            for (var i = 0; i < Core.GameplayAbilitySystem.GAS.AbilitySystemComponents.Count; i++)
            {
                var iasc = Core.GameplayAbilitySystem.GAS.AbilitySystemComponents[i];
                var asc = (Runtime.Component.AbilitySystemComponent)iasc;
                if (GUILayout.Button($"{asc.GetInstanceID()}", GUILayout.Width(WIDTH_SELECTOR_MENU - 10)))
                {
                    var secondComponentRect = ascScrollPosList[i] ;
                    var screenPos =
                        GUIUtility.GUIToScreenPoint(new Vector2(secondComponentRect.x, secondComponentRect.y));
                    ascScrollPos = GUIUtility.ScreenToGUIPoint(screenPos);
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        [Obsolete("Obsolete")]
        private void DrawIidAndGameObject(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_IID));
            EditorGUILayout.LabelField(asc.GetInstanceID().ToString(), GUILayout.Width(WIDTH_IID));
            EditorGUILayout.ObjectField(asc.gameObject, typeof(GameObject), GUILayout.Width(WIDTH_IID));
            EditorGUILayout.EndVertical();
        }

        private void DrawAttribute(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_ATTRIBUTE));
            if (asc.AttributeSetContainer.Sets.Count == 0)
            {
                EditorGUILayout.LabelField(" ", GUILayout.Width(WIDTH_ATTRIBUTE));
            }
            foreach (var kv in asc.AttributeSetContainer.Sets)
            {
                var setName = kv.Key;
                EditorGUILayout.LabelField($"Set:{setName} ↓", GUILayout.Width(WIDTH_ATTRIBUTE));
                foreach (var attributeName in kv.Value.AttributeNames)
                {
                    var attr = kv.Value[attributeName];
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(WIDTH_ATTRIBUTE));
                    EditorGUILayout.LabelField($"--{attributeName}", GUILayout.Width(WIDTH_ATTRIBUTE_NAME));
                    EditorGUILayout.LabelField(
                        $"= {attr.CurrentValue}({attr.BaseValue} + {attr.CurrentValue - attr.BaseValue})",
                        GUILayout.Width(WIDTH_ATTRIBUTE_VALUE));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawFixedTags(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_FIXED_TAG));
            if (asc.GameplayTagAggregator.FixedTags.Count == 0)
            {
                EditorGUILayout.LabelField(" ", GUILayout.Width(WIDTH_FIXED_TAG));
            }
            foreach (var tag in asc.GameplayTagAggregator.FixedTags)
                EditorGUILayout.LabelField(tag.Name, GUILayout.Width(WIDTH_FIXED_TAG));
            EditorGUILayout.EndVertical();
        }

        private void DrawDynamicTags(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_DYNAMIC_TAG));
            if (asc.GameplayTagAggregator.DynamicAddedTags.Count == 0)
            {
                EditorGUILayout.LabelField(" ", GUILayout.Width(WIDTH_DYNAMIC_TAG));
            }
            foreach (var kv in asc.GameplayTagAggregator.DynamicAddedTags)
            {
                var tagName = kv.Key.Name;
                EditorGUILayout.LabelField($"{tagName} ↓ ", GUILayout.Width(WIDTH_DYNAMIC_TAG));
                foreach (var obj in kv.Value)
                    if (obj is GameplayEffectSpec spec)
                    {
                        var owner = spec.Owner;
                        EditorGUILayout.LabelField($"--From:{owner.GetInstanceID()}-GE",
                            GUILayout.Width(WIDTH_DYNAMIC_TAG));
                    }
                    else if (obj is AbilitySpec ability)
                    {
                        var owner = ability.Owner;
                        EditorGUILayout.LabelField($"--From:{owner.GetInstanceID()}-Ability",
                            GUILayout.Width(WIDTH_DYNAMIC_TAG));
                    }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawAbility(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_ABILITY));
            if (asc.AbilityContainer.AbilitySpecs().Count == 0)
            {
                EditorGUILayout.LabelField(" ", GUILayout.Width(WIDTH_ABILITY));
            }
            foreach (var ability in asc.AbilityContainer.AbilitySpecs())
                EditorGUILayout.LabelField($"{ability.Key}", GUILayout.Width(WIDTH_ABILITY));
            EditorGUILayout.EndVertical();
        }

        private void DrawGameplayEffect(Runtime.Component.AbilitySystemComponent asc)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(WIDTH_GAMEPLAYEFFECT));
            var activeGE = asc.GameplayEffectContainer.GetActiveGameplayEffects();
            if (activeGE.Count == 0)
            {
                EditorGUILayout.LabelField(" ", GUILayout.Width(WIDTH_GAMEPLAYEFFECT));
            }

            foreach (var ge in activeGE)
            {
                string geState = $"{ge.GameplayEffect.Asset.Name};DUR:{ge.DurationRemaining()}/{ge.Duration}(s)";
                EditorGUILayout.LabelField(geState, GUILayout.Width(WIDTH_GAMEPLAYEFFECT));
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif