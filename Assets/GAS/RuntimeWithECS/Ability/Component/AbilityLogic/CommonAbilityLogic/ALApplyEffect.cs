using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic
{
    public class ALApplyEffect: AbilityLogicBase<AbilityParamArrayGameplayEffect>
    {
        private List<Entity> _appliedGEEntities = new();
        
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
            _appliedGEEntities.Clear();
            var baseInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            var owner = baseInfo.Owner;
            foreach (var effect in _param.Value)
            {
                var geEntity = CreateGameplayEffectEntity(effect);
                GEUtil.ApplyGameplayEffectTo(geEntity, owner,owner);
                _appliedGEEntities.Add(geEntity);
            }
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            EndAbility(timer);
        }

        public override void EndAbility(GlobalTimer timer)
        {
            foreach (var geEntity in _appliedGEEntities)
                GEUtil.RemoveGameplayEffect(geEntity);
            
            _appliedGEEntities.Clear();
        }
    }
}