using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag.Component
{
    public struct SingletonGameplayTagMap : IComponentData
    {
        public NativeHashMap<int, ComGameplayTag> Map;
    }

    public struct ComGameplayTag : IComponentData
    {
        public int Code;
        public NativeArray<int> Parents;
        public NativeArray<int> Children;
    }
    
    public static class SingletonGameplayTagMapExtension
    {
        public static bool IsTagAIncludeTagB(this SingletonGameplayTagMap map,int tagA, int tagB)
        {
            if (map.Map.ContainsKey(tagA) && map.Map.ContainsKey(tagB))
                return map.Map[tagA].HasTag(map.Map[tagB]);
            return false;
        }

        private static bool HasTag(this ComGameplayTag gTag, ComGameplayTag tag)
        {
            if (gTag.Code == tag.Code) return true;
            foreach (var pTag in gTag.Parents)
                if (pTag == tag.Code)
                    return true;

            return false;
        }
    }
}