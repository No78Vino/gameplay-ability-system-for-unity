using System.Collections.Generic;
using System.Linq;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Tags
{
    public class GameplayTagCollection
    {
        AbilitySystemComponent  _owner;
        private GameplayTagAggregator _gameplayTagAggregator;

        private List<GameplayTag> FixedTags { get; }
        private Dictionary<GameplayTag, List<object>> DynamicAddedTags = new Dictionary<GameplayTag, List<object>>();
        private Dictionary<GameplayTag, List<object>> DynamicRemovedTags = new Dictionary<GameplayTag, List<object>>();
        
        public GameplayTagCollection(AbilitySystemComponent owner,params GameplayTag[] tags)
        {
            _owner = owner;
            FixedTags = new List<GameplayTag>(tags);
            _gameplayTagAggregator = new GameplayTagAggregator(_owner);
        }
        
        public void Init(GameplayTag[] tags)
        {
            FixedTags.AddRange(tags);
        }
        
        public void OnEnable()
        {
            _gameplayTagAggregator.RegisterOnTagIsDirty(_owner.GameplayEffectContainer.RefreshGameplayEffectState);
        }
        
        public void OnDisable()
        {
            _gameplayTagAggregator.UnregisterOnTagIsDirty(_owner.GameplayEffectContainer.RefreshGameplayEffectState);
        }

        private void AddFixedTag(GameplayTag tag)
        {
            if (HasTag(tag)) return;
            FixedTags.Add(tag);
        }

        private void RemoveFixedTag(GameplayTag tag)
        {
            FixedTags.Remove(tag);
        }

        public void AddFixedTags(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) AddFixedTag(tag);
            
            _gameplayTagAggregator.TagIsDirty(tagSet);
        }

        public void RemoveFixedTags(GameplayTagSet tagSet)
        {
            if(tagSet.Empty) return;
            foreach (var tag in tagSet.Tags) RemoveFixedTag(tag);
            
            _gameplayTagAggregator.TagIsDirty(tagSet);
        }

        public void ApplyGameplayEffectDynamicTag(GameplayEffectSpec source)
        {
            bool tagIsDirty = false;
            var grantedTagSet = source.GameplayEffect.TagContainer.GrantedTags;
            foreach (var tag in grantedTagSet.Tags)
            {
                // TODO
                // if (HasTag(tag)) return;
                // if (!DynamicAddedTags.ContainsKey(tag))
                // {
                //     DynamicAddedTags.Add(tag,new List<object>());
                //     tagIsDirty = true;
                // }
                // DynamicAddedTags[tag].Add(source);
            }

            if (tagIsDirty)
            {
                _gameplayTagAggregator.TagIsDirty(grantedTagSet);
            }
        }
        
        public void ApplyGameplayAbilityDynamicTag(AbilitySpec source, GameplayTag tag)
        {
            // TODO
        }
        public void RestoreDynamicAddedTags()
        {
            // TODO
        }
        
        public void RestoreDynamicRemovedTags()
        {
            // TODO
        }
        
        public bool HasTag(GameplayTag tag)
        {
            return FixedTags.Any(t => t.HasTag(tag));
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