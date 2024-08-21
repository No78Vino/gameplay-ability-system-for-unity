using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public class GameplayTagAggregator
    {
        private AbilitySystemComponent _owner;

        private Dictionary<GameplayTag, List<object>> _dynamicAddedTags = new();

        private Dictionary<GameplayTag, List<object>> _dynamicRemovedTags = new();

        private readonly List<GameplayTag> _fixedTags = new();

        public GameplayTagAggregator(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        private event Action OnTagIsDirty;

        private void TagIsDirty(in GameplayTagSet tags)
        {
            if (tags.Empty) return;

            TriggerTagIsDirty();
        }

        private void TagIsDirty(in GameplayTag tag)
        {
            TriggerTagIsDirty();
        }

        private void TriggerTagIsDirty()
        {
            Profiler.BeginSample($"{nameof(GameplayTagAggregator)}::TriggerTagIsDirty");
            OnTagIsDirty?.Invoke();
            Profiler.EndSample();
        }

        public void Init(IEnumerable<GameplayTag> tags)
        {
            _fixedTags.Clear();
            _fixedTags.AddRange(tags);
        }

        public void OnEnable()
        {
            Profiler.BeginSample($"[GC Mark] {nameof(GameplayTagAggregator)}::OnEnable().OnTagIsDirty +=");
            // 有 GC, 无法避免
            OnTagIsDirty += _owner.GameplayEffectContainer.RefreshGameplayEffectState;
            Profiler.EndSample();
        }

        public void OnDisable()
        {
            OnTagIsDirty -= _owner.GameplayEffectContainer.RefreshGameplayEffectState;
        }


        private static bool IsTagInList(in GameplayTag tag, IEnumerable<GameplayTag> tags)
        {
            foreach (var t in tags)
                if (t == tag)
                    return true;

            return false;
        }

        private bool TryAddFixedTag(in GameplayTag tag)
        {
            var added = false;

            if (!IsTagInList(tag, _fixedTags))
            {
                _fixedTags.Add(tag);
                added = true;
            }

            var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
            var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);

            return added || dynamicRemovedTagsRemoved || dynamicAddedTagsRemoved;
        }

        public void AddFixedTag(in GameplayTag tag)
        {
            var dirty = TryAddFixedTag(tag);
            if (dirty) TagIsDirty(tag);
        }

        public void AddFixedTag(in GameplayTagSet tagSet)
        {
            if (tagSet.Empty) return;
            var dirty = false;
            foreach (var tag in tagSet.Tags) dirty = dirty || TryAddFixedTag(tag);

            if (dirty) TagIsDirty(tagSet);
        }

        private bool TryRemoveFixedTag(in GameplayTag tag)
        {
            var dirty = _fixedTags.Remove(tag);
            var dynamicAddedTagsRemoved = _dynamicAddedTags.Remove(tag);
            dirty = dirty || dynamicAddedTagsRemoved;
            var dynamicRemovedTagsRemoved = _dynamicRemovedTags.Remove(tag);
            dirty = dirty || dynamicRemovedTagsRemoved;
            return dirty;
        }

        public void RemoveFixedTag(in GameplayTag tag)
        {
            var dirty = TryRemoveFixedTag(tag);
            if (dirty) TagIsDirty(tag);
        }

        public void RemoveFixedTag(in GameplayTagSet tagSet)
        {
            if (tagSet.Empty) return;
            var dirty = false;
            foreach (var tag in tagSet.Tags)
                dirty = dirty || TryRemoveFixedTag(tag);

            if (dirty) TagIsDirty(tagSet);
        }

        private bool TryAddDynamicAddedTag<T>(T source, in GameplayTag tag)
        {
            if (source is not GameplayEffectSpec && source is not AbilitySpec)
            {
                return false;
            }

            var dirty = _dynamicRemovedTags.Remove(tag);
            foreach (var t in _fixedTags)
            {
                if (t == tag)
                {
                    return dirty;
                }
            }

            if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
            {
                foreach (object o in addedTag)
                {
                    if (source.Equals(o))
                    {
                        return false;
                    }
                }

                addedTag.Add(source);
            }
            else
            {
                var list = ObjectPool.Instance.Fetch<List<object>>();
                list.Add(source);
                _dynamicAddedTags.Add(tag, list);
            }

            return true;
        }

        private bool TryAddDynamicRemovedTag<T>(T source, in GameplayTag tag)
        {
            if (source is not GameplayEffectSpec && source is not AbilitySpec) return false;
            var dirty = false;
            if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
            {
                addedTag.Clear();
                ObjectPool.Instance.Recycle(addedTag);
                dirty = _dynamicAddedTags.Remove(tag);
            }

            if (!IsTagInList(tag, _fixedTags)) return dirty;

            if (_dynamicRemovedTags.TryGetValue(tag, out var removedTag))
            {
                removedTag.Add(source);
            }
            else
            {
                var list = ObjectPool.Instance.Fetch<List<object>>();
                list.Add(source);
                _dynamicRemovedTags.Add(tag, list);
            }

            return true;
        }

        private bool TryRemoveDynamicTag<T>(Dictionary<GameplayTag, List<object>> dynamicTag, T source, in GameplayTag tag)
        {
            var dirty = false;

            if (source is GameplayEffectSpec || source is AbilitySpec)
            {
                if (dynamicTag.TryGetValue(tag, out var tagList))
                {
                    tagList.Remove(source);

                    dirty = tagList.Count == 0;
                    if (dirty)
                    {
                        ObjectPool.Instance.Recycle(tagList);
                        dynamicTag.Remove(tag);
                    }
                }
            }

            return dirty;
        }

        private bool TryRemoveDynamicAddedTag<T>(T source, in GameplayTag tag)
        {
            return TryRemoveDynamicTag(_dynamicAddedTags, source, tag);
        }

        private bool TryRemoveDynamicRemovedTag<T>(T source, in GameplayTag tag)
        {
            return TryRemoveDynamicTag(_dynamicRemovedTags, source, tag);
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

        public void RestoreDynamicTags<T>(T source, in GameplayTagSet tagSet)
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

        public bool HasTag(in GameplayTag tag)
        {
            foreach (var t in _dynamicRemovedTags.Keys)
                if (t.HasTag(tag))
                    return false;

            foreach (var t in _dynamicAddedTags.Keys)
                if (t.HasTag(tag))
                    return true;

            foreach (var t in _fixedTags)
                if (t.HasTag(tag))
                    return true;

            return false;
        }

        public bool HasAllTags(in GameplayTagSet other) => HasAllTags((IEnumerable<GameplayTag>)other.Tags);
        public bool HasAllTags(params GameplayTag[] tags) => HasAllTags((IEnumerable<GameplayTag>)tags);

        public bool HasAllTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
                if (!HasTag(tag))
                    return false;

            return true;
        }

        public bool HasAnyTags(in GameplayTagSet other) => HasAnyTags((IEnumerable<GameplayTag>)other.Tags);

        public bool HasAnyTags(params GameplayTag[] tags) => HasAnyTags((IEnumerable<GameplayTag>)tags);

        public bool HasAnyTags(IEnumerable<GameplayTag> tags)
        {
            foreach (var tag in tags)
                if (HasTag(tag))
                    return true;

            return false;
        }

        public bool HasNoneTags(in GameplayTagSet other) => HasNoneTags((IEnumerable<GameplayTag>)other.Tags);
        public bool HasNoneTags(params GameplayTag[] tags) => HasNoneTags((IEnumerable<GameplayTag>)tags);
        public bool HasNoneTags(IEnumerable<GameplayTag> tags) => HasAnyTags(tags) == false;

#if UNITY_EDITOR
        public List<GameplayTag> FixedTags => _fixedTags;
        public Dictionary<GameplayTag, List<object>> DynamicAddedTags => _dynamicAddedTags;
#endif
    }
}