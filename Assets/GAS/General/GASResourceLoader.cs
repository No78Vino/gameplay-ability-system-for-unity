using System;  
using UnityEngine;  
using Object = UnityEngine.Object;  
  
namespace GAS.Runtime  
{  
    /// <summary>  
    /// EX-GAS 统一资源加载器  
    /// 框架内所有需要加载资源的地方（Cue、AbilityLogic等）统一通过此类加载，  
    /// 用户通过注册自定义的加载/释放委托来适配不同的资源管理方案（YooAsset、Addressables、AssetBundle等）。  
    /// 未注册时 fallback 到 Resources.Load。  
    /// </summary>  
    public static class GASResourceLoader  
    {  
        /// <summary>  
        /// 同步加载委托: (资源路径, 资源类型) => 资源对象  
        /// </summary>  
        private static Func<string, Type, Object> _loadSync;  
  
        /// <summary>  
        /// 异步加载委托: (资源路径, 资源类型, 加载完成回调)  
        /// </summary>  
        private static Action<string, Type, Action<Object>> _loadAsync;  
  
        /// <summary>  
        /// 资源释放委托: (资源对象)  
        /// </summary>  
        private static Action<Object> _release;  
  
        /// <summary>  
        /// 注册同步加载方法  
        /// </summary>  
        public static void RegisterLoadSync(Func<string, Type, Object> loadSync)  
        {  
            _loadSync = loadSync;  
        }  
  
        /// <summary>  
        /// 注册异步加载方法  
        /// </summary>  
        public static void RegisterLoadAsync(Action<string, Type, Action<Object>> loadAsync)  
        {  
            _loadAsync = loadAsync;  
        }  
  
        /// <summary>  
        /// 注册资源释放方法  
        /// </summary>  
        public static void RegisterRelease(Action<Object> release)  
        {  
            _release = release;  
        }  
  
        /// <summary>  
        /// 一次性注册所有方法  
        /// </summary>  
        public static void Register(  
            Func<string, Type, Object> loadSync,  
            Action<string, Type, Action<Object>> loadAsync,  
            Action<Object> release)  
        {  
            _loadSync = loadSync;  
            _loadAsync = loadAsync;  
            _release = release;  
        }  
  
        /// <summary>  
        /// 同步加载资源（泛型）  
        /// </summary>  
        public static T LoadSync<T>(string path) where T : Object  
        {  
            if (_loadSync != null)  
                return _loadSync(path, typeof(T)) as T;  
  
            // Fallback: Resources.Load  
#if UNITY_EDITOR  
            Debug.LogWarning($"[EX-GAS] GASResourceLoader.LoadSync 未注册自定义加载器，使用 Resources.Load 回退: {path}");  
#endif  
            return Resources.Load<T>(path);  
        }  
  
        /// <summary>  
        /// 同步加载资源（非泛型）  
        /// </summary>  
        public static Object LoadSync(string path, Type type)  
        {  
            if (_loadSync != null)  
                return _loadSync(path, type);  
  
#if UNITY_EDITOR  
            Debug.LogWarning($"[EX-GAS] GASResourceLoader.LoadSync 未注册自定义加载器，使用 Resources.Load 回退: {path}");  
#endif  
            return Resources.Load(path, type);  
        }  
  
        /// <summary>  
        /// 异步加载资源（泛型）  
        /// </summary>  
        public static void LoadAsync<T>(string path, Action<T> onComplete) where T : Object  
        {  
            if (_loadAsync != null)  
            {  
                _loadAsync(path, typeof(T), obj => onComplete?.Invoke(obj as T));  
                return;  
            }  
  
            // Fallback: Resources.LoadAsync  
#if UNITY_EDITOR  
            Debug.LogWarning($"[EX-GAS] GASResourceLoader.LoadAsync 未注册自定义加载器，使用 Resources.LoadAsync 回退: {path}");  
#endif  
            var request = Resources.LoadAsync<T>(path);  
            request.completed += _ => onComplete?.Invoke(request.asset as T);  
        }  
  
        /// <summary>  
        /// 释放资源  
        /// </summary>  
        public static void Release(Object asset)  
        {  
            if (asset == null) return;  
  
            if (_release != null)  
            {  
                _release(asset);  
                return;  
            }  
  
            // Fallback: Resources.UnloadAsset  
            Resources.UnloadAsset(asset);  
        }  
  
        /// <summary>  
        /// 是否已注册自定义加载器  
        /// </summary>  
        public static bool IsRegistered => _loadSync != null || _loadAsync != null;  
    }  
}