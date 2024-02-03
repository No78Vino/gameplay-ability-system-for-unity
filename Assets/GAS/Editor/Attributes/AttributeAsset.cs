#if UNITY_EDITOR
namespace GAS.Editor.Attribute
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GAS.Editor.General;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;
    using GAS.General;
    using GAS.Editor.Attributes;
    
    public class AttributeAsset : ScriptableObject
    {
        [BoxGroup("Warning", order: -1)]
        [HideLabel]
        [ShowIf("ExistDuplicatedAttribute")]
        [DisplayAsString(TextAlignment.Left, true)]
        public string Warning_DuplicatedAttribute = "";
        
        [VerticalGroup("Attributes", order: 1)]
        [ListDrawerSettings(
            Expanded = true,
            CustomRemoveElementFunction = "OnRemoveElement",
            CustomRemoveIndexFunction = "OnRemoveIndex",
            CustomAddFunction = "OnAddAttribute",
            ShowPaging = false)]
        [Searchable]
        [OnValueChanged("Save")]
        public List<AttributeAccessor> attributes = new List<AttributeAccessor>();
        
        public List<string> AttributeNames => (from attr in attributes where !string.IsNullOrEmpty(attr.Name) select attr.Name).ToList();

        private void OnEnable()
        {
            AttributeAccessor.ParentAsset = this;
        }

        [VerticalGroup("Gen Code", order: 0)]
        [GUIColor(0,0.9f,0)]
        [Button(SdfIconType.Upload,GASTextDefine.BUTTON_GenerateAttributeCollection,ButtonHeight = 30, Expanded = true)]
        [InfoBox(GASTextDefine.TIP_Warning_EmptyAttribute,InfoMessageType.Error, VisibleIf = "ExistEmptyAttribute")]
        void GenCode()
        {
            if (ExistEmptyAttribute() || ExistDuplicatedAttribute())
            {
                EditorUtility.DisplayDialog("Warning", "Please check the warning message!\n" +
                                                       "Fix the Attribute Error!\n", "OK");
                return;
            }
            Save();
            AttributeCollectionGen.Gen();
            AssetDatabase.Refresh();
        }
        
        private void Save()
        {
            Debug.Log("[EX] Attribute Asset save!");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private int OnRemoveElement(AttributeAccessor attribute)
        {
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE Attribute:{attribute.Name}?",
                "Yes", "No");

            if (!result) return -1;

            Debug.Log($"[EX] Attribute Asset remove element:{attribute.Name} !");
            Save();
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
            Save();
            return index;
        }

        private int OnAddAttribute()
        {
            attributes.Add(new AttributeAccessor(""));
            Save();
            Debug.Log("[EX] Attribute Asset add element!");
            return attributes.Count;
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
                Warning_DuplicatedAttribute = string.Format(GASTextDefine.TIP_Warning_DuplicatedAttribute, duplicatedAttributes);
            }

            return duplicates.Count > 0;
        }

        [Serializable]
        public class AttributeAccessor
        {
            public static AttributeAsset ParentAsset;
            
            [HorizontalGroup("A")]
            [HorizontalGroup("A/R", order:1)]
            [DisplayAsString] [HideLabel]
            public string Name;

            public AttributeAccessor(string attributeName)
            {
                Name = attributeName;
            }

            [HorizontalGroup("A", Width = 50)]
            [HorizontalGroup("A/L", order:0,Width = 50)]
            [Button(SdfIconType.Brush,"",ButtonHeight = 25)]
            public void Edit()
            {
                StringEditWindow.OpenWindow(Name, OnEditSuccess, "Attribute");
            }

            private void OnEditSuccess(string newName)
            {
                Name = newName;
                ParentAsset?.Save();
            }
        }
    }
}
#endif