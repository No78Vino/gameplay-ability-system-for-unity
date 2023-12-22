using System.Linq;

namespace GAS.Runtime.Tags
{
    /// <summary>
    /// If the collection of tags is stable and unchangable, use this class to improve performance.
    /// </summary>
    public struct GameplayTagSet
    {
        public GameplayTag[] Tags { get;private set; }
        
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