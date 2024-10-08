using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public abstract class GameplayEffectComponentConfig
    {
        protected static EntityManager _entityManager => GASManager.EntityManager;
        
        /// <summary>
        /// 添加组件到GE的实例上，这个函数是生成GE的核心。
        /// 因为采用了component结构，未来拓展GE的功能模块，会变得方便很多，实现了提前解耦。
        /// </summary>
        /// <param name="ge"></param>
        public abstract void LoadToGameplayEffectEntity(Entity ge);
    }
}