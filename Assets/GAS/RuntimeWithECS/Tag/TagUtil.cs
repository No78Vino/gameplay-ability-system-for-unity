using System.Collections.Generic;

namespace GAS.RuntimeWithECS.Tag
{
    public static class TagUtil
    {
        private static Dictionary<int, GASTag> _tagMap;

        public static void SetTagMap(Dictionary<int, GASTag> tagMap)
        {
            _tagMap = tagMap;
        }
        
        public static void AddTagIntoMap(GASTag tag)
        {
            _tagMap ??= new Dictionary<int, GASTag>();
            _tagMap.TryAdd(tag.ENUM, tag);
        }

        public static void RemoveTagFromMap(GASTag tag)
        {
            _tagMap?.Remove(tag.ENUM);
        }
        
        public static bool HasTag(int tagA, int tagB)
        {
            if (_tagMap.ContainsKey(tagA) && _tagMap.ContainsKey(tagB)) 
                return _tagMap[tagA].HasTag(_tagMap[tagB]);
            return false;
        }
    }
}