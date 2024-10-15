using System.Collections.Generic;
using GAS.RuntimeWithECS.Tag;

namespace GAS.ECS_TEST_RUNTIME_GEN_LIB
{
    public class GTagList
    {
        public const int Magic = 0;
        public const int Magic_Fire = 1;
        public const int Magic_Water = 2;
        public const int Magic_Earth = 3;
        
        public const int State = 4;
        public const int State_Normal = 5;
        public const int State_Hurt = 6;
        
        public static void InitTagList()
        {
            GameplayTagHub.InitTagMap(new Dictionary<int, GASTag>()
            {
                {
                    Magic, new GASTag(Magic, null, new[]
                    {
                        Magic_Fire,
                        Magic_Water,
                        Magic_Earth
                    })
                },
                { Magic_Fire, new GASTag(Magic_Fire, new[] { Magic }, null) },
                { Magic_Water, new GASTag(Magic_Water, new[] { Magic }, null) },
                { Magic_Earth, new GASTag(Magic_Earth, new[] { Magic }, null) },

                {
                    State, new GASTag(State, null, new[]
                    {
                        State_Normal,
                        State_Hurt
                    })
                },
                { State_Normal, new GASTag(State_Normal, new[] { State }, null) },
                { State_Hurt, new GASTag(State_Hurt, new[] { State }, null) },
            });
        }
    }
}