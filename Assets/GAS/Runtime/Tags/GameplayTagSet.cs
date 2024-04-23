using System;

namespace GAS.Runtime
{
    /// <summary>
    /// If the collection of tags is stable and unchangable, use this class to improve performance.
    /// </summary>
    public readonly struct GameplayTagSet
    {
        public readonly GameplayTag[] Tags;
        
        public bool Empty => Tags.Length == 0;
        
        public GameplayTagSet(string[] tagNames)
        {
            Tags = new GameplayTag[tagNames.Length];
            for (var i = 0; i < tagNames.Length; i++)
            {
                Tags[i] = new GameplayTag(tagNames[i]);
            }
        }
        
        public GameplayTagSet(params GameplayTag[] tags)
        {
            Tags = tags?? Array.Empty<GameplayTag>();
        }
        
        public bool HasTag(GameplayTag tag)
        {
            foreach (var t in Tags)
            {
                if (t.HasTag(tag)) return true;
            }

            return false;
        }
        
        public bool HasAllTags(GameplayTagSet other)
        {
            return HasAllTags(other.Tags);
        }
        
        public bool HasAllTags(params GameplayTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (!HasTag(tag)) return false;
            }

            return true;
        }
        
        public bool HasAnyTags(GameplayTagSet other)
        {
            return HasAnyTags(other.Tags);
        }

        public bool HasAnyTags(params GameplayTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return true;
            }

            return false;
        }
        
        public bool HasNoneTags(GameplayTagSet other)
        {
            return HasNoneTags(other.Tags);
        }
        
        public bool HasNoneTags(params GameplayTag[] tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return false;
            }

            return true;
        }
    }
}