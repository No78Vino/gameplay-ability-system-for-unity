using System;
using System.Collections.Generic;
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
        }

        #region Attribute 事件

        private static readonly Dictionary<Entity, Dictionary<Tuple<int, int>, Action<float>>>
            _onBaseValueChangeBefore = new();

        public static void RegisterOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode,
            Action<float> action)
        {
            if (!_onBaseValueChangeBefore.ContainsKey(entity))
                _onBaseValueChangeBefore.Add(entity, new Dictionary<Tuple<int, int>, Action<float>>());

            if (!_onBaseValueChangeBefore[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode)))
                _onBaseValueChangeBefore[entity].Add(Tuple.Create(attrSetCode, attrCode), null);

            _onBaseValueChangeBefore[entity][Tuple.Create(attrSetCode, attrCode)] += action;
        }

        public static void UnRegisterOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode,
            Action<float> action)
        {
            if (!_onBaseValueChangeBefore.ContainsKey(entity)) return;
            if (!_onBaseValueChangeBefore[entity].ContainsKey(Tuple.Create(attrSetCode, attrCode))) return;

            _onBaseValueChangeBefore[entity][Tuple.Create(attrSetCode, attrCode)] -= action;

            var delegateList = _onBaseValueChangeBefore[entity][Tuple.Create(attrSetCode, attrCode)]
                ?.GetInvocationList();
            if (delegateList is { Length: 0 })
                _onBaseValueChangeBefore[entity].Remove(Tuple.Create(attrSetCode, attrCode));

            if (_onBaseValueChangeBefore[entity].Count == 0) _onBaseValueChangeBefore.Remove(entity);
        }

        public static void InvokeOnBaseValueChangeBefore(Entity entity, int attrSetCode, int attrCode, float value)
        {
            if (!_onBaseValueChangeBefore.TryGetValue(entity, out var dictionary)) return;

            if (dictionary.TryGetValue(Tuple.Create(attrSetCode, attrCode), out var action))
                action?.Invoke(value);
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
    }
}