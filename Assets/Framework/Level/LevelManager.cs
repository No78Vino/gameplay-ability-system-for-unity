using Framework.Core;  
using Framework.Unit;  
using GAS.Runtime;  
using UnityEngine;  
using XYooAsset;  
using YooAsset;  
  
namespace Framework.Level  
{  
    /// <summary>  
    /// 关卡生命周期管理器（MonoBehaviour 单例，DontDestroyOnLoad）。  
    /// 职责：  
    ///   - 异步加载/卸载关卡场景（通过 XYoo/YooAsset）  
    ///   - 驱动 LevelBase 的 OnInit → OnStart → OnPause/Resume → OnEnd 生命周期  
    ///   - 关卡卸载时调用 GASManager.Stop() 防止 ECS World 泄漏  
    ///   - 关卡卸载时调用 GameEventBus.Clear() / UnitManager.Clear()  
    /// 对应现有：GameManager（场景加载部分）  
    /// </summary>  
    public class LevelManager : MonoBehaviour  
    {  
        // ── 单例 ──  
        private static LevelManager _instance;  
        public static LevelManager Instance  
        {  
            get  
            {  
                if (_instance == null)  
                    _instance = FindObjectOfType<LevelManager>();  
                return _instance;  
            }  
        }  
  
        private void Awake()  
        {  
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }  
            _instance = this;  
        }  
  
        // ── 当前关卡 ──  
        private LevelBase _currentLevel;  
        public LevelBase CurrentLevel => _currentLevel;  
  
        // ── 关卡状态 ──  
        public bool IsLevelRunning => _currentLevel != null;  
  
        // ──────────────────────────────────────────  
        // 加载关卡  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 加载指定场景并启动关卡。  
        /// LevelBase 子类实例由外部传入（工厂模式），场景加载完成后自动调用 OnInit/OnStart。  
        /// 对应现有 GameManager.LoadMainScene()。  
        /// </summary>  
        /// <param name="level">已实例化的 LevelBase 子类对象</param>  
        public void LoadLevel(LevelBase level)  
        {  
            if (_currentLevel != null)  
            {  
                Debug.LogWarning("[LevelManager] 已有关卡运行，请先卸载当前关卡。");  
                return;  
            }  
  
            _currentLevel = level;  
            Debug.Log($"[LevelManager] 加载关卡场景：{level.SceneName}");  
  
            // 异步加载场景（对应 GameManager.XYoo.LoadSingleSceneAsync）  
            XYoo.LoadSingleSceneAsync(level.SceneName, OnSceneLoaded);  
        }  
  
        private void OnSceneLoaded(SceneHandle handle)  
        {  
            Debug.Log($"[LevelManager] 场景加载完成：{handle.SceneName}");  
  
            // 初始化并启动关卡  
            _currentLevel.OnInit();  
            _currentLevel.OnStart();  
        }  
  
        // ──────────────────────────────────────────  
        // 暂停 / 恢复  
        // ──────────────────────────────────────────  
  
        public void PauseLevel()  
        {  
            _currentLevel?.OnPause();  
        }  
  
        public void ResumeLevel()  
        {  
            _currentLevel?.OnResume();  
        }  
  
        // ──────────────────────────────────────────  
        // 卸载关卡  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 卸载当前关卡，清理所有框架层状态。  
        /// 调用顺序：LevelBase.OnEnd → UnitManager.Clear → GameEventBus.Clear → GASManager.Stop  
        /// </summary>  
        public void UnloadCurrentLevel(LevelResult result = LevelResult.Quit)  
        {  
            if (_currentLevel == null) return;  
  
            Debug.Log($"[LevelManager] 卸载关卡，结果：{result}");  
  
            // 1. 通知关卡结束  
            _currentLevel.OnEnd(result);  
            _currentLevel = null;  
  
            // 2. 清理单位注册表  
            UnitManager.Instance.Clear();  
  
            // 3. 清理事件总线（防野引用）  
            GameEventBus.Clear();  
  
            // 4. 停止 ECS GAS World（防 ECS World 泄漏）  
            GASManager.Stop();  
        }  
  
        // ──────────────────────────────────────────  
        // 便捷：监听 LevelEndEvent 自动卸载（可选接入）  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 订阅 LevelEndEvent，自动在关卡结束时卸载。  
        /// 在 GameEntry 初始化后调用此方法启用自动卸载。  
        /// </summary>  
        public void EnableAutoUnload()  
        {  
            GameEventBus.Register<LevelEndEvent>(OnLevelEnd);  
        }  
  
        public void DisableAutoUnload()  
        {  
            GameEventBus.Unregister<LevelEndEvent>(OnLevelEnd);  
        }  
  
        private void OnLevelEnd(LevelEndEvent e)  
        {  
            UnloadCurrentLevel(e.Result);  
        }  
    }  
}