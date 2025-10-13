///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class XAttrSet
    {
        public const int FightUnit = 1;
        public const int Bullet = 2;


        public class AS_FightUnit
        {
            public const int Hp = 1;
            public const int Mp = 2;
            public const int Spd = 3;
            public const int Atk = 4;
            public const int Def = 5;
            public const int Posture = 6;
        }

        public class AS_Bullet
        {
            public const int Spd = 3;
            public const int Atk = 4;
        }

        private static Dictionary<int, NewAttributeSetConfig> _attributeSetMap = new Dictionary<int, NewAttributeSetConfig>();

        public static Dictionary<int, NewAttributeSetConfig> AttributeSetMap
        {
            get
            {
                if (_attributeSetMap.Count == 0)
                {
                    var datas = XLuban.Tables.TbattributeSet.DataList;
                    foreach (var attrSet in datas)
                    {
                        var settings = new AttributeBaseSetting[attrSet.Attribute.Length];
                        for (var i = 0; i < attrSet.Attribute.Length; i++)
                        {
                            var a = attrSet.Attribute[i];
                            settings[i] = new AttributeBaseSetting(a.Id, a.InitValue, a.UseMinValue,a.UseMaxValue, a.MinValue, a.MaxValue);
                        }
                        _attributeSetMap.Add(attrSet.Id,new NewAttributeSetConfig(attrSet.Id,settings));
                    }
                }
                return _attributeSetMap;
            }
        }
    }
}
