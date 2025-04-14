using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic
{
    public class ALApplyEffect: AbilityLogicBase<AbilityParamArrayGameplayEffect>
    {
        public void InitGameplayEffects(GameplayEffectConfig[] effects)
        {
            _param.SetValue(effects);
        }
        
        public ALApplyEffect(Entity ability) : base(ability)
        {
        }

        public override void AbilityTick(GlobalTimer timer)
        {
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            var baseInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            var owner = baseInfo.Owner;
            foreach (var effect in _param.Value)
            {
                var geEntity = CreateGameplayEffectEntity(effect);
                ApplyGameplayEffectTo(geEntity, owner, owner);
            }
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            EndAbility(timer);
        }

        public override void EndAbility(GlobalTimer timer)
        {
            var ownerAsc = GetOwnerAsc();
            var geEntities = _entityManager.GetBuffer<BEGameplayEffect>(ownerAsc);
            foreach (var beEffect in geEntities)
            {
                var effect = beEffect.GameplayEffect;
                if (_entityManager.HasComponent<CCreatedByAbility>(effect))
                {
                    var createdByAbility = _entityManager.GetComponentData<CCreatedByAbility>(effect);
                    if(createdByAbility.sourceAbility==_abilityEntity)
                        RemoveGameplayEffect(effect);
                }
            }
        }
    }
}