using System.Collections.Generic;
using System.Linq;
using GAS.Core;
using GAS.Runtime.AttributeSet;
using UnityEditor;

namespace GAS.Editor.AttributeSet
{
    public static class AttributeSetEditorUtil
    {
        public static List<string> GetAttributeSetChoice()
        {
            var choices = new List<string>();
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GasDefine.GAS_ATTRIBUTESET_ASSET_PATH);
            foreach (var attributeSetConfig in asset.AttributeSetConfigs)
            {
                var config = attributeSetConfig;
                choices.Add(config.Name);
            }

            return choices;
        }
    }
}