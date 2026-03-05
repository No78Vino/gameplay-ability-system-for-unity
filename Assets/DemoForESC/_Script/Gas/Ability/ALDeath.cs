using System;  
using GAS.Runtime;  
using Unity.Entities;  
using UnityEngine;  
  
namespace DemoForESC._Script.Gas.Ability  
{  
    /// <summary>  
    /// 通用死亡AbilityLogic  
    /// 激活时：  
    ///   1. 施加配置的死亡相关GE（如移除所有Buff等）  
    ///   2. 通知外部（通过回调）单位已死亡  
    ///   3. 死亡状态持续（不会自动结束，需外部驱动复活/销毁）  
    ///   
    /// 设计思路：  
    ///   - 使用 XParamEffectIDs 参数，可配置死亡时施加的GE列表（如清除Buff的GE等）  
    ///   - 通过 Ability 的 CancelAbilityWithTags 配置取消所有Ability标签的技能  
    ///   - 通过 Ability 的 BlockAbilityWithTags 配置阻止所有Ability标签的技能  
    ///   - 通过 ActivationOwnedTags 添加死亡状态Tag  
    /// </summary>  
    public class ALDeath : AbilityLogicBase<XParamEffectIDs>  
    {  
        private Action<AbilitySystemCell> _onDeathCallback;  
        private Action<AbilitySystemCell> _onReviveCallback;  
  
        public ALDeath(Entity ability) : base(ability)  
        {  
        }  
  
        /// <summary>  
        /// 设置死亡回调（可选），由外部在初始化时注入  
        /// </summary>  
        public void SetOnDeathCallback(Action<AbilitySystemCell> onDeath)  
        {  
            _onDeathCallback = onDeath;  
        }  
  
        /// <summary>  
        /// 设置复活回调（可选），在EndAbility时触发  
        /// </summary>  
        public void SetOnReviveCallback(Action<AbilitySystemCell> onRevive)  
        {  
            _onReviveCallback = onRevive;  
        }  
  
        public override void ActivateAbility(GlobalTimer timer)  
        {  
            Debug.Log($"[ALDeath] Entity:{_abilityEntity} 单位死亡");  
  
            var owner = Owner;  
            if (owner == null) return;  
  
            // 1. 施加死亡相关的GE（如果有配置）  
            if (_param != null && _param.IDs != null)  
            {  
                foreach (var effectCode in _param.IDs)  
                {  
                    var effectCfg = GameplayEffectHelper.GetConfigByID(effectCode);  
                    if (effectCfg == null) continue;  
                    var geEntity = CreateGameplayEffectEntity(effectCfg);  
                    ApplyGameplayEffectTo(geEntity, owner, owner);  
                }  
            }  
  
            // 2. 触发死亡回调  
            _onDeathCallback?.Invoke(owner);  
        }  
  
        public override void CancelAbility(GlobalTimer timer)  
        {  
            // 死亡被取消 = 复活  
            Debug.Log($"[ALDeath] Entity:{_abilityEntity} 单位复活(Cancel)");  
            CleanupDeathEffects();  
            _onReviveCallback?.Invoke(Owner);  
        }  
  
        public override void EndAbility(GlobalTimer timer)  
        {  
            // 死亡结束 = 复活  
            Debug.Log($"[ALDeath] Entity:{_abilityEntity} 单位复活(End)");  
            CleanupDeathEffects();  
            _onReviveCallback?.Invoke(Owner);  
        }  
  
        public override void AbilityTick(GlobalTimer timer)  
        {  
            // 死亡状态下不做任何逻辑  
            // 如需要死亡倒计时自动销毁等逻辑，可在此扩展  
        }  
  
        /// <summary>  
        /// 清理由该Ability施加的所有GE  
        /// </summary>  
        private void CleanupDeathEffects()  
        {  
            var ownerAsc = GetOwnerAscEntity();  
            if (ownerAsc == Entity.Null) return;  
  
            var geEntities = _entityManager.GetBuffer<BGameplayEffect>(ownerAsc);  
            foreach (var beEffect in geEntities)  
            {  
                var effect = beEffect.GameplayEffect;  
                if (_entityManager.HasComponent<CCreatedByAbility>(effect))  
                {  
                    var createdByAbility = _entityManager.GetComponentData<CCreatedByAbility>(effect);  
                    if (createdByAbility.sourceAbility == _abilityEntity)  
                        RemoveGameplayEffect(effect);  
                }  
            }  
        }  
    }  
}