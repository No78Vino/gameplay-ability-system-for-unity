using Newtonsoft.Json;

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
    }

    public class TagInEditor
    {
        public int id;
        public string name;
        public string desc;
    }
}