using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComDuration : IComponentData
    {
        /// <summary>
        /// 持续时间。 -1表示无限
        /// </summary>
        public int duration;
        
        /// <summary>
        /// 计时单位
        /// GAS的所有实际计时单位只有Frame（逻辑帧）和Turn（回合）两种。【如果开发人员手动控制Turn的更新速度和帧率一致，则实际Turn和Frame效果相同】
        /// 编辑器可能显示单位秒，实际存储时会换算为Frame逻辑帧
        /// </summary>
        public TimeUnit timeUnit;
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//
        
        /// <summary>
        /// 是否激活生效中。只有Durational GameplayEffect存在激活和失活的概念
        /// </summary>
        public bool active; 
    }
    
    public sealed class ConfDuration:GameplayEffectComponentConfig
    {
        public int duration;
        public TimeUnit timeUnit;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new ComDuration
            {
                duration = duration,
                timeUnit = timeUnit,
                active = false
            });
        }
    }
}