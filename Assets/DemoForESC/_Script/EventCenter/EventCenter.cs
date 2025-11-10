using System;
using System.Collections.Generic;

namespace DemoForESC._Script
{
    /// <summary>
    ///     事件中心，一个建议的消息系统
    /// </summary>
    public static class EventCenter
    {
        private static readonly Dictionary<string, Action<object>> eventDictionary = new();

        /// <summary>
        ///     注册事件
        /// </summary>
        public static void Register(string eventName, Action<object> listener)
        {
            if (!eventDictionary.ContainsKey(eventName))
                eventDictionary[eventName] = listener;
            else
                eventDictionary[eventName] += listener;
        }

        /// <summary>
        ///     注销事件
        /// </summary>
        public static void Unregister(string eventName, Action<object> listener)
        {
            if (!eventDictionary.ContainsKey(eventName)) return;
            eventDictionary[eventName] -= listener;
            if (eventDictionary[eventName] == null) eventDictionary.Remove(eventName);
        }

        /// <summary>
        ///     触发事件
        /// </summary>
        public static void Trigger(string eventName, object eventData = null)
        {
            if (eventDictionary.TryGetValue(eventName, out var value)) value?.Invoke(eventData);
        }
    }
}