using System.Collections.Generic;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Collections;

namespace GAS.RuntimeWithECS.Tag
{
    public static class GameplayTagHub
    {
        private static Dictionary<int, GASTag> _tagMap;

        /// <summary>
        ///     初始化TagMap
        /// </summary>
        /// <param name="tagMap"></param>
        public static void InitTagMap(Dictionary<int, GASTag> tagMap)
        {
            _tagMap = tagMap;

            // ECS专用单例TagMap
            var map = new NativeHashMap<int, ComGameplayTag>(tagMap.Keys.Count, Allocator.Persistent);
            foreach (var p in tagMap)
                map.TryAdd(p.Key, new ComGameplayTag
                {
                    Code = p.Value.Code,
                    Children = new NativeArray<int>(p.Value.Children, Allocator.Persistent),
                    Parents = new NativeArray<int>(p.Value.Parents, Allocator.Persistent)
                });

            GASManager.EntityManager.CreateSingleton(new SingletonGameplayTagMap { Map = map });
        }

        /// <summary>
        ///     TagA是否含有TagB
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