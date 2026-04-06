using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Editor
{
    public static class GasJsonReader
    {
        private static TagInEditor[] _tags = System.Array.Empty<TagInEditor>();
        private static AttrInEditor[] _attrs = System.Array.Empty<AttrInEditor>();
        private static AttrSetInEditor[] _attrSets = System.Array.Empty<AttrSetInEditor>();
        private static readonly List<string> _loadErrors = new();

        public static bool HasLoadErrors => _loadErrors.Count > 0;
        public static string LoadErrorSummary => string.Join("\n", _loadErrors);

        public static Dictionary<int, TagInEditor> TagMap() => BuildUniqueMap(_tags, "Tag");
        public static Dictionary<int, AttrInEditor> AttrMap() => BuildUniqueMap(_attrs, "Attribute");
        public static Dictionary<int, AttrSetInEditor> AttrSetMap() => BuildUniqueMap(_attrSets, "AttributeSet");
        
        static GasJsonReader()
        {
            ReadAllAndCache();
        }
        
        private static ValueDropdownItem[] _tagChoices;
        public static ValueDropdownItem[] TagChoices()
        {
            return _tagChoices ??= _tags
                .Where(t => t != null)
                .Select(t => new ValueDropdownItem(string.IsNullOrWhiteSpace(t.name) ? $"Tag_{t.id}" : t.name, t.id))
                .ToArray();
        }
        
        public static void ReadAllAndCache()
        {
            _loadErrors.Clear();
            var settingAsset = GASSettingAsset.Instance;
            var tagFilePath = settingAsset.PathOfJsonTag;
            _tags = TryReadJsonFile(tagFilePath, "Tag", ReadTags);

            var attrFilePath = settingAsset.PathOfJsonAttr;
            _attrs = TryReadJsonFile(attrFilePath, "Attribute", ReadAttributes);
            
            var attrSetFilePath = settingAsset.PathOfJsonAttrSet;
            _attrSets = TryReadJsonFile(attrSetFilePath, "AttributeSet", ReadAttributeSets);
        }

        public static TagInEditor[] ReadTags(string jsonContent)
        {
            _tags = SafeDeserializeArray<TagInEditor>(jsonContent, "Tag");
            _tagChoices = _tags
                .Where(t => t != null)
                .Select(t => new ValueDropdownItem(string.IsNullOrWhiteSpace(t.name) ? $"Tag_{t.id}" : t.name, t.id))
                .ToArray();
            return _tags;
        }
        
        public static AttrInEditor[] ReadAttributes(string jsonContent)
        {
            _attrs = SafeDeserializeArray<AttrInEditor>(jsonContent, "Attribute");
            return _attrs;
        }
        
        public static AttrSetInEditor[] ReadAttributeSets(string jsonContent)
        {
            _attrSets = SafeDeserializeArray<AttrSetInEditor>(jsonContent, "AttributeSet");
            foreach (var attrSet in _attrSets.Where(x => x != null))
            {
                attrSet.attribute ??= System.Array.Empty<AttrInSetInEditor>();
            }
            return _attrSets;
        }

        private static T[] TryReadJsonFile<T>(string path, string label, System.Func<string, T[]> parser)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                AddLoadError($"{label}: JSON 路径为空。请在 GAS Setting 配置路径。");
                return System.Array.Empty<T>();
            }

            if (!File.Exists(path))
            {
                AddLoadError($"{label}: JSON 文件不存在 -> {path}");
                return System.Array.Empty<T>();
            }

            try
            {
                return parser(File.ReadAllText(path)) ?? System.Array.Empty<T>();
            }
            catch (System.Exception ex)
            {
                AddLoadError($"{label}: 读取失败 -> {path}\n{ex.GetType().Name}: {ex.Message}");
                return System.Array.Empty<T>();
            }
        }

        private static T[] SafeDeserializeArray<T>(string jsonContent, string label)
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                AddLoadError($"{label}: JSON 内容为空。");
                return System.Array.Empty<T>();
            }

            try
            {
                var token = JToken.Parse(jsonContent);
                if (token.Type == JTokenType.Array)
                {
                    return token.ToObject<T[]>() ?? System.Array.Empty<T>();
                }

                // 兼容部分导表格式：{ "items": [...] } / { "data": [...] } / { "list": [...] }
                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;
                    var arrayToken = obj["items"] ?? obj["data"] ?? obj["list"];
                    if (arrayToken is JArray jArray)
                    {
                        return jArray.ToObject<T[]>() ?? System.Array.Empty<T>();
                    }
                }

                AddLoadError($"{label}: JSON 根节点不是数组，且未找到 items/data/list。");
                return System.Array.Empty<T>();
            }
            catch (System.Exception ex)
            {
                AddLoadError($"{label}: JSON 反序列化失败 -> {ex.GetType().Name}: {ex.Message}");
                return System.Array.Empty<T>();
            }
        }

        private static Dictionary<int, T> BuildUniqueMap<T>(IEnumerable<T> source, string label) where T : class
        {
            var result = new Dictionary<int, T>();
            if (source == null)
            {
                return result;
            }

            var idField = typeof(T).GetField("id");
            if (idField == null || idField.FieldType != typeof(int))
            {
                AddLoadError($"{label}: 类型 {typeof(T).Name} 缺少 int id 字段。");
                return result;
            }

            foreach (var item in source.Where(x => x != null))
            {
                var id = (int)idField.GetValue(item);
                if (!result.TryAdd(id, item))
                {
                    Debug.LogWarning($"[EX-GAS] {label} 存在重复 id={id}，已保留第一项并忽略后续项。");
                }
            }

            return result;
        }

        private static void AddLoadError(string message)
        {
            _loadErrors.Add(message);
            Debug.LogWarning($"[EX-GAS] {message}");
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
