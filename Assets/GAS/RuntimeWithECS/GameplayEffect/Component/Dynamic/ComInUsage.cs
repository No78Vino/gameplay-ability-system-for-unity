using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    /// <summary>
    /// 正在使用中的Component
    /// </summary>
    public struct ComInUsage : IComponentData
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
        /// 是否合法可生效：检测ApplicationRequiredTags是否满足 
        /// </summary>
        public bool Valid;
    }
}