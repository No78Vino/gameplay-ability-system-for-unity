using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Editor
{
    public static class GasJsonReader
    {
        private static TagInEditor[] _tags;
        private static AttrInEditor[] _attrs;
        private static AttrSetInEditor[] _attrSets;

        public static Dictionary<int, TagInEditor> TagMap() => _tags.ToDictionary(t => t.id, t => t);
        public static Dictionary<int, AttrInEditor> AttrMap() => _attrs.ToDictionary(t => t.id, t => t);
        public static Dictionary<int, AttrSetInEditor> AttrSetMap() => _attrSets.ToDictionary(t => t.id, t => t);
        
        static GasJsonReader()
        {
            ReadAllAndCache();
        }
        
        private static ValueDropdownItem[] _tagChoices;
        public static ValueDropdownItem[] TagChoices()
        {
            return _tagChoices ??= _tags.Select(t => new ValueDropdownItem(t.name, t.id)).ToArray();
        }
        
        public static void ReadAllAndCache()
        {
            var settingAsset = GASSettingAsset.Instance;
            var tagFilePath = settingAsset.PathOfJsonTag;
            if (File.Exists(tagFilePath))
                ReadTags(File.ReadAllText(tagFilePath));
            else
                Debug.LogError($"JSON file not found at {tagFilePath}");

            var attrFilePath = settingAsset.PathOfJsonAttr;
            if (File.Exists(attrFilePath))
                ReadAttributes(File.ReadAllText(attrFilePath));
            else
                Debug.LogError($"JSON file not found at {attrFilePath}");
            
            var attrSetFilePath = settingAsset.PathOfJsonAttrSet;
            if (File.Exists(attrSetFilePath))
                ReadAttributeSets(File.ReadAllText(attrSetFilePath));
            else
                Debug.LogError($"JSON file not found at {attrSetFilePath}");
        }

        public static TagInEditor[] ReadTags(string jsonContent)
        {
            _tags = JsonConvert.DeserializeObject<TagInEditor[]>(jsonContent);
            _tagChoices = _tags.Select(t => new ValueDropdownItem(t.name, t.id)).ToArray();
            return _tags;
        }
        
        public static AttrInEditor[] ReadAttributes(string jsonContent)
        {
            _attrs = JsonConvert.DeserializeObject<AttrInEditor[]>(jsonContent);
            return _attrs;
        }
        
        public static AttrSetInEditor[] ReadAttributeSets(string jsonContent)
        {
            _attrSets = JsonConvert.DeserializeObject<AttrSetInEditor[]>(jsonContent);
            return _attrSets;
        }
    }

    public class TagInEditor
    {
        public int id;
        public string name;
        public string desc;
    }
    
    public class AttrInEditor
    {
        public int id;
        public string name;
        public string desc;
    }
    
    public class AttrSetInEditor
    {
        [HorizontalGroup("A",Width = 200)]
        [TitleGroup("A/基本信息",BoldTitle = false,Order = 1)]
        [LabelText("属性集ID")]
        [DisplayAsString]
        public int id;
        
        [TitleGroup("A/基本信息")]
        [LabelText("属性集名")]
        [DisplayAsString]
        public string name;
        
        [TitleGroup("A/基本信息")]
        [LabelText("属性集描述")]
        [DisplayAsString]
        public string desc;
        
        [TitleGroup("A/包含属性",BoldTitle = false,Order = 2)]
        [LabelText(" ")]
        [TableList(AlwaysExpanded = true,IsReadOnly = true)]
        public AttrInSetInEditor[] attribute;
    }

    public class AttrInSetInEditor
    {
        [VerticalGroup("属性ID")]
        [HorizontalGroup("属性ID/A")]
        [HideLabel][DisplayAsString]
        public int id;
        
        [VerticalGroup("属性初始值")]
        [HideLabel][DisplayAsString]
        public float initValue;
        
        [VerticalGroup("属性最小值")]
        [HorizontalGroup("属性最小值/H",Width=50)]
        [LabelText("启用"),LabelWidth(30)]
        public bool useMinValue;
        
        [VerticalGroup("属性最大值")]
        [HorizontalGroup("属性最大值/H",Width=50)]
        [LabelText("启用"),LabelWidth(30)]
        public bool useMaxValue;
        
        [HorizontalGroup("属性最小值/H")]
        [DisplayAsString]
        [HideLabel]
        [ShowIf(nameof(useMinValue))]
        public float minValue;
        
        [HorizontalGroup("属性最大值/H")]
        [DisplayAsString]
        [HideLabel]
        [ShowIf(nameof(useMaxValue))]
        public float maxValue;

        [HorizontalGroup("属性ID/A")]
        [ShowInInspector]
        [HideLabel]
        [DisplayAsString(EnableRichText = true)]
        public string AttrName
        {
            get
            {
                var map = GasJsonReader.AttrMap();
                return map.TryGetValue(id,out var attr) ? $"<color=white>{attr.name}</color>" : $"<color=red>ERROR</color>";
            }
        }

        public string GetAttrName()
        {
            var map = GasJsonReader.AttrMap();
            return map.TryGetValue(id,out var attr) ? attr.name : "ERROR_ATTR";
        }
    }
}