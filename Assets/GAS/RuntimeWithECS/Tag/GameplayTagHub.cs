using System.Collections.Generic;

namespace GAS.RuntimeWithECS.Tag
{
    public static class GameplayTagHub
    {
        private static Dictionary<int, GASTag> _tagMap;

        public static void SetTagMap(Dictionary<int, GASTag> tagMap)
        {
            _tagMap = tagMap;
        }
        
        public static void AddTagToMap(GASTag tag)
        {
            _tagMap ??= new Dictionary<int, GASTag>();
            _tagMap.TryAdd(tag.ENUM, tag);
        }

        public static void RemoveTagFromMap(GASTag tag)
        {
            _tagMap?.Remove(tag.ENUM);
        }
        
        /// <summary>
        /// TagA是否含有TagB
        /// </summary>
        /// <param name="tagA"></param>
        /// <param name="tagB"></param>
        /// <returns></returns>
        public static bool HasTag(int tagA, int tagB)
        {
            if (_tagMap.ContainsKey(tagA) && _tagMap.ContainsKey(tagB)) 
                return _tagMap[tagA].HasTag(_tagMap[tagB]);
            return false;
        }
    }
}