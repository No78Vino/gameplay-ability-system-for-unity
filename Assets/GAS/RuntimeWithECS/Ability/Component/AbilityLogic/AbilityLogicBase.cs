using GAS.RuntimeWithECS.Dynamic;
using GAS.RuntimeWithECS.Static;
using Unity.Entities;

namespace GAS.Runtime
{
    public abstract class AbilityLogicBase
    {
        protected static EntityManager _entityManager => GASManager.EntityManager;
        protected IAbilityParam _paramRaw;
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
        
        public Entity GetAbilityEntity()
        {
            return _abilityEntity;
        }
        
        public Entity GetAscEntity()
        {
            if (!_entityManager.Exists(_abilityEntity)) return Entity.Null;
            
            var basicInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            return basicInfo.Owner;
        }
        
        public virtual void TryEndSelf()
        {
            if (GASManager.EntityManager.Exists(_abilityEntity))
            {
                GASManager.EntityManager.AddComponent<CAbilityInTryEnd>(_abilityEntity);
            }
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
        
        public virtual void SetParam(IAbilityParam abilityParam)
        {
            _paramRaw = abilityParam;
        }
        
        protected Entity CreateGameplayEffectEntity(GameplayEffectConfig config)
        {
            return GEUtil.CreateGameplayEffectEntity(config.ComponentConfigs);
        } 
        
        protected void ApplyGameplayEffectTo(Entity gameplayEffect, Entity target, Entity source)
        {
            GEUtil.ApplyGameplayEffectTo(gameplayEffect, target,source);
            EntityHelper.AddComponent<CCreatedByAbility>(gameplayEffect);
            EntityHelper.SetComponent(gameplayEffect,new CCreatedByAbility()
            {
                sourceAbility = _abilityEntity
            });
        }

        protected void RemoveGameplayEffect(Entity geEntity)
        {
            GEUtil.RemoveGameplayEffect(geEntity);
        }
    }

    public abstract class AbilityLogicBase<T>:AbilityLogicBase where T:IAbilityParam
    {
        protected T _param;
        
        protected AbilityLogicBase(Entity ability) : base(ability)
        {
        }

        public override void SetParam(IAbilityParam abilityParam)
        {
            base.SetParam(abilityParam);
            SetParam((T)abilityParam);
        }
        
        public void SetParam(T abilityParam)
        {
            _param = abilityParam;
        }
        
        public T GetParam()
        {
            return _param;
        }
    }
}