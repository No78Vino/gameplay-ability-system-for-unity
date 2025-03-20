using System;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityBaseInfo : IComponentData
    {
        /// <summary>
        ///  能力代码，用于标识能力，查找Ability的对应自定义类
        /// </summary>
        public int Code;
        
        
        //////////////////////// 以下为运行时变量不用配置load ////////////////////////
        
        /// <summary>
        /// 等级
        /// </summary>
        public int Level;
        
        /// <summary>
        ///  拥有者ASC
        /// </summary>
        public Entity Owner;
    }
    
    public sealed class ConfAbilityBaseInfo:GameplayAbilityComponentConfig
    {
        public int Code;
        public int Level;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityBaseInfo
            {
                Code = Code,
                Level = Level
            });
        }
    }
}