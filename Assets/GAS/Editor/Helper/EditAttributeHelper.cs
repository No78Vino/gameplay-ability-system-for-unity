using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GAS.Editor;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Helper
{
    public static class EditAttributeHelper
    {
        #region Attribute

        private static ValueDropdownItem[] _attributeSetChoices;

        private static List<AttributeSetConfig> LoadAttributeSet()
        {
            // 属性信息
            // var attributeAsset = AttributeAsset.LoadOrCreate();
            // var attributeInfos = (from t in attributeAsset.attributes
            //     where !string.IsNullOrWhiteSpace(t.Name)
            //     select new Tuple<int, string>(t.GetCode(), t.Name)).ToList();

            // 属性集信息
            var attributeSetAsset = AttributeSetAsset.LoadOrCreate();
            var attributeSetInfos = attributeSetAsset.AttributeSetConfigs;
            return attributeSetInfos;
        }

        public static IEnumerable<ValueDropdownItem> AttributeSetChoices
        {
            get
            {
                _attributeSetChoices ??= LoadAttributeSet()
                    .Select(attrSetAsset => new ValueDropdownItem(attrSetAsset.Name, attrSetAsset.GetCode()))
                    .ToArray();

                return _attributeSetChoices;
            }
        }

        private static Dictionary<string,int> _attributeCodeMap;

        private static Dictionary<string,int> LoadAttributeCodeMap()
        {
            // 属性信息
            var attributeAsset = AttributeAsset.LoadOrCreate();
            _attributeCodeMap = new Dictionary<string, int>();

            foreach (var attr in attributeAsset.attributes)
            {
                _attributeCodeMap.Add(attr.Name,attr.GetCode());
            }
            return _attributeCodeMap;
        }
        
        public static Dictionary<string,int> AttributeCodeMap()
        {
            if (_attributeCodeMap == null)
            {
                LoadAttributeCodeMap();
            }
            return _attributeCodeMap;
        }
        
        public static IEnumerable<ValueDropdownItem> GetAttributeChoiceByAttrSet(int attrSetCode)
        {
            var attrSetCfgs = LoadAttributeSet();
            AttributeSetConfig attrSet = attrSetCfgs.FirstOrDefault(cfg => cfg.GetCode() == attrSetCode);

            var codeMap = AttributeCodeMap();
            if (attrSet == null) return Array.Empty<ValueDropdownItem>();
            var dropdownItems = new ValueDropdownItem[attrSet.AttributeNames.Count];
            for (var i = 0; i < attrSet.AttributeNames.Count; i++)
            {
                var name = attrSet.AttributeNames[i];
                dropdownItems[i] = new ValueDropdownItem(name, codeMap[name]);
            }
            
            return dropdownItems;
        }
        #endregion
    }
}