// Assets/Framework/Unit/PlayerUnit.cs  
using DemoForESC._Script.Gas.Ability; // XParamMove  
using Framework.Core;  
using GAS.Runtime;  
using UnityEngine;  
  
namespace Framework.Unit  
{  
    /// <summary>  
    /// 玩家单位。  
    /// - 继承 UnitBase，Awake 时自动完成 ASC 初始化和 UnitManager 注册  
    /// - 暴露语义化 Ability 触发方法供 PlayerController 调用  
    /// - Move 使用主摄像机方向（而非 transform.forward），与 DemoPlayer 保持一致  
    /// - 获取实例：UnitManager.Instance.GetUnit&lt;PlayerUnit&gt;()  
    /// </summary>  
    public class PlayerUnit : UnitBase  
    {  
        private Camera _mainCamera;  
          
        // 移动参数缓存，每帧复用，避免 GC  
        private XParamMove _cacheParamMove = new();  
  
        protected override void Awake()  
        {  
            base.Awake(); // → ASC 初始化 + UnitManager 注册  
  
            _mainCamera = Camera.main;  
  
            // 添加固有阵营 Tag（Faction.Player）  
            ASC.Cell.AddFixedTag(XTag.Faction_Player);  
            // 添加 Ability 根 Tag，部分 Ability 激活条件依赖此 Tag  
            ASC.Cell.AddFixedTag(XTag.Ability);  
  
            // 施加初始 Buff，例如耐力自动回复（effect id 1007）  
            ApplyEffectToSelf(1007);  
        }  
  
        // ──────────────────────────────────────────  
        // 属性钳制/监听（OnEnable/OnDisable 生命周期由 UnitBase 驱动）  
        // ──────────────────────────────────────────  
  
        protected override void RegisterAttributeCallbacks()  
        {  
            // Hp/Mp/Sp 上限钳制（不超过 Max 值）  
            GASEventCenter.SetOnAttrBaseValueChangeBefore(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp, OnHpChangeBefore);  
            GASEventCenter.SetOnAttrBaseValueChangeBefore(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Mp, OnMpChangeBefore);  
            GASEventCenter.SetOnAttrBaseValueChangeBefore(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Sp, OnSpChangeBefore);  
  
            // Sp 归零时自动停止奔跑  
            GASEventCenter.RegisterOnAttrCurrentValueChangeAfter(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Sp, OnSpChangeAfter);  
        }  
  
        protected override void UnregisterAttributeCallbacks()  
        {  
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(ASC.Cell, XAttrSet.FightUnit, XAttribute.Hp);  
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(ASC.Cell, XAttrSet.FightUnit, XAttribute.Mp);  
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(ASC.Cell, XAttrSet.FightUnit, XAttribute.Sp);  
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(  
                ASC.Cell, XAttrSet.FightUnit, XAttribute.Sp, OnSpChangeAfter);  
        }  
  
        // ── 属性钳制回调 ──  
        private float OnHpChangeBefore(float newVal)  
            => Mathf.Min(newVal, ASC.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax));  
  
        private float OnMpChangeBefore(float newVal)  
            => Mathf.Min(newVal, ASC.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.MpMax));  
  
        private float OnSpChangeBefore(float newVal)  
            => Mathf.Min(newVal, ASC.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.SpMax));  
  
        private void OnSpChangeAfter(float lastSp, float newSp)  
        {  
            if (newSp <= 0) StopRun();  
        }  
  
        // ──────────────────────────────────────────  
        // Ability 触发方法（由 PlayerController 调用）  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 每帧持续调用。使用相机方向作为视角前方，保证角色朝向正确。  
        /// 参照 DemoPlayer.Move() 实现。  
        /// </summary>  
        public override void Move(Vector3 direction)  
        {  
            if (!IsAbilityActive(XAbility.ABILITY_move))  
                TryActivateAbility(XAbility.ABILITY_move, _cacheParamMove);  
  
            var viewFwd = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;  
            _cacheParamMove.SetDirection(direction, viewFwd);  
            SetAbilityParam(XAbility.ABILITY_move, _cacheParamMove);  
        }  
  
        public override void StopMove()  
        {  
            if (IsAbilityActive(XAbility.ABILITY_move))  
                TryEndAbility(XAbility.ABILITY_move);  
        }  
  
        public void StartRun()  
        {  
            // RunSpeedUp 需要 Event.Moving (3001) Tag 才能激活（见 exgas_tbability.json）  
            if (!IsAbilityActive(XAbility.ABILITY_RunSpeedUp))  
                TryActivateAbility(XAbility.ABILITY_RunSpeedUp);  
        }  
  
        public void StopRun()  
        {  
            if (IsAbilityActive(XAbility.ABILITY_RunSpeedUp))  
                TryEndAbility(XAbility.ABILITY_RunSpeedUp);  
        }  
  
        public override void Attack()  
        {  
            // Attack 自带 ActivationBlockedTags = [Event.Attacking]，防止重复激活  
            ASC.Cell.TryActivateAbility(XAbility.ABILITY_Attack);  
        }  
  
        /// <summary>获取当前移速属性（供 PlayerController 的速度插值使用）</summary>  
        public float GetSpeed()  
            => ASC.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Spd);  
    }  
}