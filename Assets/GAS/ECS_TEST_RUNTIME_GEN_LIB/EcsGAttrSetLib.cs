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
        public static AttributeSetConfig AS_FIGHT =
            new(EcsGAttrSetCode.Fight, new[]
            {
                EcsGAttrLib.HP, EcsGAttrLib.ATK, EcsGAttrLib.DEF
            });

        public static AttributeSetConfig AS_FIGHT_MONSTER =
            new(EcsGAttrSetCode.Fight_Monster, new[]
            {
                EcsGAttrLib.HP
            });
    }

    #region 属性集的Runtime Components

    public struct ASCom_FIGHT : IComponentData
    {
        public int CodeValue;
        public AttributeComponent HP;
        public AttributeComponent ATK;
        public AttributeComponent DEF;
    }
    
    public struct ASCom_FIGHT_MONSTER : IComponentData
    {
        public int CodeValue;
        public AttributeComponent HP;
    }

    #endregion
}