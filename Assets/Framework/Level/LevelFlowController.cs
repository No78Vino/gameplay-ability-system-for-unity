using System;  
using System.Collections.Generic;  
using Framework.Core;  
using Framework.Unit;  
using UnityEngine;  
  
namespace Framework.Level  
{  
    public enum FlowState  
    {  
        Idle,        // 未启动  
        Preparing,   // 准备阶段（加载资源等）  
        Running,     // 运行中  
        Paused,      // 暂停  
        Ended        // 已结束  
    }  
  
    /// <summary>  
    /// 关卡流程控制器（纯 C# 类，非 MonoBehaviour）。  
    /// 职责：  
    ///   - 维护 Idle→Preparing→Running→Paused→Ended 状态机  
    ///   - 驱动波次推进，通过 GameEventBus 广播 WaveStartEvent  
    ///   - 监听 UnitDeadEvent，判断胜负条件，广播 LevelEndEvent  
    /// 与 LevelBase 的关系：LevelBase 持有本类实例，单向依赖。  
    /// 本类通过 GameEventBus 事件通知 LevelBase，不持有 LevelBase 引用（避免循环依赖）。  
    /// </summary>  
    public class LevelFlowController  
    {  
        // ── 状态 ──  
        public FlowState State { get; private set; } = FlowState.Idle;  
        public int CurrentWave { get; private set; } = 0;  
        public int TotalWaves { get; private set; } = 1;  
  
        // ── 胜负判定：敌方 Tag ID（由外部在 Configure 时传入）──  
        private int _enemyTagId;  
  
        // ── 波次数据（每波 Spawn 信息由 LevelBase 子类通过委托提供）──  
        // Action<int waveIndex>：由 LevelBase 订阅，负责实际 Spawn 单位  
        public event Action<int> OnWaveSpawnRequested;  
  
        // ── 胜利/失败判定委托（可由 LevelBase 子类重写）──  
        // 返回 true = 玩家胜利  
        private Func<bool> _winCondition;  
        // 返回 true = 玩家失败  
        private Func<bool> _loseCondition;  
  
        // ──────────────────────────────────────────  
        // 配置入口  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 初始化配置。在 LevelBase.OnInit 中调用。  
        /// </summary>  
        /// <param name="totalWaves">总波次数</param>  
        /// <param name="enemyTagId">敌方单位的 GameplayTag ID，用于胜负判定</param>  
        /// <param name="winCondition">自定义胜利条件（null 则默认为"敌方全灭"）</param>  
        /// <param name="loseCondition">自定义失败条件（null 则默认为"玩家死亡"）</param>  
        public void Configure(  
            int totalWaves,  
            int enemyTagId,  
            Func<bool> winCondition = null,  
            Func<bool> loseCondition = null)  
        {  
            TotalWaves = totalWaves;  
            _enemyTagId = enemyTagId;  
            _winCondition = winCondition ?? DefaultWinCondition;  
            _loseCondition = loseCondition ?? DefaultLoseCondition;  
        }  
  
        // ──────────────────────────────────────────  
        // 生命周期  
        // ──────────────────────────────────────────  
  
        /// <summary>启动流程。由 LevelBase.OnStart 调用。</summary>  
        public void StartFlow()  
        {  
            if (State != FlowState.Idle && State != FlowState.Preparing)  
            {  
                Debug.LogWarning("[LevelFlowController] StartFlow called in invalid state: " + State);  
                return;  
            }  
  
            State = FlowState.Running;  
            CurrentWave = 0;  
  
            // 订阅单位死亡事件  
            GameEventBus.Register<UnitDeadEvent>(OnUnitDead);  
  
            // 推进第一波  
            AdvanceWave();  
        }  
  
        /// <summary>暂停流程（不影响 ECS Tick，仅框架层状态）。</summary>  
        public void Pause()  
        {  
            if (State == FlowState.Running)  
                State = FlowState.Paused;  
        }  
  
        /// <summary>恢复流程。</summary>  
        public void Resume()  
        {  
            if (State == FlowState.Paused)  
                State = FlowState.Running;  
        }  
  
        /// <summary>  
        /// 停止并清理。由 LevelBase.OnEnd 调用。  
        /// </summary>  
        public void StopFlow()  
        {  
            GameEventBus.Unregister<UnitDeadEvent>(OnUnitDead);  
            State = FlowState.Ended;  
        }  
  
        // ──────────────────────────────────────────  
        // 波次推进  
        // ──────────────────────────────────────────  
  
        private void AdvanceWave()  
        {  
            CurrentWave++;  
            Debug.Log($"[LevelFlowController] Wave {CurrentWave}/{TotalWaves} start");  
  
            // 广播波次开始事件（LevelBase 子类订阅后执行 Spawn）  
            GameEventBus.Dispatch(new WaveStartEvent { WaveIndex = CurrentWave });  
  
            // 同时通过委托通知（两路选其一，委托更直接）  
            OnWaveSpawnRequested?.Invoke(CurrentWave);  
        }  
  
        /// <summary>  
        /// 当前波次所有敌人清除后，由外部调用推进下一波。  
        /// 也可以由 OnUnitDead 自动触发（见下方逻辑）。  
        /// </summary>  
        public void TryAdvanceToNextWave()  
        {  
            if (State != FlowState.Running) return;  
  
            if (CurrentWave < TotalWaves)  
            {  
                AdvanceWave();  
            }  
            else  
            {  
                // 最后一波已清，检查胜利  
                CheckWinLose();  
            }  
        }  
  
        // ──────────────────────────────────────────  
        // 胜负判定  
        // ──────────────────────────────────────────  
  
        private void OnUnitDead(UnitDeadEvent e)  
        {  
            if (State != FlowState.Running) return;  
            CheckWinLose();  
        }  
  
        private void CheckWinLose()  
        {  
            if (_loseCondition != null && _loseCondition())  
            {  
                EndLevel(LevelResult.Lose);  
                return;  
            }  
  
            if (_winCondition != null && _winCondition())  
            {  
                // 还有下一波？  
                if (CurrentWave < TotalWaves)  
                    AdvanceWave();  
                else  
                    EndLevel(LevelResult.Win);  
            }  
        }  
  
        private void EndLevel(LevelResult result)  
        {  
            StopFlow();  
            Debug.Log($"[LevelFlowController] Level ended: {result}");  
            GameEventBus.Dispatch(new LevelEndEvent { Result = result });  
        }  
  
        // ──────────────────────────────────────────  
        // 默认胜负条件  
        // ──────────────────────────────────────────  
  
        /// <summary>默认胜利条件：场上没有敌方 Tag 的单位</summary>  
        private bool DefaultWinCondition()  
        {  
            var enemies = UnitManager.Instance.GetUnitsWithTag(_enemyTagId);  
            return enemies.Count == 0;  
        }  
  
        /// <summary>默认失败条件：玩家单位不存在</summary>  
        private bool DefaultLoseCondition()  
        {  
            var player = UnitManager.Instance.GetUnit<PlayerUnit>();  
            return player == null;  
        }  
    }  
}