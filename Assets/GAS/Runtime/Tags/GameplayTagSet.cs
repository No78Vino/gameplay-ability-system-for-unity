using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    /// <summary>
    /// If the collection of tags is stable and unchangable, use this struct to improve performance.
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
            Tags = tags ?? Array.Empty<GameplayTag>();
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

        public bool HasAllTags(in GameplayTagSet other) => HasAllTags((IEnumerable<GameplayTag>)other.Tags);
        public bool HasAllTags(params GameplayTag[] tags) => HasAllTags((IEnumerable<GameplayTag>)tags);

        public bool HasAllTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
            {
                if (!HasTag(tag))
                    return false;
            }

            return true;
        }

        public bool HasAnyTags(in GameplayTagSet other) => HasAnyTags((IEnumerable<GameplayTag>)other.Tags);

        public bool HasAnyTags(params GameplayTag[] tags) => HasAnyTags((IEnumerable<GameplayTag>)tags);

        public bool HasAnyTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
            {
                if (HasTag(tag))
                    return true;
            }

            return false;
        }

        public bool HasNoneTags(in GameplayTagSet other) => HasNoneTags((IEnumerable<GameplayTag>)other.Tags);
        public bool HasNoneTags(params GameplayTag[] tags) => HasNoneTags((IEnumerable<GameplayTag>)tags);
        public bool HasNoneTags(IEnumerable<GameplayTag> tags) => HasAnyTags(tags) == false;
    }
}