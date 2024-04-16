#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using GAS.Runtime;
    
    public static class TagEditorUntil
    {
        public static List<GameplayTag> GetTagChoice()
        {
            var tagAsset = GameplayTagsAsset.LoadOrCreate();
            return tagAsset.Tags;
        }
    }
}
#endif