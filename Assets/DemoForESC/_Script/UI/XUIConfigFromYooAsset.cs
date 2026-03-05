using System;  
using System.Collections.Generic;  
using System.Threading.Tasks;  
using DemoForESC._Script.UI.View;  
using EXUI;  
using UnityEngine;  
using XYooAsset;  
  
namespace DemoForESC._Script.UI  
{  
    /// <summary>  
    /// IXUIConfig 的 YooAsset 实现。  
    /// 封装 WindowPathMap 和 YooAsset 加载委托，Runtime 层不感知具体加载方式。  
    /// </summary>  
    public class XUIConfigFromYooAsset : IXUIConfig  
    {  
        private readonly Dictionary<Type, string> _pathMap;  
  
        public XUIConfigFromYooAsset()  
        {  
            // 路径映射仍来自 UIConfig，但只在此处构造，Runtime 层不感知  
            _pathMap = new Dictionary<Type, string>  
            {  
                [typeof(MaskWindow)]  = "Assets/DemoForESC/Resources/Prefabs/UI/MaskWindow",  
                [typeof(MenuWindow)]  = "Assets/DemoForESC/Resources/Prefabs/UI/MenuWindow",  
                [typeof(MainWindow)]  = "Assets/DemoForESC/Resources/Prefabs/UI/MainWindow",  
                [typeof(GuideWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/GuideWindow",  
                [typeof(DeathWindow)] = "Assets/DemoForESC/Resources/Prefabs/UI/DeathWindow",  
            };  
        }

        public XUIConfigFromYooAsset(Dictionary<Type, string> map)
        {
            _pathMap = map;
        }
        

        public Dictionary<Type, string> GetViewPrefabPathMap() => _pathMap;  
  
        // ✅ 同步加载：委托给 YooAsset 的 LoadAssetSync  
        public Func<string, GameObject> GetSyncLoader()  
            => XYooAssetManager.Instance.LoadAssetSync<GameObject>;  
  
        // ✅ 异步加载：委托给 YooAsset 的 LoadAssetAsync（Step 8 中已开放的异步接口）  
        public Func<string, Task<GameObject>> GetAsyncLoader()  
        {  
            return (path) =>  
            {  
                var tcs = new TaskCompletionSource<GameObject>();  
                // ✅ 签名匹配：(string assetPath, Action<TObject> completed)  
                XYooAssetManager.Instance.LoadAssetAsync<GameObject>(path, go =>  
                {  
                    if (go != null)  
                        tcs.SetResult(go);  
                    else  
                        tcs.SetException(new Exception($"[EXUI] Failed to load prefab async: {path}"));  
                });  
                return tcs.Task;  
            };  
        }
    }  
}