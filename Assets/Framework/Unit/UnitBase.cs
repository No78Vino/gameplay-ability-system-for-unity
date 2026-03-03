using Framework.Core;  
using GAS.Runtime;  
using UnityEngine;  
  
namespace Framework.Unit  
{  
    /// <summary>  
    /// 所有游戏单位的基类。  
    /// - 持有并初始化 AbilitySystemComponent（ASC 预设由 Inspector 配置）  
    /// - Awake 时自动向 UnitManager 注册，OnDestroy 时注销并广播 UnitDeadEvent  
    /// - OnEnable/OnDisable 中管理属性钳制回调，子类重写 RegisterAttributeCallbacks/UnregisterAttributeCallbacks  
    /// </summary>  
    [RequireComponent(typeof(AbilitySystemComponent))]  
    public abstract class UnitBase : MonoBehaviour  
    {  
        /// <summary>ASC 预设 ID，对应 exgas_tbasc.json 中的配置，在 Inspector 中设置</summary>  
        [SerializeField] protected int _ascPresetId = 0;  
  
        /// <summary>该单位的 AbilitySystemComponent（MonoBehaviour 包装）</summary>  
        public AbilitySystemComponent ASC { get; private set; }  
  
        protected virtual void Awake()  
        {  
            // 1. 获取或添加 AbilitySystemComponent  
            ASC = GetComponent<AbilitySystemComponent>();  
  
            // 2. 用 Luban 预设初始化 ASC（加载 Tags、AttrSets、Abilities、Level）  
            ASC.Init(XLuban.GetAscConfig(_ascPresetId));  
  
            // 3. 向 UnitManager 注册自身  
            UnitManager.Instance.Register(this);  
        }  
  
        protected virtual void OnEnable()  
        {  
            RegisterAttributeCallbacks();  
        }  
  
        protected virtual void OnDisable()  
        {  
            UnregisterAttributeCallbacks();  
        }  
  
        protected virtual void OnDestroy()  
        {  
            // 注销并广播死亡事件  
            UnitManager.Instance.Unregister(this);  
            GameEventBus.Dispatch(new UnitDeadEvent { Unit = gameObject });  
        }  
  
        // ──────────────────────────────────────────  
        // 属性回调（子类重写以注册钳制/监听）  
        // ──────────────────────────────────────────  
  
        /// <summary>  
        /// 在 OnEnable 时调用，子类重写以注册 GASEventCenter 属性回调。  
        /// 约定：SetOnAttrBaseValueChangeBefore（钳制）+ RegisterOnAttrCurrentValueChangeAfter（监听）  
        /// 必须与 UnregisterAttributeCallbacks 成对。  
        /// </summary>  
        protected virtual void RegisterAttributeCallbacks() { }  
  
        /// <summary>  
        /// 在 OnDisable 时调用，子类重写以注销 GASEventCenter 属性回调。  
        /// </summary>  
        protected virtual void UnregisterAttributeCallbacks() { }  
  
        // ──────────────────────────────────────────  
        // GAS 快捷接口（供子类使用，避免重复写 ASC.Cell.XXX）  
        // ──────────────────────────────────────────  
  
        /// <summary>尝试激活技能</summary>  
        public void TryActivateAbility(int abilityId, XParam param = null)  
            => ASC.Cell.TryActivateAbility(abilityId, param);  
  
        /// <summary>尝试结束技能</summary>  
        public void TryEndAbility(int abilityId)  
            => ASC.Cell.TryEndAbility(abilityId);  
  
        /// <summary>判断技能是否激活中</summary>  
        public bool IsAbilityActive(int abilityId)  
            => ASC.Cell.IsAbilityActive(abilityId);  
  
        /// <summary>设置技能参数（每帧更新方向等）</summary>  
        public void SetAbilityParam(int abilityId, XParam param)  
            => ASC.Cell.SetAbilityParam(abilityId, param);  
  
        /// <summary>对自身施加 GameplayEffect（通过 ID 构造 Spec）</summary>  
        public void ApplyEffectToSelf(int effectId)  
        {  
            var spec = new GameplayEffectSpec(XLuban.GetGameplayEffectConfig(effectId).ComponentConfigs);  
            ASC.Cell.ApplyGameplayEffectToSelf(spec);  
        }  
  
        /// <summary>查询 ASC 是否持有指定 Tag</summary>  
        public bool HasTag(int tagId) => ASC.Cell.HasTag(tagId);  
  
        /// <summary>获取指定属性当前值</summary>  
        public float GetAttrCurrentValue(int attrSetCode, int attrCode)  
            => ASC.GetAttrCurrentValue(attrSetCode, attrCode);  
  
        // ──────────────────────────────────────────  
        // 通用行为虚方法（子类按需重写）  
        // ──────────────────────────────────────────  
  
        public virtual void Move(Vector3 direction) { }  
        public virtual void StopMove() { }  
        public virtual void Attack() { }  
    }  
}