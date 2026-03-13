using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public enum EffectStackType
    {
        [LabelText("来源", SdfIconType.Magic)]
        AggregateBySource, //目标(Target)上的每个源(Source)ASC都有一个单独的堆栈实例, 每个源(Source)可以应用堆栈中的X个GameplayEffect.

        [LabelText("目标", SdfIconType.Person)]
        AggregateByTarget //目标(Target)上只有一个堆栈实例而不管源(Source)如何, 每个源(Source)都可以在共享堆栈限制(Shared Stack Limit)内应用堆栈.
    }

    public enum EffectDurationRefreshPolicy
    {
        [LabelText("NeverRefresh - 不刷新Effect的持续时间", SdfIconType.XCircleFill)]
        NeverRefresh, //不刷新Effect的持续时间

        [LabelText(
            "RefreshOnSuccessfulApplication - 每次apply成功后刷新持续时间",
            SdfIconType.HourglassTop)]
        RefreshOnSuccessfulApplication //每次apply成功后刷新Effect的持续时间, denyOverflowApplication如果为True则多余的Apply不会刷新Duration
    }

    public enum EffectPeriodResetPolicy
    {
        [LabelText("NeverReset - 不重置Effect的周期计时", SdfIconType.XCircleFill)]
        NeverRefresh, //不重置Effect的周期计时

        [LabelText("ResetOnSuccessfulApplication - 每次apply成功后重置Effect的周期计时", SdfIconType.HourglassTop)]
        ResetOnSuccessfulApplication //每次apply成功后重置Effect的周期计时
    }

    public enum EffectExpirationPolicy
    {
        [LabelText("ClearEntireStack - 持续时间结束时, 清除所有层数", SdfIconType.TrashFill)]
        ClearEntireStack, //持续时间结束时,清除所有层数

        [LabelText("RemoveSingleStackAndRefreshDuration - 持续时间结束时减少一层，然后重新经历一个Duration", SdfIconType.EraserFill)]
        RemoveSingleStackAndRefreshDuration, //持续时间结束时减少一层，然后重新经历一个Duration，一直持续到层数减为0

        [LabelText("RefreshDuration - 持续时间结束时,再次刷新Duration", SdfIconType.HourglassTop)]
        RefreshDuration //持续时间结束时,再次刷新Duration，这相当于无限Duration，
        //TODO :可以通过调用GameplayEffectsContainer的OnStackCountChange(GameplayEffect ActiveEffect, int OldStackCount, int NewStackCount)来处理层数，
        //TODO :可以达到Duration结束时减少两层并刷新Duration这样复杂的效果。
    }

    public struct CStacking : IComponentData
    {
        public EffectStackType StackType;
        public int StackingCode;
        public int LimitCount;

        public EffectDurationRefreshPolicy EffectDurationRefreshPolicy;
        public EffectPeriodResetPolicy EffectPeriodResetPolicy;
        public EffectExpirationPolicy EffectExpirationPolicy;

        // Overflow 溢出逻辑处理
        public bool denyOverflowApplication; //对应于StackDurationRefreshPolicy，如果为True则多余的Apply不会刷新Duration
        public bool clearStackOnOverflow; //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
        public NativeArray<Entity> overflowEffects; // 超过StackLimitCount数量的Effect被Apply时将会调用该Over
        
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//
        public int StackCount;
    }
    
    public sealed class ConfStacking:GameplayEffectComponentConfig
    {
        public EffectStackType StackType;
        public int StackingCode;
        public int LimitCount;

        public EffectDurationRefreshPolicy EffectDurationRefreshPolicy;
        public EffectPeriodResetPolicy EffectPeriodResetPolicy;
        public EffectExpirationPolicy EffectExpirationPolicy;

        public bool denyOverflowApplication;
        public bool clearStackOnOverflow;
        public GameplayEffectConfig[] overflowEffects;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var overflowEntities = new NativeArray<Entity>(overflowEffects.Length, Allocator.Persistent);
            for (var i = 0; i < overflowEffects.Length; i++)
            {
                var comConfig = overflowEffects[i];
                overflowEntities[i] = GameplayEffectHelper.CreateGameplayEffectEntity(comConfig.ComponentConfigs);
            }
            
            EntityHelper.AddComponent<CStacking>(ge);
            EntityHelper.SetComponent(ge, new CStacking
            {
                StackType = StackType,
                StackingCode = StackingCode,
                LimitCount = LimitCount,
                EffectDurationRefreshPolicy = EffectDurationRefreshPolicy,
                EffectPeriodResetPolicy = EffectPeriodResetPolicy,
                EffectExpirationPolicy = EffectExpirationPolicy,
                denyOverflowApplication = denyOverflowApplication,
                clearStackOnOverflow = clearStackOnOverflow,
                overflowEffects = overflowEntities,
            });
        }
    }
}