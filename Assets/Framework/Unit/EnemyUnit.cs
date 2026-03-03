using Framework.Core;  
using GAS.Runtime;  
using UnityEngine;  
  
namespace Framework.Unit  
{  
    /// <summary>  
    /// 敌人单位基类。  
    /// - 继承 UnitBase，Awake 时自动完成 ASC 初始化和 UnitManager 注册  
    /// - 持有固有阵营 Tag（Faction.Enemy），供 UnitManager.GetUnitsWithTag / LevelFlowController 胜负判定使用  
    /// - 内置简单 AI：感知玩家 → 激活攻击 Ability  
    /// - 子类可重写 OnAITick() 实现更复杂的 AI 逻辑  
    /// </summary>  
    public class EnemyUnit : UnitBase  
    {  
        [Header("AI 配置")]  
        [SerializeField] protected int _attackAbilityId = 0;       // 攻击 Ability ID（在 Inspector 填入或子类赋值）  
        [SerializeField] protected float _detectionRange = 8f;      // 感知半径（Unity 单位）  
        [SerializeField] protected float _attackInterval = 2f;      // AI 攻击间隔（秒）  
  
        private float _attackTimer = 0f;  
        private PlayerUnit _cachedPlayer;  
  
        protected override void Awake()  
        {  
            base.Awake(); // → ASC 初始化 + UnitManager 注册  
  
            // 添加敌方阵营 Tag（Faction.Enemy），LevelFlowController 默认胜负判定依赖此 Tag  
            ASC.Cell.AddFixedTag(XTag.Faction_Enemy);  
        }  
  
        protected override void RegisterAttributeCallbacks()  
        {  
            // Hp 钳制：不超过 HpMax  
            GASEventCenter.SetOnAttrBaseValueChangeBefore(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp,  
                v => Mathf.Min(v, ASC.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax)));  
  
            // Hp 归零时销毁单位（触发 UnitBase.OnDestroy → UnitDeadEvent）  
            GASEventCenter.RegisterOnAttrCurrentValueChangeAfter(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp, OnHpChangeAfter);  
        }  
  
        protected override void UnregisterAttributeCallbacks()  
        {  
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp);  
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp, OnHpChangeAfter);  
        }  
  
        private void OnHpChangeAfter(float lastHp, float newHp)  
        {  
            if (newHp <= 0)  
                Destroy(gameObject); // → 触发 UnitBase.OnDestroy → UnitDeadEvent → LevelFlowController 判定  
        }  
  
        // ──────────────────────────────────────────  
        // 简单 AI（每帧轮询）  
        // ──────────────────────────────────────────  
  
        private void Update()  
        {  
            OnAITick();  
        }  
  
        /// <summary>  
        /// AI 主循环，子类重写以实现自定义 AI。  
        /// 默认行为：定时检测范围内的 PlayerUnit，激活攻击 Ability。  
        /// </summary>  
        protected virtual void OnAITick()  
        {  
            if (_attackAbilityId == 0) return;  
  
            _attackTimer -= Time.deltaTime;  
            if (_attackTimer > 0) return;  
  
            // 延迟查找（避免每帧 GetUnit 开销）  
            _cachedPlayer ??= UnitManager.Instance.GetUnit<PlayerUnit>();  
            if (_cachedPlayer == null) return;  
  
            var dist = Vector3.Distance(transform.position, _cachedPlayer.transform.position);  
            if (dist <= _detectionRange)  
            {  
                TryActivateAbility(_attackAbilityId);  
                _attackTimer = _attackInterval;  
            }  
        }  
  
        // ──────────────────────────────────────────  
        // 通用行为（可选重写）  
        // ──────────────────────────────────────────  
  
        public override void Attack()  
        {  
            if (_attackAbilityId != 0)  
                TryActivateAbility(_attackAbilityId);  
        }  
    }  
}