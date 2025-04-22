using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component
{
    public abstract class AbilityLogicBase
    {
        protected EntityCommandBuffer _ecb;
        protected static EntityManager _entityManager => GASManager.EntityManager;
        protected AbilityParamBase _paramRaw;
        protected Entity _abilityEntity;
        
        protected AbilityLogicBase(Entity ability)
        {
            _abilityEntity = ability;
        }
        
        public abstract void ActivateAbility(GlobalTimer timer);

        public abstract void CancelAbility(GlobalTimer timer);

        public abstract void EndAbility(GlobalTimer timer);
        
        public abstract void AbilityTick(GlobalTimer timer);
        
        public void SetAbilityEntity(Entity abilityEntity)
        {
            _abilityEntity = abilityEntity;
        }

        public Entity GetOwnerAsc()
        {
            if (_entityManager.HasComponent<CAbilityBaseInfo>(_abilityEntity))
            {
                var basicInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
                return basicInfo.Owner;
            }
            return Entity.Null;
        }
        
        public virtual void SetParam(AbilityParamBase abilityParam)
        {
            _paramRaw = abilityParam;
        }
        
        public void UpdateEntityCommandBuffer(EntityCommandBuffer ecb)
        {
            _ecb = ecb;
        }
        
        public void RemoveEntityCommandBuffer()
        {
            _ecb = default;
        }
        
        protected Entity CreateGameplayEffectEntity(GameplayEffectConfig config)
        {
            return GEUtil.CreateGameplayEffectEntity(config.ComponentConfigs,_ecb);
        } 
        
        protected void ApplyGameplayEffectTo(Entity gameplayEffect, Entity target, Entity source)
        {
            GEUtil.ApplyGameplayEffectTo(gameplayEffect, target,source,_ecb);
            _ecb.AddComponent(gameplayEffect,new CCreatedByAbility()
            {
                sourceAbility = _abilityEntity
            });
        }

        protected void RemoveGameplayEffect(Entity geEntity)
        {
            GEUtil.RemoveGameplayEffect(geEntity,_ecb);
        }
    }

    public abstract class AbilityLogicBase<T>:AbilityLogicBase where T:AbilityParamBase
    {
        protected T _param;
        
        protected AbilityLogicBase(Entity ability) : base(ability)
        {
        }

        public override void SetParam(AbilityParamBase abilityParam)
        {
            base.SetParam(abilityParam);
            SetParam((T)abilityParam);
        }
        
        public void SetParam(T abilityParam)
        {
            _param = abilityParam;
        }
    }
}