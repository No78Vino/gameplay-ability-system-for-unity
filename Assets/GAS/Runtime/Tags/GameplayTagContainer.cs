using System.Collections.Generic;

namespace GAS.Runtime
{
    /// <summary>
    /// If the collection of tags is unstable and changable, use this class.
    /// </summary>
    public class GameplayTagContainer
    {
        public List<GameplayTag> Tags { get; }

        public GameplayTagContainer(IEnumerable<GameplayTag> tags)
        {
            Tags = new List<GameplayTag>(tags);
        }

        public void AddTag(in GameplayTag tag)
        {
            if (Tags.Contains(tag)) return;
            Tags.Add(tag);
        }

        public void RemoveTag(in GameplayTag tag)
        {
            Tags.Remove(tag);
        }

        public void AddTag(in GameplayTagSet tagSet)
        {
            foreach (var tag in tagSet.Tags)
                AddTag(tag);
        }

        public void RemoveTag(in GameplayTagSet tagSet)
        {
            foreach (var tag in tagSet.Tags)
                RemoveTag(tag);
        }

        public bool HasTag(in GameplayTag tag)
        {
            foreach (var t in Tags)
            {
                if (t.HasTag(tag))
                    return true;
            }

            return false;
        }

        public bool HasAllTags(in GameplayTagSet other) => HasAllTags(other.Tags);

        public bool HasAllTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
            {
                if (!HasTag(tag))
                    return false;
            }

            return true;
        }

        public bool HasAnyTags(in GameplayTagSet other) => HasAnyTags(other.Tags);

        public bool HasAnyTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag))
                    return true;
            }

            return false;
        }

        public bool HasNoneTags(in GameplayTagSet other) => HasNoneTags(other.Tags);

        public bool HasNoneTags(IEnumerable<GameplayTag> tags) => HasAnyTags(tags) == false;
    }
}