using System;
using UnityEngine.Serialization;

namespace GAS.Runtime
{
    public enum StackingType
    {
        None, //不会叠加，如果多次释放则每个Effect相当于单个Effect
        AggregateBySource,//目标(Target)上的每个源(Source)ASC都有一个单独的堆栈实例, 每个源(Source)可以应用堆栈中的X个GameplayEffect.
        AggregateByTarget,//目标(Target)上只有一个堆栈实例而不管源(Source)如何, 每个源(Source)都可以在共享堆栈限制(Shared Stack Limit)内应用堆栈.
    }

    public enum DurationRefreshPolicy
    {
        NeverRefresh, //不刷新Effect的持续时间
        RefreshOnSuccessfulApplication,  //每次apply成功后刷新Effect的持续时间
    }
    
    public enum PeriodResetPolicy
    {
        NeverRefresh, //不重置Effect的周期计时
        ResetOnSuccessfulApplication,//每次apply成功后重置Effect的周期计时
    }
    
    public enum ExpirationPolicy
    {
        ClearEntireStack,  //持续时间结束时,清楚所有层数
        RemoveSingleStackAndRefreshDuration, //持续时间结束时减少一层，然后重新经历一个Duration，一直持续到层数减为0
        RefreshDuration //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                        //可以通过调用GameplayEffectsContainer的OnStackCountChange(GameplayEffect ActiveEffect, int OldStackCount, int NewStackCount)来处理层数，
                        //可以达到Duration结束时减少两层并刷新Duration这样复杂的效果。
    }
    
    // GE堆栈数据结构
    public struct GameplayEffectStacking
    {
        public StackingType stackingType;
        public int limitCount;
        public DurationRefreshPolicy durationRefreshPolicy;
        public PeriodResetPolicy periodResetPolicy;
        public ExpirationPolicy expirationPolicy;
        
        // Overflow 溢出逻辑处理
        public GameplayEffect[] overflowEffects; // 超过StackLimitCount数量的Effect被Apply时将会调用该OverflowEffects
        public bool denyOverflowApplication; //对应于StackDurationRefreshPolicy，如果为True则多余的Apply不会刷新Duration
        public bool clearStackOnOverflow; //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
    }
}