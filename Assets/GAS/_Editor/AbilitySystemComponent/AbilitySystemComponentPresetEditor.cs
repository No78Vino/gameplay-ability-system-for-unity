using System.Collections.Generic;
using System.Linq;
using GAS.Editor.General;
using GAS.Editor.Tags;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.AbilitySystemComponent
{
    [CustomEditor(typeof(AbilitySystemComponentPreset))]
    public class AbilitySystemComponentPresetEditor : UnityEditor.Editor
    {
        private ScriptableObjectReorderableList<AbilityAsset> _baseAbilities;
        private ArraySetFromChoicesAsset<GameplayTag> _baseTags;
        private List<GameplayTag> tagChoices = new List<GameplayTag>();
        private AbilitySystemComponentPreset Asset => (AbilitySystemComponentPreset)target;

        private void OnEnable()
        {
            tagChoices = TagEditorUntil.GetTagChoice();
            _baseTags = new ArraySetFromChoicesAsset<GameplayTag>(
                Asset.BaseTags, tagChoices, "BaseTags", null);

            _baseAbilities = new ScriptableObjectReorderableList<AbilityAsset>(
                Asset.BaseAbilities,
                "BaseAbilities");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Name", GUILayout.Width(100f));
                Asset.Name = EditorGUILayout.TextField("", Asset.Name);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(100f));
                Asset.Description = EditorGUILayout.TextField("", Asset.Description);
            }

            ConfigErrorTip();

            GUILayout.Space(5f);
            _baseTags.OnGUI();
            GUILayout.Space(3f);
            _baseAbilities.OnGUI();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save")) Save();

            EditorGUILayout.EndVertical();
        }

        private void Save()
        {
            // Save BaseTags
            Asset.BaseTags = _baseTags.GetItemList().ToArray();

            // Save BaseAbilities
            Asset.BaseAbilities = _baseAbilities.GetItemList().ToArray();

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