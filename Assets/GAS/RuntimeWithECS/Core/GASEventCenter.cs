using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Tag;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public static class GASEventCenter
    {
        public static void Clear()
        {
            // 属性事件
            _onBaseValueChangeBefore.Clear();
            _onBaseValueChangeAfter.Clear();
            _onCurrentValueChangeAfter.Clear();
            // GameplayEffect 事件
            _onGameplayEffectContainerIsDirty.Clear();
        }

        #region Attribute 事件

        private static readonly Dictionary<Entity, Dictionary<Tuple<int, int>, Func<float, float>>>
            _onBaseValueChangeBefore = new();

        public static void SetOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode,
            Func<float, float> action)
        {
            if (!_onBaseValueChangeBefore.ContainsKey(entity))
                _onBaseValueChangeBefore.Add(entity, new Dictionary<Tuple<int, int>, Func<float, float>>());

            _onBaseValueChangeBefore[entity][Tuple.Create(attrSetCode, attrCode)] = action;
        }

        public static void ClearOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode)
        {
            if (!_onBaseValueChangeBefore.TryGetValue(entity, out var value)) return;
            if (!value.ContainsKey(Tuple.Create(attrSetCode, attrCode))) return;
            _onBaseValueChangeBefore[entity].Remove(Tuple.Create(attrSetCode, attrCode));

            if (_onBaseValueChangeBefore[entity].Count == 0) _onBaseValueChangeBefore.Remove(entity);
        }

        public static float InvokeOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode, float value)
        {
            if (!_onBaseValueChangeBefore.TryGetValue(entity, out var dictionary)) return value;

            if (dictionary.TryGetValue(Tuple.Create(attrSetCode, attrCode), out var action))
                return action?.Invoke(value) ?? value;

            return value;
        }


        private static readonly Dictionary<Entity, Dictionary<Tuple<int, int>, Action<float, float>>>
            _onBaseValueChangeAfter = new();

        public static void RegisterOnBaseValueChangeAfter(Entity entity, int attrSetCode, int attrCode,
            Action<float, float> action)
        {
            if (!_onBaseValueChangeAfter.ContainsKey(entity))
                _onBaseValueChangeAfter.Add(entity, new Dictionary<Tuple<int, int>, Action<float, float>>());

            if (!_onBaseValueChangeAfter[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode)))
                _onBaseValueChangeAfter[entity].Add(Tuple.Create(attrSetCode, attrCode), null);

            _onBaseValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)] += action;
        }

        public static void UnRegisterOnBaseValueChangeAfter(Entity entity, int attrSetCode, int attrCode,
            Action<float, float> action)
        {
            if (!_onBaseValueChangeAfter.ContainsKey(entity)) return;
            if (!_onBaseValueChangeAfter[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode))) return;

            _onBaseValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)] -= action;

            var delegateList = _onBaseValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)]
                ?.GetInvocationList();
            if (delegateList is { Length: 0 })
                _onBaseValueChangeAfter[entity].Remove(Tuple.Create(attrSetCode, attrCode));
            if (_onBaseValueChangeAfter[entity].Count == 0) _onBaseValueChangeAfter.Remove(entity);
        }

        public static void InvokeOnBaseValueChangeAfter(Entity entity, int attrSetCode, int attrCode, float oldValue,
            float newValue)
        {
            if (!_onBaseValueChangeAfter.TryGetValue(entity, out var dictionary)) return;

            if (dictionary.TryGetValue(Tuple.Create(attrSetCode, attrCode), out var action))
                action?.Invoke(oldValue, newValue);
        }


        private static readonly Dictionary<Entity, Dictionary<Tuple<int, int>, Action<float, float>>>
            _onCurrentValueChangeAfter = new();

        public static void RegisterOnCurrentValueChangeAfter(Entity entity, int attrSetCode, int attrCode,
            Action<float, float> action)
        {
            if (!_onCurrentValueChangeAfter.ContainsKey(entity))
                _onCurrentValueChangeAfter.Add(entity, new Dictionary<Tuple<int, int>, Action<float, float>>());

            if (!_onCurrentValueChangeAfter[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode)))
                _onCurrentValueChangeAfter[entity].Add(Tuple.Create(attrSetCode, attrCode), null);

            _onCurrentValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)] += action;
        }

        public static void UnRegisterOnCurrentValueChangeAfter(Entity entity, int attrSetCode, int attrCode,
            Action<float, float> action)
        {
            if (!_onCurrentValueChangeAfter.ContainsKey(entity)) return;
            if (!_onCurrentValueChangeAfter[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode))) return;

            _onCurrentValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)] -= action;

            var delegateList = _onCurrentValueChangeAfter[entity][Tuple.Create(attrSetCode, attrCode)]
                ?.GetInvocationList();
            if (delegateList is { Length: 0 })
                _onCurrentValueChangeAfter[entity].Remove(Tuple.Create(attrSetCode, attrCode));

            if (_onCurrentValueChangeAfter[entity].Count == 0) _onCurrentValueChangeAfter.Remove(entity);
        }

        public static void InvokeOnCurrentValueChangeAfter(Entity entity, int attrSetCode, int attrCode, float oldValue,
            float newValue)
        {
            if (!_onCurrentValueChangeAfter.TryGetValue(entity, out var dictionary)) return;

            if (dictionary.TryGetValue(Tuple.Create(attrSetCode, attrCode), out var action))
                action?.Invoke(oldValue, newValue);
        }

        #endregion

        #region GameplayEffectContainerIsDirty 事件

        private static readonly Dictionary<Entity, Action> _onGameplayEffectContainerIsDirty = new();

        public static void RegisterOnGameplayEffectContainerIsDirty(Entity entity, Action action)
        {
            if (!_onGameplayEffectContainerIsDirty.TryAdd(entity, action))
                _onGameplayEffectContainerIsDirty[entity] =
                    (Action)Delegate.Combine(_onGameplayEffectContainerIsDirty[entity], action);
        }

        public static void UnRegisterOnGameplayEffectContainerIsDirty(Entity entity, Action action)
        {
            if (!_onGameplayEffectContainerIsDirty.TryGetValue(entity, out var existingAction)) return;
            var newAction = (Action)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onGameplayEffectContainerIsDirty.Remove(entity);
            else
                _onGameplayEffectContainerIsDirty[entity] = newAction;
        }

        public static void InvokeOnGameplayEffectContainerIsDirty(Entity dirtyAsc)
        {
            if (_onGameplayEffectContainerIsDirty.TryGetValue(dirtyAsc, out var action))
                action?.Invoke();
        }

        #endregion

        #region TryChangeGameplayEffectStackCount 事件

        private static readonly Dictionary<Entity, Action<int, int>> _onTryChangeGameplayEffectStackCount = new();

        public static void RegisterOnTryChangeGameplayEffectStackCount(Entity entity, Action<int, int> action)
        {
            if (!_onTryChangeGameplayEffectStackCount.TryAdd(entity, action))
                _onTryChangeGameplayEffectStackCount[entity] =
                    (Action<int, int>)Delegate.Combine(_onTryChangeGameplayEffectStackCount[entity], action);
        }

        public static void UnRegisterOnTryChangeGameplayEffectStackCount(Entity entity, Action<int, int> action)
        {
            if (!_onTryChangeGameplayEffectStackCount.TryGetValue(entity, out var existingAction)) return;
            var newAction = (Action<int, int>)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onTryChangeGameplayEffectStackCount.Remove(entity);
            else
                _onTryChangeGameplayEffectStackCount[entity] = newAction;
        }

        public static void InvokeOnTryChangeGameplayEffectStackCount(Entity entity, int oldStackCount,
            int newStackCount)
        {
            if (_onTryChangeGameplayEffectStackCount.TryGetValue(entity, out var action))
                action?.Invoke(oldStackCount, newStackCount);
        }

        #endregion

        #region Ability 事件

        // protected event Action<AbilityActivateResult> _onActivateResult;
        // protected event Action _onEndAbility;
        // protected event Action _onCancelAbility;
        
        private static readonly Dictionary<Entity, Action<AbilityActivationResult>> _onActivateResult = new();
        private static readonly Dictionary<Entity, Action> _onEndAbility = new();
        private static readonly Dictionary<Entity, Action> _onCancelAbility = new();
        
        public static void RegisterOnActivateResult(Entity abilityEntity, Action<AbilityActivationResult> action)
        {
            if (!_onActivateResult.TryAdd(abilityEntity, action))
                _onActivateResult[abilityEntity] = (Action<AbilityActivationResult>)Delegate.Combine(_onActivateResult[abilityEntity], action);
        }
        
        public static void UnRegisterOnActivateResult(Entity abilityEntity, Action<AbilityActivationResult> action)
        {
            if (!_onActivateResult.TryGetValue(abilityEntity, out var existingAction)) return;
            var newAction = (Action<AbilityActivationResult>)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onActivateResult.Remove(abilityEntity);
            else
                _onActivateResult[abilityEntity] = newAction;
        }
        
        public static void InvokeOnActivateResult(Entity abilityEntity, AbilityActivationResult result)
        {
            if (_onActivateResult.TryGetValue(abilityEntity, out var action))
                action?.Invoke(result);
        }
        
        public static void RegisterOnEndAbility(Entity abilityEntity, Action action)
        {
            if (!_onEndAbility.TryAdd(abilityEntity, action))
                _onEndAbility[abilityEntity] = (Action)Delegate.Combine(_onEndAbility[abilityEntity], action);
        }
        
        public static void UnRegisterOnEndAbility(Entity abilityEntity, Action action)
        {
            if (!_onEndAbility.TryGetValue(abilityEntity, out var existingAction)) return;
            var newAction = (Action)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onEndAbility.Remove(abilityEntity);
            else
                _onEndAbility[abilityEntity] = newAction;
        }
        
        public static void InvokeOnEndAbility(Entity abilityEntity)
        {
            if (_onEndAbility.TryGetValue(abilityEntity, out var action))
                action?.Invoke();
        }
        
        public static void RegisterOnCancelAbility(Entity abilityEntity, Action action)
        {
            if (!_onCancelAbility.TryAdd(abilityEntity, action))
                _onCancelAbility[abilityEntity] = (Action)Delegate.Combine(_onCancelAbility[abilityEntity], action);
        }
        
        public static void UnRegisterOnCancelAbility(Entity abilityEntity, Action action)
        {
            if (!_onCancelAbility.TryGetValue(abilityEntity, out var existingAction)) return;
            var newAction = (Action)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onCancelAbility.Remove(abilityEntity);
            else
                _onCancelAbility[abilityEntity] = newAction;
        }
        
        public static void InvokeOnCancelAbility(Entity abilityEntity)
        {
            if (_onCancelAbility.TryGetValue(abilityEntity, out var action))
                action?.Invoke();
        }
        #endregion

        #region Tag 事件

        private static readonly Dictionary<Entity, Action<Entity,int,GameplayTagChangeEvent>> _onTagIsDirty = new();
        
        public static void RegisterOnTagIsDirty(Entity entity, Action<Entity,int,GameplayTagChangeEvent> action)
        {
            if (!_onTagIsDirty.TryAdd(entity, action))
                _onTagIsDirty[entity] = (Action<Entity,int,GameplayTagChangeEvent>)Delegate.Combine(_onTagIsDirty[entity], action);
        }
        
        public static void UnRegisterOnTagIsDirty(Entity entity, Action<Entity,int,GameplayTagChangeEvent> action)
        {
            if (!_onTagIsDirty.TryGetValue(entity, out var existingAction)) return;
            var newAction = (Action<Entity,int,GameplayTagChangeEvent>)Delegate.Remove(existingAction, action);
            if (newAction == null)
                _onTagIsDirty.Remove(entity);
            else
                _onTagIsDirty[entity] = newAction;
        }
        
        public static void InvokeOnTagIsDirty(Entity entity, int tag, GameplayTagChangeEvent changeEvent)
        {
            if (_onTagIsDirty.TryGetValue(entity, out var action))
                action?.Invoke(entity,tag,changeEvent);
        }

        #endregion
    }
}