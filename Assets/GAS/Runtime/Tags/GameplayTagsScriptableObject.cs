using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.Runtime.Tags
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Tags Settings", fileName = "GameplayTagsSettings")]
    public class GameplayTagsScriptableObject : ScriptableObject
    {
        [BoxGroup("Tags", order: 1)] [TableList(IsReadOnly = true)]
        public List<GameplayTag> Tags = new();
        
        /// <summary>
        /// Cache the root tags for quick access.
        /// </summary>
        [HideInInspector]
        public List<GameplayTag> RootTags = new();

        /// <summary>
        /// Cache the tag data for quick access.
        /// </summary>
        public Dictionary<int,GameplayTag> TagData = new();
        
        public void OnValidate()
        {
            UpdateCache();
        }

        private void UpdateCache()
        {
            // First, Complete the remaining gaps in the ancestor tags.
            var currentTagCount = Tags.Count;
            for (var i = 0; i < currentTagCount; i++)
            {
                var tag = Tags[i];
                for (var j = 0; j < tag.AncestorHashCodes.Length; j++)
                {
                    var ancestor = tag.AncestorHashCodes[j];
                    if (!ContainTag(ancestor)) Tags.Add(new GameplayTag(tag.AncestorNames[j]));
                }
            }

            // Second, Add the direct descendant and direct ancestor to the ancestor tags.
            for (var i = 0; i < Tags.Count; i++)
            {
                Tags[i].ClearDescendants();
                foreach (var t in Tags)
                    if (!t.Root && t.AncestorHashCodes.Last() == Tags[i].HashCode)
                        Tags[i].AddDescendant(t);
            }

            // Finally, Build the tag cache.
            TagData.Clear();
            foreach (var tag in Tags)
                TagData.Add(tag.HashCode,tag);
            
            RootTags.Clear();
            foreach (var tag in Tags)
                if (tag.Root)
                    RootTags.Add(tag);
            
            EditorUtility.SetDirty(this);
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }

        private bool ContainTag(int tagHashCode)
        {
            foreach (var t in Tags)
                if (t.GetHashCode() == tagHashCode)
                    return true;

            return false;
        }

        public void CreateNewTag(string tagName)
        {
            Tags.Add(new GameplayTag(tagName));
            UpdateCache();
        }
    }
}