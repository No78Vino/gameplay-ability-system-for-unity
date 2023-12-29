using System.Collections.Generic;
using System.Linq;
using GAS.Core;
using GAS.Runtime.AttributeSet;
using UnityEditor;

namespace GAS.Editor.Attributes
{
    public static class AttributeEditorUtil
    {
        public static List<string> GetAttributeNameChoices()
        {
            var names = new List<string>();
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GasDefine.GAS_ATTRIBUTESET_ASSET_PATH);
            foreach (var attributeSetConfig in asset.AttributeSetConfigs)
            {
                var config = attributeSetConfig;
                names.AddRange(attributeSetConfig.AttributeNames.Select(shortName => $"{config.Name}.{shortName}"));
            }
            return names;
        }
    }
}