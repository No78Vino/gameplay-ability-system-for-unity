using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// 不同于Mono版本的EX-GAS中的AbilitySpec，ECS版AbilitySpec的作用是把ECS相关的概念封闭
    /// 将ECS框架下的Ability包装成OOP的形式，方便使用者理解
    /// </summary>
    public class AbilitySpec
    {
        private Entity _abilityEntity;
        private Entity _ascEntity;
        
        protected static EntityManager _entityManager => GASManager.EntityManager;
        
        public Entity AbilityEntity => _abilityEntity;
        
        public AbilitySpec(Entity abilityEntity)
        {
            _abilityEntity = abilityEntity;
            _ascEntity = GetAscEntity();
            
        }
        
        private Entity GetAscEntity()
        {
            if (!_entityManager.Exists(_abilityEntity)) return Entity.Null;
            
            var basicInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            return basicInfo.Owner;
        }
        
        public AbilitySystemCell Owner
        {
            get
            {
                if (_ascEntity == Entity.Null) return null;
                var asc = GASManager.GetAscFromEntity(_ascEntity);
                return asc;
            }
        }
    }
}