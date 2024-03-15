#if UNITY_EDITOR
namespace GAS.Editor.Tags
{
    using System.Collections.Generic;
    using GAS.Core;
    using GAS.Runtime.Tags;
    using UnityEditor;
    using Editor;

    
    public static class TagEditorUntil
    {
        public static List<GameplayTag> GetTagChoice()
        {
            var tagAsset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GASSettingAsset.GAS_TAG_ASSET_PATH);
            return tagAsset.Tags;
        }
    }
}
#endif