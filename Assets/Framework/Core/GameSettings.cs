using UnityEngine;  
  
namespace Framework.Core  
{  
    /// <summary>  
    /// 全局游戏参数配置（ScriptableObject）。  
    /// 在 Project 窗口右键 → Create/Framework/GameSettings 创建资产。  
    /// 运行时只读，通过 GameEntry 注入各模块。  
    /// </summary>  
    [CreateAssetMenu(menuName = "Framework/GameSettings", fileName = "GameSettings")]  
    public class GameSettings : ScriptableObject  
    {  
        [Header("资源系统")]  
        [Tooltip("YooAsset 资源包名称")]  
        public string AssetPackageName = "MainPackage";  
  
        [Header("配置表")]  
        [Tooltip("EX-GAS Luban JSON 表所在目录（相对于 StreamingAssets）")]  
        public string GameConfigDir = "Tables";  
  
        [Header("关卡")]  
        [Tooltip("游戏启动后加载的第一个关卡场景名")]  
        public string StartSceneName = "Level_01";  
  
        [Tooltip("起始关卡对应的 ASC 预设 ID（用于全局 GlobalAsc 初始化，可为 0 不用）")]  
        public int GlobalAscPresetId = 0;  
  
        [Header("玩家")]  
        [Tooltip("玩家 ASC 预设 ID，对应 exgas_tbasc.json 中的配置")]  
        public int PlayerAscPresetId = 1;  
  
        [Header("调试")]  
        [Tooltip("是否开启框架调试日志")]  
        public bool EnableDebugLog = true;  
    }  
}