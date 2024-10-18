using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    /// <summary>
    /// 正在使用中的Component
    /// </summary>
    public struct ComInUsage : IComponentData,IEnableableComponent
    {
        /// <summary>
        /// 施加目标
        /// </summary>
        public Entity Target;
        
        /// <summary>
        /// 施加来源
        /// </summary>
        public Entity Source;
    }
}