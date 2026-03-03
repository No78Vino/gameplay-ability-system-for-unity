using System;  
using System.Collections.Generic;  
  
namespace Framework.Core  
{  
    /// <summary>  
    /// 全局泛型事件总线。  
    /// 使用强类型 struct 作为事件载体，无字符串 key，编译期安全。  
    /// 无任何外部依赖，可跨项目复用。  
    /// </summary>  
    public static class GameEventBus  
    {  
        // 每个事件类型 T 对应一个独立的 Action<T>  
        private static readonly Dictionary<Type, Delegate> _handlers = new();  
  
        /// <summary>  
        /// 注册事件监听。  
        /// 同一 handler 重复注册不会重复触发（Delegate 自动去重）。  
        /// </summary>  
        public static void Register<T>(Action<T> handler) where T : struct  
        {  
            var type = typeof(T);  
            if (_handlers.TryGetValue(type, out var existing))  
                _handlers[type] = Delegate.Combine(existing, handler);  
            else  
                _handlers[type] = handler;  
        }  
  
        /// <summary>  
        /// 注销事件监听。  
        /// </summary>  
        public static void Unregister<T>(Action<T> handler) where T : struct  
        {  
            var type = typeof(T);  
            if (!_handlers.TryGetValue(type, out var existing)) return;  
  
            var newDelegate = Delegate.Remove(existing, handler);  
            if (newDelegate == null)  
                _handlers.Remove(type);  
            else  
                _handlers[type] = newDelegate;  
        }  
  
        /// <summary>  
        /// 广播事件。所有已注册该类型的 handler 都会被调用。  
        /// </summary>  
        public static void Dispatch<T>(T evt) where T : struct  
        {  
            var type = typeof(T);  
            if (_handlers.TryGetValue(type, out var existing))  
                ((Action<T>)existing)?.Invoke(evt);  
        }  
  
        /// <summary>  
        /// 清除所有事件监听（场景卸载时调用，防止野引用）。  
        /// </summary>  
        public static void Clear()  
        {  
            _handlers.Clear();  
        }  
    }  
  
    // ──────────────────────────────────────────────  
    // 框架级约定事件结构体（业务层可在自己的文件中扩展）  
    // ──────────────────────────────────────────────  
  
    /// <summary>单位死亡事件</summary>  
    public struct UnitDeadEvent  
    {  
        /// <summary>死亡的单位（MonoBehaviour）</summary>  
        public UnityEngine.GameObject Unit;  
          
        /// <summary>可选：死亡原因 Tag ID（由 GAS Tag 体系定义）</summary>  
        public int CauseTagId;  
    }  
  
    /// <summary>关卡结束事件</summary>  
    public struct LevelEndEvent  
    {  
        public LevelResult Result;  
    }  
  
    /// <summary>波次开始事件</summary>  
    public struct WaveStartEvent  
    {  
        public int WaveIndex;  
    }  
  
    /// <summary>关卡结果枚举</summary>  
    public enum LevelResult  
    {  
        Win,  
        Lose,  
        Quit,  
    }  
}