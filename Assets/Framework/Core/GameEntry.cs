using Framework.Level;  
using Framework.Unit;  
using GAS.Runtime;  
using SimpleJSON;  
using UnityEngine;  
using XYooAsset;  
  
namespace Framework.Core  
{  
    /// <summary>  
    /// 游戏启动入口（MonoBehaviour，挂在 DontDestroyOnLoad 的 GameObject 上）。  
    /// 职责：按序初始化所有子系统，串联各模块。  
    ///  
    /// 初始化顺序（强制）：  
    ///   1. XLauncher.Launch()         → 注册 Tag/Attr 缓存 + GASManager.Initialize()  
    ///   2. GASManager.Run()           → 启动 ECS GAS World（必须在任何 ASC 创建之前）  
    ///   3. XYoo.Initialize(pkg)       → 初始化 YooAsset 资源系统  
    ///   4. XUI.Launch()               → 初始化 UI 系统（可选，依赖项目 UI 框架）  
    ///   5. XLauncher.InitConfigTables → 加载 Luban JSON 配置表（必须在 UnitBase.Awake 之前）  
    ///   6. LevelManager.LoadLevel()   → 加载首个关卡  
    ///  
    /// 对应现有：DemoLauncher  
    /// </summary>  
    public class GameEntry : MonoBehaviour  
    {  
        [SerializeField] private GameSettings _settings;  
  
        private void Awake()  
        {  
            // 自身常驻，不随场景切换销毁  
            DontDestroyOnLoad(gameObject);  
  
            // ── Step 1：初始化 EX-GAS ECS World ──  
            // XLauncher.Launch() 内部调用：  
            //   XAbility.LoadAbilityCode() / XMmc.LoadMmcType() / XCue.LoadCueType()（缓存注册）  
            //   GASManager.Initialize()（创建 ECS World 和系统组）  
            //   XTag.InitTagList()（注意必须在 Initialize 之后）  
            XLauncher.Launch();  
  
            // ── Step 2：启动 GAS ECS Tick ──  
            GASManager.Run();  
  
            // ── Step 3：初始化资源系统（YooAsset）──  
            XYoo.Initialize(_settings.AssetPackageName);  
            XYooAssetManager.Instance.RegisterPackageInitComplete(OnAssetSystemReady);  
  
            // ── Step 4：初始化 UI 系统（按项目实际情况接入）──  
            // XUI.Launch();  
            // XUI.M.RegisterViewPrefabPath(UIConfig.WindowPathMap, XYooAssetManager.Instance.LoadAssetSync<GameObject>);  
  
            // ── 可选：启用 LevelManager 自动卸载 ──  
            LevelManager.Instance.EnableAutoUnload();  
  
            if (_settings.EnableDebugLog)  
                Debug.Log("[GameEntry] 初始化完成，等待资源系统就绪...");  
        }  
  
        // ──────────────────────────────────────────  
        // 资源系统就绪回调  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// YooAsset 包初始化完成后回调。  
        /// 此时才能安全加载 Luban JSON 配置表（配置表存储在 YooAsset 包中）。  
        /// 对应现有 DemoLauncher.OnXYooAssetInitComplete()。  
        /// </summary>  
        private void OnAssetSystemReady()  
        {  
            XYooAssetManager.Instance.UnregisterPackageInitComplete(OnAssetSystemReady);  
  
            // ── Step 5：加载 Luban JSON 配置表 ──  
            // 必须在所有 UnitBase.Awake（ASC.Init）之前完成  
            XLauncher.InitConfigTables(LoadConfig);  
  
            if (_settings.EnableDebugLog)  
                Debug.Log("[GameEntry] 配置表加载完成，准备加载首个关卡...");  
  
            // ── Step 6：加载首个关卡 ──  
            OnConfigReady();  
        }  
  
        /// <summary>  
        /// 从 YooAsset 加载 JSON 配置文件（供 XLauncher.InitConfigTables 使用）。  
        /// 对应现有 DemoLauncher.GetConfig()。  
        /// </summary>  
        private JSONNode LoadConfig(string fileName)  
        {  
            var path = $"{_settings.GameConfigDir}/{fileName}.json";  
            var textAsset = XYooAssetManager.Instance.LoadAssetSync<TextAsset>(path);  
            return JSON.Parse(textAsset.text);  
        }  
  
        // ──────────────────────────────────────────  
        // 关卡启动（子类重写或在此直接实现）  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 配置表就绪后，启动首个关卡。  
        /// 子类可重写此方法以接入菜单/加载界面等流程。  
        /// </summary>  
        protected virtual void OnConfigReady()  
        {  
            // 默认实现：直接加载起始关卡（跳过菜单）  
            // 实际项目中通常在此打开主菜单 UI，由玩家触发进入关卡  
            // LevelManager.Instance.LoadLevel(new Level_Tutorial());  
        }  
    }  
}