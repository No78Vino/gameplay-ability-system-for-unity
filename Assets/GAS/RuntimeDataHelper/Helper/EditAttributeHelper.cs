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

        #endregion
    }
}