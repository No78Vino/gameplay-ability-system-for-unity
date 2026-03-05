using System;  
using System.Collections.Generic;  
using UnityEngine;  
  
namespace EXUI  
{  
    /// <summary>  
    /// XUI 配置胶水层接口。  
    /// Runtime 层只依赖此接口，不感知底层配置格式（SO / JSON / CSV / Text 等）。  
    /// </summary>  
    public interface IXUIConfig  
    {  
        /// <summary>  
        /// 返回 View 类型 → Prefab 地址（字符串路径/地址）的映射  
        /// </summary>  
        Dictionary<Type, string> GetViewPrefabPathMap();  
  
        /// <summary>  
        /// 同步 Prefab 加载委托（可选，小型项目使用）  
        /// </summary>  
        Func<string, GameObject> GetSyncLoader();  
  
        /// <summary>  
        /// 异步 Prefab 加载委托（可选，null 表示不支持异步）  
        /// </summary>  
        Func<string, System.Threading.Tasks.Task<GameObject>> GetAsyncLoader();  
    }  
}