using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component
{
    public abstract class AbilityLogicBase
    {
        protected AbilityParamBase _param;
        protected Entity _abilityEntity;
        
        protected AbilityLogicBase(Entity ability)
        {
            _abilityEntity = ability;
        }
        
        public abstract void ActivateAbility();

        public abstract void CancelAbility();

        public abstract void EndAbility();
        
        public abstract void AbilityTick();
        
        public virtual void SetParam(AbilityParamBase abilityParam)
        {
            _param = abilityParam;
        }
        
        public void SetAbilityEntity(Entity abilityEntity)
        {
            _abilityEntity = abilityEntity;
        }
    }
}