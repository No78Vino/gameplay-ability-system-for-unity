using System;
using System.Linq;

namespace GAS.Runtime.Tags
{
    /// <summary>
    /// If the collection of tags is stable and unchangable, use this class to improve performance.
    /// </summary>
    public struct GameplayTagSet
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
            Tags = tags;
        }
        
        public bool HasTag(GameplayTag tag)
        {
            return Tags.Any(t => t.HasTag(tag));
        }
        
        public bool HasAllTags(GameplayTagSet other)
        {
            return other.Tags.All(HasTag);
        }
        
        public bool HasAllTags(params GameplayTag[] tags)
        {
            return tags.All(HasTag);
        }
        
        public bool HasAnyTags(GameplayTagSet other)
        {
            return other.Tags.Any(HasTag);
        }

        public bool HasAnyTags(params GameplayTag[] tags)
        {
            return tags.Any(HasTag);
        }
        
        public bool HasNoneTags(GameplayTagSet other)
        {
            return !other.Tags.Any(HasTag);
        }
        
        public bool HasNoneTags(params GameplayTag[] tags)
        {
            return !tags.Any(HasTag);
        }
    }
}