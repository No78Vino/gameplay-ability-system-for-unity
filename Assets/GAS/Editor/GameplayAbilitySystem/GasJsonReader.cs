using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public static class GasJsonReader
    {
        /// <summary>
        /// 读取标签数据
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static TagInEditor[] ReadTags(string jsonContent)
        {
            // 使用newtonsoft.json解析JSON内容
            // tag的json内容格式：[
            // {
            //     "id": 100,
            //     "name": "Faction",
            //     "desc": "阵营描述标签"
            // },
            // {
            //     "id": 1001,
            //     "name": "Faction.Player",
            //     "desc": "玩家阵营"
            // },.....]
            return JsonConvert.DeserializeObject<TagInEditor[]>(jsonContent);
        }
        
        public static AttrInEditor[] ReadAttributes(string jsonContent)
        {
            return JsonConvert.DeserializeObject<AttrInEditor[]>(jsonContent);
        }
        
        public static AttrSetInEditor[] ReadAttributeSets(string jsonContent)
        {
            return JsonConvert.DeserializeObject<AttrSetInEditor[]>(jsonContent);
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
        [HorizontalGroup("A")]
        [TitleGroup("A/基本信息",order:1)]
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
        
        [TitleGroup("A/包含属性",order:2)]
        [LabelText(" ")]
        [TableList(AlwaysExpanded = true,IsReadOnly = true)]
        public AttrInSetInEditor[] attribute;
    }

    public class AttrInSetInEditor
    {
        [VerticalGroup("属性ID")]
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
    }
}