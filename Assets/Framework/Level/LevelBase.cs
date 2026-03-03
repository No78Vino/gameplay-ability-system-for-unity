using Framework.Core;  
using UnityEngine;  
  
namespace Framework.Level  
{  
    /// <summary>  
    /// 关卡抽象基类（纯 C# 类，非 MonoBehaviour）。  
    /// 生命周期：OnInit → OnStart → [OnPause ↔ OnResume] → OnEnd  
    ///  
    /// 职责分工：  
    ///   - LevelBase：定义关卡初始化、单位配置、胜负响应等具体业务（子类实现）  
    ///   - LevelFlowController：维护状态机、波次推进、胜负判定（通过 GameEventBus 解耦通知）  
    ///  
    /// 依赖方向：LevelBase → LevelFlowController（单向）  
    ///           LevelFlowController --GameEventBus--> LevelBase（事件）  
    /// </summary>  
    public abstract class LevelBase  
    {  
        // ── FlowController ──  
        protected LevelFlowController FlowController { get; } = new();  
  
        // ── 关卡场景名（用于 LevelManager 加载场景）──  
        public abstract string SceneName { get; }  
  
        // ──────────────────────────────────────────  
        // 生命周期（由 LevelManager 驱动）  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 场景加载完成后调用。  
        /// 子类重写：配置 FlowController、初始化 GlobalAsc 等。  
        /// </summary>  
        public virtual void OnInit()  
        {  
            // 订阅关卡结束事件（由 FlowController 广播）  
            GameEventBus.Register<LevelEndEvent>(OnLevelEnd);  
            GameEventBus.Register<WaveStartEvent>(OnWaveStart);  
  
            // 默认配置：1 波，使用 XTag.Enemy 作为敌方 Tag，可由子类 override 后调用 base.OnInit()  
            ConfigureFlow();  
        }  
  
        /// <summary>  
        /// 配置 FlowController 的入口，子类重写以设置波次数、Tag、胜负条件。  
        /// 必须在 OnInit 内（或之后）被调用。  
        /// </summary>  
        protected virtual void ConfigureFlow()  
        {  
            // 子类示例：  
            // FlowController.Configure(totalWaves: 3, enemyTagId: XTag.Enemy);  
            // FlowController.OnWaveSpawnRequested += SpawnWave;  
        }  
  
        /// <summary>场景激活后、正式开始游戏逻辑时调用。</summary>  
        public virtual void OnStart()  
        {  
            FlowController.StartFlow();  
        }  
  
        /// <summary>暂停关卡（如弹出菜单）。</summary>  
        public virtual void OnPause()  
        {  
            FlowController.Pause();  
        }  
  
        /// <summary>从暂停恢复。</summary>  
        public virtual void OnResume()  
        {  
            FlowController.Resume();  
        }  
  
        /// <summary>  
        /// 关卡结束时调用（胜/负/退出）。  
        /// 子类重写以处理结算 UI、数据上报等。  
        /// </summary>  
        public virtual void OnEnd(LevelResult result)  
        {  
            FlowController.StopFlow();  
  
            // 注销事件  
            GameEventBus.Unregister<LevelEndEvent>(OnLevelEnd);  
            GameEventBus.Unregister<WaveStartEvent>(OnWaveStart);  
  
            Debug.Log($"[LevelBase] Level ended: {result}");  
        }  
  
        // ──────────────────────────────────────────  
        // 事件回调  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 接收 FlowController 广播的 LevelEndEvent，转发给 OnEnd。  
        /// </summary>  
        private void OnLevelEnd(LevelEndEvent e)  
        {  
            OnEnd(e.Result);  
        }  
  
        /// <summary>  
        /// 每波开始时触发，子类重写以执行实际 Spawn 逻辑。  
        /// 也可以在 ConfigureFlow 中订阅 FlowController.OnWaveSpawnRequested 委托。  
        /// </summary>  
        protected virtual void OnWaveStart(WaveStartEvent e)  
        {  
            // 子类示例：  
            // UnitManager.Instance.SpawnUnit(goblinPrefab, new Vector3(0, 0, 10));  
        }  
    }  
}