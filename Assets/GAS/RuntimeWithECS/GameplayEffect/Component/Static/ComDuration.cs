using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComDuration : IComponentData
    {
        /// <summary>
        /// 持续时间。 小于等于0 表示无限
        /// </summary>
        public int duration;
        
        /// <summary>
        /// 计时单位
        /// GAS的所有实际计时单位只有Frame（逻辑帧）和Turn（回合）两种。【如果开发人员手动控制Turn的更新速度和帧率一致，则实际Turn和Frame效果相同】
        /// 编辑器可能显示单位秒，实际存储时会换算为Frame逻辑帧
        /// </summary>
        public TimeUnit timeUnit;

        /// <summary>
        /// 是否在激活时，刷新计时起始时间
        /// </summary>
        public bool ResetStartTimeWhenActivated;
        
        /// <summary>
        /// 是否在失活时，停止计时
        /// </summary>
        public bool StopTickWhenDeactivated;
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//
        
        /// <summary>
        /// 开始计时的时间点
        /// </summary>
        public int activeTime; 
        
        /// <summary>
        /// 是否激活生效中。只有Durational GameplayEffect存在激活和失活的概念
        /// </summary>
        public bool active;

        /// <summary>
        /// StopTickWhenDeactivated=true时，该字段生效
        /// 上一次开始计时时间
        /// </summary>
        public int lastActiveTime;
        
        /// <summary>
        /// StopTickWhenDeactivated=true时，该字段生效
        /// 剩余持续时间
        /// </summary>
        public int remianTime;
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