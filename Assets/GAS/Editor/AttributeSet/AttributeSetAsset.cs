#if UNITY_EDITOR
namespace GAS.Editor.AttributeSet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using GAS.Editor.Attribute;
    using GAS.Editor.GameplayAbilitySystem;
    using GAS.Editor.General;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;
    
    [Serializable]
    public class AttributeSetConfig
    {
        public static AttributeSetAsset ParentAsset;

        private static IEnumerable AttributeChoices = new ValueDropdownList<string>();
        private const string ERROR_DuplicatedAttribute = "<size=16><b>Exist Duplicated Attribute!</b></size>";
        private const string ERROR_Empty = "<size=16><b>It's Empty!</b></size>";
        private const string ERROR_EmptyName = "<size=16><b>AttributeSet'name can't Empty!</b></size>";
        
        [HorizontalGroup("A")]
        [HorizontalGroup("A/R", order:1)]
        [DisplayAsString(TextAlignment.Left,FontSize = 18)]
        [HideLabel]
        [InfoBox(ERROR_DuplicatedAttribute,InfoMessageType.Error,VisibleIf = "ExistDuplicatedAttribute")]
        [InfoBox(ERROR_Empty,InfoMessageType.Error,VisibleIf = "EmptyAttribute")]
        [InfoBox(ERROR_EmptyName,InfoMessageType.Error,VisibleIf = "EmptyAttributeSetName")]
        public string Name;
        
        [Space]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false,ShowPaging = false)]
        [ValueDropdown("AttributeChoices")]
        [LabelText("Attributes")]
        [Searchable]
        public List<string> AttributeNames;

        [HorizontalGroup("A", Width = 50)]
        [HorizontalGroup("A/L", order:0,Width = 50)]
        [Button(SdfIconType.Brush,"",ButtonHeight = 25)]
        public void EditName()
        {
            StringEditWindow.OpenWindow(Name, OnEditNameSuccess, "AttributeSet Name");
        }
        
        private void OnEditNameSuccess(string newName)
        {
            Name = newName;
            ParentAsset.Save();
        }
        
        public static void SetAttributeChoices(List<string> attributeChoices)
        {
            var choices = new ValueDropdownList<string>();
            foreach (var attribute in attributeChoices)
            {
                choices.Add(attribute,attribute);
            }
            AttributeChoices = choices;
        }

        public bool EmptyAttribute()
        {
            return AttributeNames==null || AttributeNames.Count == 0;
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

    public class AttributeSetAsset : ScriptableObject
    {
        private const string ERROR_InElements = "<size=16><b><color=orange>Please fix the errors in the AttributeSet!</color></b></size>";
        
        [BoxGroup("Warning", order: -1)]
        [HideLabel]
        [ShowIf("ExistDuplicatedAttributeSetName")]
        [DisplayAsString(TextAlignment.Left, true)]
        public string ERROR_DuplicatedAttributeSet = "";
        
        [VerticalGroup("AttributeSetConfigs",order:1)]
        [ListDrawerSettings( Expanded = true,
            CustomRemoveElementFunction = "OnRemoveElement",
            CustomRemoveIndexFunction = "OnRemoveIndex")]
        [Searchable]
        public List<AttributeSetConfig> AttributeSetConfigs;

        private void OnEnable()
        {
            AttributeSetConfig.ParentAsset = this;
            var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH);
            AttributeSetConfig.SetAttributeChoices(asset?.AttributeNames);
        }

        public void Save()
        {
            Debug.Log("[EX] AttributeSetAsset save!");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        [VerticalGroup("Generate AttributeSet Code",order:0)]
        [GUIColor(0,0.9f,0)]
        [Button(SdfIconType.Upload,"Generate AttributeSet Code",ButtonHeight = 30, Expanded = true)]
        [InfoBox(ERROR_InElements,InfoMessageType.Error,VisibleIf = "ErrorInElements")]
        private void GenCode()
        {
            if(ExistDuplicatedAttributeSetName() || ErrorInElements())
            {
                EditorUtility.DisplayDialog("Warning", "Please check the warning message!\n" +
                                                       "Fix the AttributeSet Error!\n", "OK");
                return;
            }
            
            Save();
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
                ERROR_DuplicatedAttributeSet = $"<size=16><b><color=orange>Exist Duplicated AttributeSet!\n" +
                                               $"<color=white> ->  {duplicatedAttributeSets}</color></color></b></size>";
            }
            return duplicates.Count > 0;
        }
        
        private int OnRemoveElement(AttributeSetConfig attributeSet)
        {
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE AttributeSet:{attributeSet.Name}?",
                "Yes", "No");

            if (!result) return -1;

            Debug.Log($"[EX] AttributeSet Asset remove element:{attributeSet.Name} !");
            Save();
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
            Save();
            return index;
        }
    }
}
#endif