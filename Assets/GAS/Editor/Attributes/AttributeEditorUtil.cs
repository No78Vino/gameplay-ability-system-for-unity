#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using GAS.Core;
    using Editor;
    using UnityEditor;


    public static class AttributeEditorUtil
    {
        public static List<string> GetAttributeNameChoices()
        {
            var names = new List<string>();
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
            foreach (var attributeSetConfig in asset.AttributeSetConfigs)
            {
                var config = attributeSetConfig;
                names.AddRange(attributeSetConfig.AttributeNames.Select(shortName => $"AS_{config.Name}.{shortName}"));
            }
            return names;
        }
    }
}
#endif