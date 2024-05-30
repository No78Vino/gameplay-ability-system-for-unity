using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GAS.Editor.General;
using GAS.General;
using GAS.General.Validation;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    [Serializable]
    public class AttributeSetConfig
    {
        public static AttributeSetAsset ParentAsset;

        private static IEnumerable AttributeChoices = new ValueDropdownList<string>();

        [HorizontalGroup("A")]
        [HorizontalGroup("A/R", order: 1)]
        [DisplayAsString(TextAlignment.Left, FontSize = 18)]
        [HideLabel]
        [InfoBox(GASTextDefine.ERROR_DuplicatedAttribute, InfoMessageType.Error,
            VisibleIf = "ExistDuplicatedAttribute")]
        [InfoBox(GASTextDefine.ERROR_Empty, InfoMessageType.Error, VisibleIf = "EmptyAttribute")]
        [InfoBox(GASTextDefine.ERROR_EmptyName, InfoMessageType.Error, VisibleIf = "EmptyAttributeSetName")]
        public string Name = "Unnamed";

        [Space]
        [ListDrawerSettings(ShowFoldout = true, ShowIndexLabels = false, ShowItemCount = false, ShowPaging = false,
            OnTitleBarGUI = "DrawAttributeNamesButtons")]
        [ValueDropdown("AttributeChoices", IsUniqueList = true)]
        [LabelText("Attributes")]
        [Searchable]
        public List<string> AttributeNames = new List<string>();

        private void DrawAttributeNamesButtons()
        {
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeNames = AttributeNames.OrderBy(x => x).ToList();
                ParentAsset.SaveAsset();
            }
        }

        [HorizontalGroup("A", Width = 50)]
        [HorizontalGroup("A/L", order: 0, Width = 50)]
        [Button(SdfIconType.Brush, "", ButtonHeight = 25)]
        public void EditName()
        {
            StringEditWindow.OpenWindow("AttributeSet Name", Name, Validations.ValidateVariableName, OnEditNameSuccess,
                "Edit AttributeSet Name");
        }

        private void OnEditNameSuccess(string newName)
        {
            Name = newName;
            ParentAsset.SaveAsset();
        }

        public static void SetAttributeChoices(List<string> attributeChoices)
        {
            var choices = new ValueDropdownList<string>();
            foreach (var attribute in attributeChoices)
            {
                choices.Add(attribute, attribute);
            }

            AttributeChoices = choices;
        }

        public bool EmptyAttribute()
        {
            return AttributeNames.Count == 0;
        }

        public bool EmptyAttributeSetName()
        {
            return string.IsNullOrEmpty(Name);
        }

        public bool ExistDuplicatedAttribute()
        {
            var duplicates = AttributeNames
                .Where(a => !string.IsNullOrEmpty(a))
                .GroupBy(a => a)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            return duplicates.Count > 0;
        }
    }

    [FilePath(GasDefine.GAS_ATTRIBUTE_SET_ASSET_PATH)]
    public class AttributeSetAsset : ScriptableSingleton<AttributeSetAsset>
    {
        [BoxGroup("Warning", order: -1)]
        [HideLabel]
        [ShowIf("ExistDuplicatedAttributeSetName")]
        [DisplayAsString(TextAlignment.Left, true)]
        public string ERROR_DuplicatedAttributeSet = "";

        [VerticalGroup("AttributeSetConfigs", order: 1)]
        [ListDrawerSettings(ShowFoldout = true,
            CustomAddFunction = "OnAddAttributeSet",
            CustomRemoveElementFunction = "OnRemoveElement",
            CustomRemoveIndexFunction = "OnRemoveIndex", OnTitleBarGUI = "DrawAttributeSetConfigsButtons")]
        [Searchable]
        public List<AttributeSetConfig> AttributeSetConfigs = new List<AttributeSetConfig>();

        private void DrawAttributeSetConfigsButtons()
        {
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeSetConfigs = AttributeSetConfigs.OrderBy(x => x.Name).ToList();
                SaveAsset();
            }
        }

        private void OnEnable()
        {
            AttributeSetConfig.ParentAsset = this;
            var asset = AttributeAsset.LoadOrCreate();
            AttributeSetConfig.SetAttributeChoices(asset?.AttributeNames);
        }

        public void SaveAsset()
        {
            EditorUtility.SetDirty(this);
            UpdateAsset(this);
            Save();
            Debug.Log("[EX] AttributeSetAsset save!");
        }

        [VerticalGroup("Generate AttributeSet Code", order: 0)]
        [GUIColor(0, 0.9f, 0)]
        [Button(SdfIconType.Upload, GASTextDefine.BUTTON_GenerateAttributeSetCode, ButtonHeight = 30, Expanded = true)]
        [InfoBox(GASTextDefine.ERROR_InElements, InfoMessageType.Error, VisibleIf = "ErrorInElements")]
        private void GenCode()
        {
            if (ExistDuplicatedAttributeSetName() || ErrorInElements())
            {
                EditorUtility.DisplayDialog("Warning", "Please check the warning message!\n" +
                                                       "Fix the AttributeSet Error!\n", "OK");
                return;
            }

            SaveAsset();
            AttributeSetClassGen.Gen();
            AssetDatabase.Refresh();
        }

        bool ErrorInElements()
        {
            return AttributeSetConfigs.Any(attribute =>
                attribute.EmptyAttribute() ||
                attribute.ExistDuplicatedAttribute() ||
                attribute.EmptyAttributeSetName());
        }

        bool ExistDuplicatedAttributeSetName()
        {
            var duplicates = AttributeSetConfigs
                .Where(a => !string.IsNullOrEmpty(a.Name))
                .GroupBy(a => a.Name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                var duplicatedAttributeSets = duplicates.Aggregate("", (current, d) => current + d + ",");
                duplicatedAttributeSets = duplicatedAttributeSets.Remove(duplicatedAttributeSets.Length - 1, 1);
                ERROR_DuplicatedAttributeSet =
                    string.Format(GASTextDefine.ERROR_DuplicatedAttributeSet, duplicatedAttributeSets);
            }

            return duplicates.Count > 0;
        }

        private void OnAddAttributeSet()
        {
            StringEditWindow.OpenWindow("AttributeSet Name", "", (newName) =>
                {
                    var validateVariableName = Validations.ValidateVariableName(newName);

                    if (!validateVariableName.IsValid)
                    {
                        return validateVariableName;
                    }

                    if (AttributeSetConfigs.Exists(x => x.Name == newName))
                    {
                        return ValidationResult.Invalid($"The name(\"{newName}\") already exists!");
                    }

                    return ValidationResult.Valid;
                },
                attributeSetName => AttributeSetConfigs.Add(new AttributeSetConfig() { Name = attributeSetName }),
                "Create new AttributeSet");
            GUIUtility.ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
        }

        private int OnRemoveElement(AttributeSetConfig attributeSet)
        {
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE AttributeSet:{attributeSet.Name}?",
                "Yes", "No");

            if (!result) return -1;

            Debug.Log($"[EX] AttributeSet Asset remove element:{attributeSet.Name} !");
            SaveAsset();
            return AttributeSetConfigs.IndexOf(attributeSet);
        }

        private int OnRemoveIndex(int index)
        {
            var attributeSet = AttributeSetConfigs[index];
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE AttributeSet:{attributeSet.Name}?",
                "Yes", "No");

            if (!result) return -1;

            AttributeSetConfigs.RemoveAt(index);
            Debug.Log($"[EX] Attribute Asset remove element:{attributeSet.Name} !");
            SaveAsset();
            return index;
        }
    }
}