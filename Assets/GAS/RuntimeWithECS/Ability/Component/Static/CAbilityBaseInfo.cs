using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityBaseInfo : IComponentData
    {
        /// <summary>
        ///  能力代码，用于标识能力，查找Ability的对应自定义类
        /// </summary>
        public int Code;
        
        /// <summary>
        /// 等级
        /// </summary>
        public int Level;
        
        /// <summary>
        /// 激活次数
        /// </summary>
        public int ActivationCount;
        
        /// <summary>
        ///  拥有者ASC
        /// </summary>
        public Entity Owner;
    }
}