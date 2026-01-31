using Unity.Entities;

namespace GAS.Runtime
{
    public class ALApplyEffect : AbilityLogicBase<XParamEffectIDs>
    {
        public ALApplyEffect(Entity ability) : base(ability)
        {
        }

        public override void AbilityTick(GlobalTimer timer)
        {
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            var owner = Owner;
            foreach (var effectCode in _param.IDs)
            {
                var effectCfg = GameplayEffectHelper.GetConfigByID(effectCode);
                var geEntity = CreateGameplayEffectEntity(effectCfg);
                ApplyGameplayEffectTo(geEntity, owner, owner);
            }
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            EndAbility(timer);
        }

        public override void EndAbility(GlobalTimer timer)
        {
            var ownerAsc = GetOwnerAscEntity();
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