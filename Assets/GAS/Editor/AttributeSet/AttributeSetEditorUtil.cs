
#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections.Generic;
    using GAS;
    using Runtime;
    using UnityEditor;
    using Editor;

    public static class AttributeSetEditorUtil
    {
        public static List<string> GetAttributeSetChoice()
        {
            var choices = new List<string>();
            var asset = AttributeSetAsset.LoadOrCreate();
            foreach (var attributeSetConfig in asset.AttributeSetConfigs)
            {
                var config = attributeSetConfig;
                choices.Add(config.Name);
            }

            return choices;
        }
    }
}
#endif