using System;

#if UNITY_EDITOR
namespace GAS.Editor.AbilitySystemComponent
{
    using System.Collections.Generic;
    using System.Linq;
    using AttributeSet;
    using General;
    using Tags;
    using GAS.Runtime.Ability;
    using Runtime.Component;
    using GAS.Runtime.Tags;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(AbilitySystemComponentPreset))]
    public class AbilitySystemComponentPresetEditor : UnityEditor.Editor
    {
        private ScriptableObjectReorderableList<AbilityAsset> _baseAbilities;
        
        private ArraySetFromChoicesAsset<GameplayTag> _baseTags;
        private List<GameplayTag> tagChoices = new List<GameplayTag>();
        
        private ArraySetFromChoicesAsset<string> _baseAttributeSets;
        private List<string> _attributeSetChoices = new List<string>();
        
        private AbilitySystemComponentPreset Asset => (AbilitySystemComponentPreset)target;

        private GUIStyle greenButtonStyle;

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
            _baseTags = new ArraySetFromChoicesAsset<GameplayTag>(
                Asset.BaseTags, tagChoices, "BaseTags", null);

            _attributeSetChoices = AttributeSetEditorUtil.GetAttributeSetChoice();
            _baseAttributeSets = new ArraySetFromChoicesAsset<string>(
                Asset.AttributeSets, _attributeSetChoices, "AttributeSets", null);
                
            _baseAbilities = new ScriptableObjectReorderableList<AbilityAsset>(
                Asset.BaseAbilities,
                "BaseAbilities");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            ConfigErrorTip();
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            
            EditorGUILayout.BeginVertical(GUILayout.Width(300f));
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", GUILayout.Width(100f));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name, GUILayout.Width(200f));
            }
            EditorGUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(100f));
                Asset.Description = EditorGUILayout.TextField("", Asset.Description,GUILayout.Width(200f));
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            _baseAttributeSets.OnGUI();

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(300)))
            {
                _baseTags.OnGUI();  
            }
            GUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(300)))
            {
                 _baseAbilities.OnGUI();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save",greenButtonStyle)) Save();

            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            // Save BaseTags
            Asset.BaseTags = _baseTags.GetItemList().ToArray();

            // Save BaseAbilities
            Asset.BaseAbilities = _baseAbilities.GetItemList().ToArray();

            // Save BaseAttributeSets
            Asset.AttributeSets = _baseAttributeSets.GetItemList().ToArray();
            
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ConfigErrorTip()
        {
            var tags = _baseTags.GetItemList();
            var isTagDuplicate = tags.Any(tag => tags.FindAll(x => x == tag).Count > 1);
            if (isTagDuplicate)
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField("<size=16><b><color=red>There are duplicate tags!</color></b></size>",
                        new GUIStyle(GUI.skin.label) { richText = true });
                }

            var abilities = _baseAbilities.GetItemList();
            var isAbilityNull = abilities.Any(ability => ability == null);
            var isAbilityDuplicate = abilities.Any(ability => abilities.FindAll(x => x == ability).Count > 1);
            if (isAbilityDuplicate || isAbilityNull)
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                {
                    EditorGUILayout.LabelField(
                        "<size=16><b><color=red>There are duplicate abilities or Null abilities!</color></b></size>",
                        new GUIStyle(GUI.skin.label) { richText = true });
                }
        }
    }
}
#endif