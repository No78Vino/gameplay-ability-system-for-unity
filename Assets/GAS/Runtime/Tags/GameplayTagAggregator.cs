using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public class GameplayTagAggregator
    {
        private AbilitySystemComponent _owner;

        private Dictionary<GameplayTag, List<object>> _dynamicAddedTags =
            new Dictionary<GameplayTag, List<object>>();

        private Dictionary<GameplayTag, List<object>> _dynamicRemovedTags =
            new Dictionary<GameplayTag, List<object>>();

        private readonly List<GameplayTag> _fixedTags = new List<GameplayTag>();

        private static Pool _pool = new Pool(typeof(List<object>), 1024);

        public GameplayTagAggregator(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        private event Action OnTagIsDirty;

        private void TagIsDirty(GameplayTagSet tags)
        {
            Profiler.BeginSample($"{nameof(GameplayTagAggregator)}::TagIsDirty(GameplayTagSet)");
            if (!tags.Empty) OnTagIsDirty?.Invoke();
            Profiler.EndSample();
        }

        private void TagIsDirty(GameplayTag tag)
        {
            Profiler.BeginSample($"{nameof(GameplayTagAggregator)}::TagIsDirty(GameplayTag)");
            OnTagIsDirty?.Invoke();
            Profiler.EndSample();
        }

        public void Init(GameplayTag[] tags)
        {
            _fixedTags.Clear();
            _fixedTags.AddRange(tags);
        }

        public void OnEnable()
        {
            Profiler.BeginSample($"[GC Mark] {nameof(GameplayTagAggregator)}::OnEnable()");
            // 有 GC, 无法避免
            OnTagIsDirty += _owner.GameplayEffectContainer.RefreshGameplayEffectState;
            Profiler.EndSample();
        }

        public void OnDisable()
        {
            OnTagIsDirty -= _owner.GameplayEffectContainer.RefreshGameplayEffectState;
        }


        private static bool IsTagInList(GameplayTag tag, List<GameplayTag> tags)
        {
            foreach (var t in tags)
            {
                if (t == tag)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryAddFixedTag(GameplayTag tag)
        {
            var dirty = !IsTagInList(tag, _fixedTags);
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
            if (!(source is GameplayEffectSpec) && !(source is AbilitySpec))
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
                var list = _pool.Get() as List<object>;
                list.Add(source);
                _dynamicAddedTags.Add(tag, list);
            }

            return true;
        }

        private bool TryAddDynamicRemovedTag<T>(T source, GameplayTag tag)
        {
            if (!(source is GameplayEffectSpec) && !(source is AbilitySpec)) return false;
            var dirty = false;
            if (_dynamicAddedTags.TryGetValue(tag, out var addedTag))
            {
                addedTag.Clear();
                _pool.Return(addedTag);
                dirty = _dynamicAddedTags.Remove(tag);
            }

            if (!IsTagInList(tag, _fixedTags)) return dirty;

            if (_dynamicRemovedTags.TryGetValue(tag, out var removedTag))
                removedTag.Add(source);
            else
            {
                var list = _pool.Get() as List<object>;
                list.Add(source);
                _dynamicRemovedTags.Add(tag, list);
            }

            return true;
        }

        private bool TryRemoveDynamicTag<T>(ref Dictionary<GameplayTag, List<object>> dynamicTag, T source,
            GameplayTag tag)
        {
            var dirty = false;
            Profiler.BeginSample("TryRemoveDynamicTag");

            if (source is GameplayEffectSpec || source is AbilitySpec)
            {
                Profiler.BeginSample("[GC Mark]TryGetValue");
                var hasValue = dynamicTag.TryGetValue(tag, out var tagList);
                Profiler.EndSample();
                if (hasValue)
                {
                    Profiler.BeginSample("remove source from tag list");
                    tagList.Remove(source);
                    Profiler.EndSample();

                    dirty = tagList.Count == 0;
                    if (dirty)
                    {
                        _pool.Return(tagList);

                        Profiler.BeginSample("[GC Mark]remove dynamic tag");
                        dynamicTag.Remove(tag); // 有 GC
                        Profiler.EndSample();
                    }
                }
            }

            Profiler.EndSample();
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
            // LINQ表达式存在GC，且HasTag调用频率很高，所以这里全都使用foreach
            var fixedTagsContainsTag = false;
            foreach (var t in _fixedTags)
            {
                if (t.HasTag(tag))
                {
                    fixedTagsContainsTag = true;
                    break;
                }
            }

            var dynamicAddedTagsContainsTag = false;
            foreach (var t in _dynamicAddedTags)
            {
                if (t.Key.HasTag(tag))
                {
                    dynamicAddedTagsContainsTag = true;
                    break;
                }
            }

            var dynamicRemovedTagsContainsTag = false;
            foreach (var t in _dynamicRemovedTags)
            {
                if (t.Key.HasTag(tag))
                {
                    dynamicRemovedTagsContainsTag = true;
                    break;
                }
            }

            return (fixedTagsContainsTag || dynamicAddedTagsContainsTag) && !dynamicRemovedTagsContainsTag;
        }

        public bool HasAllTags(GameplayTagSet other)
        {
            if (other.Empty) return true;
            foreach (var tag in other.Tags)
                if (!HasTag(tag))
                    return false;

            return true;
        }

        public bool HasAllTags(params GameplayTag[] tags)
        {
            foreach (var tag in tags)
                if (!HasTag(tag))
                    return false;

            return true;
        }

        public bool HasAnyTags(GameplayTagSet other)
        {
            if (other.Empty) return false;
            foreach (var tag in other.Tags)
            {
                if (HasTag(tag)) return true;
            }

            return false;
            //return !other.Empty && other.Tags.Any(HasTag);
        }

        public bool HasAnyTags(params GameplayTag[] tags)
        {
            bool hasAny = false;
            foreach (var tag in tags)
                if (HasTag(tag))
                {
                    hasAny = true;
                    break;
                }

            return hasAny;
            //return tags.Any(HasTag);
        }

        public bool HasNoneTags(GameplayTagSet other)
        {
            if (other.Empty) return true;
            foreach (var tag in other.Tags)
            {
                if (HasTag(tag)) return false;
            }

            return true;
            //return other.Empty || !other.Tags.Any(HasTag);
        }

        public bool HasNoneTags(params GameplayTag[] tags)
        {
            if (tags.Length == 0) return true;
            foreach (var tag in tags)
            {
                if (HasTag(tag)) return false;
            }

            return true;
            //return !tags.Any(HasTag);
        }

#if UNITY_EDITOR
        public List<GameplayTag> FixedTags => _fixedTags;
        public Dictionary<GameplayTag, List<object>> DynamicAddedTags => _dynamicAddedTags;
#endif
    }
}