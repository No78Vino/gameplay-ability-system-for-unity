using System.Collections.Generic;
using System.Linq;

namespace GAS.Runtime
{
    /// <summary>
    ///     If the collection of tags is unstable and changable, use this class.
    /// </summary>
    public class GameplayTagContainer
    {
        public List<GameplayTag> Tags { get; }
        
        public GameplayTagContainer(params GameplayTag[] tags)
        {
            Tags = new List<GameplayTag>(tags);
        }

        public void AddTag(GameplayTag tag)
        {
            if (Tags.Contains(tag)) return;
            Tags.Add(tag);
        }

        public void RemoveTag(GameplayTag tag)
        {
            Tags.Remove(tag);
        }

        public void AddTag(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) AddTag(tag);
        }

        public void RemoveTag(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) RemoveTag(tag);
        }

        public bool HasTag(GameplayTag tag)
        {
            return Tags.Any(t => t.HasTag(tag));
        }

        public bool HasAllTags(GameplayTagSet other)
        {
            return other.Empty || other.Tags.All(HasTag);
        }

        public bool HasAllTags(params GameplayTag[] tags)
        {
            return tags.All(HasTag);
        }

        public bool HasAnyTags(GameplayTagSet other)
        {
            return other.Empty || other.Tags.Any(HasTag);
        }

        public bool HasAnyTags(params GameplayTag[] tags)
        {
            return tags.Any(HasTag);
        }

        public bool HasNoneTags(GameplayTagSet other)
        {
            return other.Empty || !other.Tags.Any(HasTag);
        }

        public bool HasNoneTags(params GameplayTag[] tags)
        {
            return !tags.Any(HasTag);
        }
    }
}