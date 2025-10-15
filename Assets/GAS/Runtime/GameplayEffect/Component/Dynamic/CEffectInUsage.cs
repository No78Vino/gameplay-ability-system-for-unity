using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// 正在使用中的Component
    /// </summary>
    public struct CEffectInUsage : IComponentData
    {
        /// <summary>
        /// 施加目标
        /// </summary>
        public Entity Target;
        
        /// <summary>
        /// 施加来源
        /// </summary>
        public Entity Source;

        /// <summary>
        /// GE 等级
        /// </summary>
        public int Level;
    }
}