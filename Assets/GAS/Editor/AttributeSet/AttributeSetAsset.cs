using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GAS.Editor.General;
using GAS.General;
using GAS.General.Validation;
using GAS.Runtime;
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
        [InfoBox(GASConstDefine.ERROR_DuplicatedAttribute, InfoMessageType.Error,
            VisibleIf = "ExistDuplicatedAttribute")]
        [InfoBox(GASConstDefine.ERROR_Empty, InfoMessageType.Error, VisibleIf = "EmptyAttribute")]
        [InfoBox(GASConstDefine.ERROR_EmptyName, InfoMessageType.Error, VisibleIf = "EmptyAttributeSetName")]
        [OnValueChanged(nameof(OnNameChanged))]
        public string Name = "Unnamed";

        [HideInInspector] public int Code;

        [HorizontalGroup("B")]
        [HorizontalGroup("B/L", order: 0, Width = 150)]
        [ListDrawerSettings(ShowFoldout = false,
            ShowIndexLabels = false,
            ShowItemCount = false,
            ShowPaging = false,
            OnTitleBarGUI = nameof(DrawAttributeNamesButtons))]
        [ValueDropdown("AttributeChoices", IsUniqueList = true)]
        [LabelText("Attributes")]
        [OnValueChanged(nameof(OnAttritesChange))]
        public List<string> AttributeNames = new();

        [HorizontalGroup("B/L")]
        [LabelText(" ")]
        [TableList(AlwaysExpanded = true,
            DrawScrollView = false,
            IsReadOnly = true,
            HideToolbar = true)]
        public List<AttrInASCustomConfig> Attributes = new();

        private void OnNameChanged()
        {
            Code = Name.GetHashCode();
        }

        private void DrawAttributeNamesButtons()
        {
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeNames = AttributeNames.OrderBy(x => x).ToList();
                OnAttritesChange();
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
            foreach (var attribute in attributeChoices) choices.Add(attribute, attribute);

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

        public int GetCode()
        {
            if (Code == 0) Code = Name.GetHashCode();
            return Code;
        }

        private void OnAttritesChange()
        {
            var lastData = new List<AttrInASCustomConfig>(Attributes);
            Attributes.Clear();
            foreach (var name in AttributeNames)
            {
                var item = new AttrInASCustomConfig();
                item.AttrCode = name.GetHashCode();
                item.attrName = name;
                foreach (var cfg in lastData)
                    if (cfg.AttrCode == item.AttrCode)
                    {
                        item = cfg;
                        break;
                    }

                Attributes.Add(item);
            }

            ParentAsset.SaveAsset();
        }
    }

    [SingletonFilePath(GasDefine.GAS_ATTRIBUTE_SET_ASSET_PATH)]
    public class AttributeSetAsset : ScriptableSingleton<AttributeSetAsset>
    {
        [BoxGroup("Warning", order: -1)]
        [HideLabel]
        [ShowIf(nameof(ExistDuplicatedAttributeSetName))]
        [DisplayAsString(TextAlignment.Left, true)]
        public string ERROR_DuplicatedAttributeSet = "";

        [VerticalGroup("AttributeSetConfigs", 1)]
        [ListDrawerSettings(ShowFoldout = false,
            CustomAddFunction = nameof(OnAddAttributeSet),
            CustomRemoveElementFunction = nameof(OnRemoveElement),
            CustomRemoveIndexFunction = nameof(OnRemoveIndex),
            OnTitleBarGUI = nameof(DrawAttributeSetConfigsButtons))]
        [Searchable]
        public List<AttributeSetConfig> AttributeSetConfigs = new();

        private void OnEnable()
        {
            AttributeSetConfig.ParentAsset = this;
            var asset = AttributeAsset.LoadOrCreate();
            AttributeSetConfig.SetAttributeChoices(asset?.AttributeNames);
        }

        private void DrawAttributeSetConfigsButtons()
        {
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeSetConfigs = AttributeSetConfigs.OrderBy(x => x.Name).ToList();
                SaveAsset();
            }
        }

        [HorizontalGroup("GEN",order:0,width:100)]
        [GUIColor(0.8f, 0.6f, 0.2f)]
        [Button(SdfIconType.Save, "保存", ButtonHeight = 30, Expanded = true)]
        public void SaveAsset()
        {
            EditorUtility.SetDirty(this);
            UpdateAsset(this);
            Save();
            Debug.Log("[EX] AttributeSetAsset save!");
        }

        [HorizontalGroup("GEN")]
        [GUIColor(0, 0.9f, 0)]
        [Button(SdfIconType.Upload, GASConstDefine.BUTTON_GenerateAttributeSetCode, ButtonHeight = 30, Expanded = true)]
        [InfoBox(GASConstDefine.ERROR_InElements, InfoMessageType.Error, VisibleIf = "ErrorInElements")]
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

        private bool ErrorInElements()
        {
            return AttributeSetConfigs.Any(attribute =>
                attribute.EmptyAttribute() ||
                attribute.ExistDuplicatedAttribute() ||
                attribute.EmptyAttributeSetName());
        }

        private bool ExistDuplicatedAttributeSetName()
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
                    string.Format(GASConstDefine.ERROR_DuplicatedAttributeSet, duplicatedAttributeSets);
            }

            return duplicates.Count > 0;
        }

        private void OnAddAttributeSet()
        {
            StringEditWindow.OpenWindow("AttributeSet Name", "", newName =>
                {
                    var validateVariableName = Validations.ValidateVariableName(newName);

                    if (!validateVariableName.IsValid) return validateVariableName;

                    if (AttributeSetConfigs.Exists(x => x.Name == newName))
                        return ValidationResult.Invalid($"The name(\"{newName}\") already exists!");

                    return ValidationResult.Valid;
                },
                attributeSetName => AttributeSetConfigs.Add(new AttributeSetConfig { Name = attributeSetName }),
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