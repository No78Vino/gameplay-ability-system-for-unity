using System;
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
    [FilePath(GasDefine.GAS_ATTRIBUTE_ASSET_PATH)]
    public class AttributeAsset : ScriptableSingleton<AttributeAsset>
    {
        [BoxGroup("Warning", order: -1)]
        [HideLabel]
        [ShowIf("ExistDuplicatedAttribute")]
        [DisplayAsString(TextAlignment.Left, true)]
        [NonSerialized]
        public string Warning_DuplicatedAttribute = "";
        
        [VerticalGroup("Attributes", order: 1)]
        [ListDrawerSettings(
            ShowFoldout = true,
            CustomRemoveElementFunction = "OnRemoveElement",
            CustomRemoveIndexFunction = "OnRemoveIndex",
            CustomAddFunction = "OnAddAttribute",
            ShowPaging = false, OnTitleBarGUI = "DrawAttributeButtons")]
        [Searchable]
        [OnValueChanged("@OnValueChanged()", true)]
        [OnCollectionChanged(after: "@OnCollectionChanged()")]
        public List<AttributeAccessor> attributes = new List<AttributeAccessor>();

        private void OnValueChanged()
        {
            Debug.Log("OnListChanged");
            SaveAsset();
        }

        private void OnCollectionChanged()
        {
            Debug.Log("OnCollectionChanged");
            SaveAsset();
        }

        private void DrawAttributeButtons()
        {
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                attributes = attributes.OrderBy(x => x.Name).ToList();
                SaveAsset();
            }
        }

        public List<string> AttributeNames =>
            (from attr in attributes where !string.IsNullOrEmpty(attr.Name) select attr.Name).ToList();

        private void OnEnable()
        {
            AttributeAccessor.ParentAsset = this;
        }

        [VerticalGroup("Gen Code", order: 0)]
        [GUIColor(0, 0.9f, 0)]
        [Button(SdfIconType.Upload, GASTextDefine.BUTTON_GenerateAttributeCollection, ButtonHeight = 30,
            Expanded = true)]
        [InfoBox(GASTextDefine.TIP_Warning_EmptyAttribute, InfoMessageType.Error, VisibleIf = "ExistEmptyAttribute")]
        void GenCode()
        {
            if (ExistEmptyAttribute() || ExistDuplicatedAttribute())
            {
                EditorUtility.DisplayDialog("Warning", "Please check the warning message!\n" +
                                                       "Fix the Attribute Error!\n", "OK");
                return;
            }

            SaveAsset();
            AttributeCollectionGen.Gen();
            AssetDatabase.Refresh();
        }

        private void SaveAsset()
        {
            Debug.Log("[EX] Attribute Asset save!");
            EditorUtility.SetDirty(this);
            UpdateAsset(this);
            Save();
        }

        private int OnRemoveElement(AttributeAccessor attribute)
        {
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE Attribute:{attribute.Name}?",
                "Yes", "No");

            if (!result) return -1;

            Debug.Log($"[EX] Attribute Asset remove element:{attribute.Name} !");
            SaveAsset();
            return attributes.IndexOf(attribute);
        }

        private int OnRemoveIndex(int index)
        {
            var attribute = attributes[index];
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE Attribute:{attribute.Name}?",
                "Yes", "No");

            if (!result) return -1;

            attributes.RemoveAt(index);
            Debug.Log($"[EX] Attribute Asset remove element:{attribute.Name} !");
            SaveAsset();
            return index;
        }

        private void OnAddAttribute()
        {
            StringEditWindow.OpenWindow("创建新属性", null, newName =>
            {
                var validateVariableName = Validations.ValidateVariableName(newName);

                if (validateVariableName.IsValid == false)
                {
                    return validateVariableName;
                }

                if (attributes.Exists(x => x.Name == newName))
                {
                    return ValidationResult.Invalid($"属性名已存在: \"{newName}\"!");
                }

                return ValidationResult.Valid;
            }, x =>
            {
                attributes.Add(new AttributeAccessor { Name = x });
                SaveAsset();
                Debug.Log("[EX] Attribute Asset add element!");
            });
            GUIUtility.ExitGUI(); // In order to solve: "EndLayoutGroup: BeginLayoutGroup must be called first."
        }

        private bool ExistEmptyAttribute()
        {
            return attributes.Any(attribute => string.IsNullOrEmpty(attribute.Name));
        }

        private bool ExistDuplicatedAttribute()
        {
            var duplicates = attributes
                .Where(a => !string.IsNullOrEmpty(a.Name))
                .GroupBy(a => a.Name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                var duplicatedAttributes = duplicates.Aggregate("", (current, d) => current + d + ",");
                duplicatedAttributes = duplicatedAttributes.Remove(duplicatedAttributes.Length - 1, 1);
                Warning_DuplicatedAttribute =
                    string.Format(GASTextDefine.TIP_Warning_DuplicatedAttribute, duplicatedAttributes);
            }

            return duplicates.Count > 0;
        }

        [Serializable]
        public class AttributeAccessor
        {
            private const int LabelWidth = 100;
            public static AttributeAsset ParentAsset;

            private string DisplayName => $"{Name} - {Comment}";

            [FoldoutGroup("$DisplayName", false)]
            [LabelText("属性名"), LabelWidth(LabelWidth)]
            [DelayedProperty]
            [ValidateInput("@OnNameChanged($value)", "Attribute name is invalid!")]
            [PropertyOrder(1)]
            public string Name = "Unnamed";

            private bool OnNameChanged(string value)
            {
                if (ParentAsset == null) return true;

                return Validations.IsValidVariableName(value);
            }

            [FoldoutGroup("$DisplayName")]
            [DelayedProperty]
            [LabelText("备注"), LabelWidth(LabelWidth)]
            [PropertyOrder(2)]
            public string Comment = "";

            [FoldoutGroup("$DisplayName")]
            [LabelText("计算模式"), LabelWidth(LabelWidth)]
            [PropertyOrder(3)]
            [OnValueChanged("@OnCalculateModeChanged()")]
            [HorizontalGroup("$DisplayName/d")]
            public CalculateMode CalculateMode = CalculateMode.Stacking;

            private void OnCalculateModeChanged()
            {
                if (CalculateMode is CalculateMode.MinValueOnly or CalculateMode.MaxValueOnly)
                {
                    SupportedOperation = SupportedOperation.Override;
                }
            }

            [FoldoutGroup("$DisplayName")]
            [LabelText("支持运算"), LabelWidth(LabelWidth)]
            [PropertyOrder(4)]
            [DisableIf(
                "@CalculateMode == GAS.Runtime.CalculateMode.MinValueOnly || CalculateMode == GAS.Runtime.CalculateMode.MaxValueOnly")]
            [HorizontalGroup("$DisplayName/d")]
            public SupportedOperation SupportedOperation = SupportedOperation.All;

            [FoldoutGroup("$DisplayName")]
            [LabelText("默认值"), LabelWidth(LabelWidth)]
            [DelayedProperty]
            [PropertyOrder(5)]
            [HorizontalGroup("$DisplayName/Values")]
            public float DefaultValue = 0f;

            [FoldoutGroup("$DisplayName")]
            [LabelText("最小值"), LabelWidth(40)]
            [DelayedProperty]
            [PropertyOrder(6)]
            [HorizontalGroup("$DisplayName/Values")]
            [ToggleLeft]
            public bool LimitMinValue = false;

            [FoldoutGroup("$DisplayName")]
            [HideLabel]
            [DelayedProperty]
            [PropertyOrder(6)]
            [EnableIf("LimitMinValue")]
            [HorizontalGroup("$DisplayName/Values")]
            public float MinValue = float.MinValue;

            [FoldoutGroup("$DisplayName")]
            [LabelText("最大值"), LabelWidth(50)]
            [DelayedProperty]
            [PropertyOrder(6)]
            [HorizontalGroup("$DisplayName/Values")]
            public bool LimitMaxValue = false;

            [FoldoutGroup("$DisplayName")]
            [HideLabel]
            [DelayedProperty]
            [PropertyOrder(7)]
            [EnableIf("LimitMaxValue")]
            [HorizontalGroup("$DisplayName/Values")]
            public float MaxValue = float.MaxValue;
        }
    }
}