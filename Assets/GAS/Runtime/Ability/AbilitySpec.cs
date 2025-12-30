using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// 不同于Mono版本的EX-GAS中的AbilitySpec，ECS版AbilitySpec的作用是把ECS相关的概念封闭
    /// 将ECS框架下的Ability包装成OOP的形式，方便使用者理解
    /// </summary>
    public class AbilitySpec
    {
        private Entity _entityAbility;

        public AbilitySpec(Entity entityAbility)
        {
            _entityAbility = entityAbility;
        }
    }
}