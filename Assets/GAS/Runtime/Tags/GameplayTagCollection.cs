using System.Collections.Generic;
using System.Linq;
using GAS.Runtime.Component;

namespace GAS.Runtime.Tags
{
    public class GameplayTagCollection
    {
        AbilitySystemComponent  _owner;
        private GameplayTagAggregator _gameplayTagAggregator;
        
        public List<GameplayTag> Tags { get; }
        
        public GameplayTagCollection(AbilitySystemComponent owner,params GameplayTag[] tags)
        {
            _owner = owner;
            Tags = new List<GameplayTag>(tags);
            _gameplayTagAggregator = new GameplayTagAggregator(_owner);
        }
        
        public void OnEnable()
        {
            _gameplayTagAggregator.RegisterOnTagIsDirty(_owner.GameplayEffectContainer.RefreshGameplayEffectState);
        }
        
        public void OnDisable()
        {
            _gameplayTagAggregator.UnregisterOnTagIsDirty(_owner.GameplayEffectContainer.RefreshGameplayEffectState);
        }

        public void AddTag(GameplayTag tag)
        {
            if (HasTag(tag)) return;
            Tags.Add(tag);
        }

        private void RemoveTag(GameplayTag tag)
        {
            Tags.Remove(tag);
        }

        public void AddTags(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) AddTag(tag);
            
            _gameplayTagAggregator.TagIsDirty(tagSet);
        }

        public void RemoveTags(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) RemoveTag(tag);
            
            _gameplayTagAggregator.TagIsDirty(tagSet);
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