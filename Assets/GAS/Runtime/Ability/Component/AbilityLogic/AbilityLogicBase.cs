using Unity.Entities;

namespace GAS.Runtime
{
    public abstract class AbilityLogicBase
    {
        protected static EntityManager _entityManager => GASManager.EntityManager;
        protected IAbilityParam _paramRaw;
        protected Entity _abilityEntity;
        protected int _code;

        public AbilitySpec Spec
        {
            get
            {
                var owner = GetOwnerAsc();
                return owner.GetAbilitySpec(_code);
            }
        }
        
        protected AbilityLogicBase(Entity ability)
        {
            _abilityEntity = ability;
            var basicInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            _code = basicInfo.Code;
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

        public Entity GetOwnerAscEntity()
        {
            if (_entityManager.HasComponent<CAbilityBaseInfo>(_abilityEntity))
            {
                var basicInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
                return basicInfo.Owner;
            }
            return Entity.Null;
        }
        
        public AbilitySystemCell GetOwnerAsc()
        {
            var owner = GetAscEntity();
            if (owner == Entity.Null) return null;
            var asc = GASManager.GetAscFromEntity(owner);
            return asc;
        }
        
        public virtual void SetParam(IAbilityParam abilityParam)
        {
            _paramRaw = abilityParam;
        }
        
        protected Entity CreateGameplayEffectEntity(GameplayEffectConfig config)
        {
            return EffectUtil.CreateGameplayEffectEntity(config.ComponentConfigs);
        } 
        
        protected void ApplyGameplayEffectTo(Entity gameplayEffect, Entity target, Entity source)
        {
            EffectUtil.ApplyGameplayEffectTo(gameplayEffect, target,source);
            EntityHelper.AddComponent<CCreatedByAbility>(gameplayEffect);
            EntityHelper.SetComponent(gameplayEffect,new CCreatedByAbility()
            {
                sourceAbility = _abilityEntity
            });
        }
        
        protected void ApplyGameplayEffectTo(Entity gameplayEffect, AbilitySystemCell target, AbilitySystemCell source)
        {
            ApplyGameplayEffectTo(gameplayEffect, target.Entity, source.Entity);
        }

        protected void RemoveGameplayEffect(Entity geEntity)
        {
            EffectUtil.RemoveGameplayEffect(geEntity);
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