using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Tags;
using UnityEditor;

namespace GAS.Editor.Tags
{
    public static class TagEditorUntil
    {
        public static List<GameplayTag> GetTagChoice()
        {
            var tagAsset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GasDefine.GAS_TAG_ASSET_PATH);
            return tagAsset.Tags;
        }
    }
}