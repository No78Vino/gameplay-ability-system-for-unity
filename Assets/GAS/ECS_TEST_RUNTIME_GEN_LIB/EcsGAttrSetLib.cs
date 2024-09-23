using System.Collections.Generic;
using GAS.RuntimeWithECS.Attribute;
using GAS.RuntimeWithECS.Attribute.Component;
using Unity.Entities;

namespace GAS.ECS_TEST_RUNTIME_GEN_LIB
{
    public static class EcsGAttrLib
    {
        public const int HP = 1;
        public const int ATK = 2;
        public const int DEF = 3;

        /// <summary>
        /// true = 整数；false = 浮点数
        /// </summary>
        public static readonly Dictionary<int, bool> AttributeTypeMap = new()
        {
            {HP,true},
            {ATK,false},
            {DEF,false},
        };
    }

    public static class EcsGAttrSetCode
    {
        public const int Fight = 1;
        public const int Fight_Monster = 2;
    }

    public static class EcsGAttrSetLib
    {
        public static NewAttributeSetConfig AS_FIGHT = 
            new(EcsGAttrSetCode.Fight, new AttributeBaseSetting[]
            {
                new(EcsGAttrLib.HP, 100, 0,100),
                new(EcsGAttrLib.ATK, 100, 0,100),
                new(EcsGAttrLib.DEF, 100, 0,100),
            });

        public static NewAttributeSetConfig AS_FIGHT_MONSTER = 
            new(EcsGAttrSetCode.Fight_Monster, new AttributeBaseSetting[]
            {
                new(EcsGAttrLib.HP, 100, 0,1000),
            });
    }

    #region 属性集的Runtime Components

    public struct ASCom_FIGHT : IComponentData
    {
        public int CodeValue;
        public AttributeData HP;
        public AttributeData ATK;
        public AttributeData DEF;
    }
    
    public struct ASCom_FIGHT_MONSTER : IComponentData
    {
        public int CodeValue;
        public AttributeData HP;
    }

    #endregion
}