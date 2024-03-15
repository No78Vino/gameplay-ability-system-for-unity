using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime.Ability;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Editor
{
    public class GameplayTagAggregator
    {
        private AbilitySystemComponent _owner;

        private Dictionary<GameplayTag, List<object>> _dynamicAddedTags =
            new Dictionary<GameplayTag, List<object>>();

        private Dictionary<GameplayTag, List<object>> _dynamicRemovedTags =
            new Dictionary<GameplayTag, List<object>>();

        private readonly List<GameplayTag> _fixedTags = new List<GameplayTag>();

        public GameplayTagAggregator(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        private event Action OnTagIsDirty;

        private void TagIsDirty(GameplayTagSet tags)
        {
            if (!tags.Empty) OnTagIsDirty?.Invoke();
        }

        private void TagIsDirty(GameplayTag tag)
        {
            OnTagIsDirty?.Invoke();
        }

        public void Init(GameplayTag[] tags)
        {
            _fixedTags.Clear();
            _fixedTags.AddRange(tags);
        }

        public void OnEnable()
        {
            OnTagIsDirty += _owner.GameplayEffectContainer.RefreshGameplayEffectState;
        }

        public void OnDisable()
        {
            OnTagIsDirty -= _owner.GameplayEffectContainer.RefreshGameplayEffectState;
        }

        private bool TryAddFixedTag(GameplayTag tag)
        {
            var dirty = !_fixedTags.Contains(tag);
            if (dirty) _fixedTags.Add(tag);
            var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
            dirty = dirty || dynamicRemovedTagsRemoved;
            var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);
            dirty = dirty || dynamicAddedTagsRemoved;
            return dirty;
        }

        public void AddFixedTag(GameplayTag tag)
        {
            var dirty = TryAddFixedTag(tag);
            if (dirty) TagIsDirty(tag);
        }

        public void AddFixedTag(GameplayTagSet tagSet)
        {
            if (tagSet.Empty) return;
            var dirty = false;
            foreach (var tag in tagSet.Tags) dirty = dirty || TryAddFixedTag(tag);

            if (dirty) TagIsDirty(tagSet);
        }

        private bool TryRemoveFixedTag(GameplayTag tag)
        {
            var dirty = _fixedTags.Remove(tag);
            var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);
            dirty = dirty || dynamicAddedTagsRemoved;
            var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
            dirty = dirty || dynamicRemovedTagsRemoved;
            return dirty;
        }

        public void RemoveFixedTag(GameplayTag tag)
        {
            var dirty = TryRemoveFixedTag(tag);
            if (dirty) TagIsDirty(tag);
        }

        public void RemoveFixedTag(GameplayTagSet tagSet)
        {
            if (tagSet.Empty) return;
            var dirty = false;
            foreach (var tag in tagSet.Tags) dirty = dirty || TryRemoveFixedTag(tag);

            if (dirty) TagIsDirty(tagSet);
        }

        private bool TryAddDynamicAddedTag<T>(T source, GameplayTag tag)
        {
            if (!(source is GameplayEffectSpec) && !(source is AbilitySpec)) return false;
            var dirty = _dynamicRemovedTags.Remove(tag);
            if (_fixedTags.Contains(tag)) return dirty;

            if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
            {
                if (addedTag.Contains(source))
                {
                    return false;
                }

                addedTag.Add(source);
            }
            else
            {
                _dynamicAddedTags.Add(tag, new List<object> { source });
            }
            return true;
        }

        private bool TryAddDynamicRemovedTag<T>(T source, GameplayTag tag)
        {
            if (!(source is GameplayEffectSpec) && !(source is AbilitySpec)) return false;
            var dirty = _dynamicAddedTags.Remove(tag);
            if (!_fixedTags.Contains(tag)) return dirty;

            if (_dynamicRemovedTags.TryGetValue(tag, out var removedTag))
                removedTag.Add(source);
            else
                _dynamicRemovedTags.Add(tag, new List<object> { source });
            return true;
        }

        private bool TryRemoveDynamicTag<T>(ref Dictionary<GameplayTag, List<object>> dynamicTag, T source,
            GameplayTag tag)
        {
            if (!(source is GameplayEffectSpec) && !(source is AbilitySpec)) return false;
            var dirty = false;
            if (dynamicTag.TryGetValue(tag, out var tagList))
            {
                tagList.Remove(source);
                dirty = tagList.Count == 0;
                if (dirty) dynamicTag.Remove(tag);
            }

            return dirty;
        }

        private bool TryRemoveDynamicAddedTag<T>(T source, GameplayTag tag)
        {
            return TryRemoveDynamicTag(ref _dynamicAddedTags, source, tag);
        }

        private bool TryRemoveDynamicRemovedTag<T>(T source, GameplayTag tag)
        {
            return TryRemoveDynamicTag(ref _dynamicRemovedTags, source, tag);
        }

        public void ApplyGameplayEffectDynamicTag(GameplayEffectSpec source)
        {
            var tagIsDirty = false;
            var grantedTagSet = source.GameplayEffect.TagContainer.GrantedTags;
            foreach (var tag in grantedTagSet.Tags)
            {
                var dirty = TryAddDynamicAddedTag(source, tag);
                tagIsDirty = tagIsDirty || dirty;
            }

            if (tagIsDirty) TagIsDirty(grantedTagSet);
        }

        public void ApplyGameplayAbilityDynamicTag(AbilitySpec source)
        {
            var tagIsDirty = false;
            var activationOwnedTag = source.Ability.Tag.ActivationOwnedTag;
            foreach (var tag in activationOwnedTag.Tags)
            {
                var dirty = TryAddDynamicAddedTag(source, tag);
                tagIsDirty = tagIsDirty || dirty;
            }

            if (tagIsDirty) TagIsDirty(activationOwnedTag);
        }

        public void RestoreDynamicTags<T>(T source, GameplayTagSet tagSet)
        {
            var tagIsDirty = false;
            foreach (var tag in tagSet.Tags)
            {
                var dirty = TryRemoveDynamicAddedTag(source, tag);
                tagIsDirty = tagIsDirty || dirty;
            }

            if (tagIsDirty) TagIsDirty(tagSet);
        }

        public void RestoreGameplayEffectDynamicTags(GameplayEffectSpec effectSpec)
        {
            RestoreDynamicTags(effectSpec, effectSpec.GameplayEffect.TagContainer.GrantedTags);
        }

        public void RestoreGameplayAbilityDynamicTags(AbilitySpec abilitySpec)
        {
            RestoreDynamicTags(abilitySpec, abilitySpec.Ability.Tag.ActivationOwnedTag);
        }

        public bool HasTag(GameplayTag tag)
        {
            var fixedTagsContainsTag = _fixedTags.Any(t => t.HasTag(tag));
            var dynamicAddedTagsContainsTag = _dynamicAddedTags.Keys.Any(t => t.HasTag(tag));
            var dynamicRemovedTagsContainsTag = _dynamicRemovedTags.Keys.Any(t => t.HasTag(tag));
            return (fixedTagsContainsTag || dynamicAddedTagsContainsTag) && !dynamicRemovedTagsContainsTag;
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
            return !other.Empty && other.Tags.Any(HasTag);
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
        
        #if UNITY_EDITOR
        public List<GameplayTag> FixedTags => _fixedTags;
        public Dictionary<GameplayTag, List<object>> DynamicAddedTags => _dynamicAddedTags;
        #endif
    }
}