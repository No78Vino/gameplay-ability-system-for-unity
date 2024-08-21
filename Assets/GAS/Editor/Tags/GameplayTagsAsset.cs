using System.Collections.Generic;
using GAS.Runtime;
using UnityEditor.TreeDataModel;
using UnityEngine;

namespace GAS.Editor
{
    [FilePath(GasDefine.GAS_TAGS_MANAGER_ASSET_PATH)]
    public class GameplayTagsAsset : ScriptableSingleton<GameplayTagsAsset>
    {
        [SerializeField]
        List<GameplayTagTreeElement> gameplayTagTreeElements = new List<GameplayTagTreeElement>();

        internal List<GameplayTagTreeElement> GameplayTagTreeElements => gameplayTagTreeElements;

        [SerializeField]
        public List<GameplayTag> Tags = new();


        public void CacheTags()
        {
            Tags.Clear();
            foreach (var tagTreeElement in gameplayTagTreeElements)
            {
                ExTreeElement tag = tagTreeElement;
                if (tag.Depth == -1) continue;
                string tagName = tag.Name;
                while (tag.Parent.Depth >= 0)
                {
                    tagName = tag.Parent.Name + "." + tagName;
                    tag = tag.Parent;
                }

                Tags.Add(new(tagName));
            }
        }
    }
}