///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;
using GAS.RuntimeWithECS.Attribute;

namespace GAS.Runtime
{
    public static class GEN_AttributeCode
    {
        public const int HP = 119047830;
        public const int MP = 119047825;
        public const int STAMINA = -987323975;
        public const int POSTURE = 985802258;
        public const int ATK = -1844636348;
        public const int SPEED = 2025855271;
    }
    public static class GEN_AttrSetCode
    {
        public const int Fight = 109648900;
        public static NewAttributeSetConfig AS_Fight = new(Fight, new AttributeBaseSetting[]
        {
            new(GEN_AttributeCode.HP,100,0,100),
            new(GEN_AttributeCode.MP,100,0,100),
            new(GEN_AttributeCode.STAMINA,100,0,100),
            new(GEN_AttributeCode.POSTURE,100,0,100),
            new(GEN_AttributeCode.ATK,100,0,100),
            new(GEN_AttributeCode.SPEED,100,0,100),
        });
        public static Dictionary<int,NewAttributeSetConfig> AttributeSetMap = new()
        {
            {Fight,AS_Fight},
        };
    }
}