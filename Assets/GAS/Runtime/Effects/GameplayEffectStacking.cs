using System;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public enum StackingType
    {
        [LabelText("独立", SdfIconType.XCircleFill)]
        None, //不会叠加，如果多次释放则每个Effect相当于单个Effect

        [LabelText("来源", SdfIconType.Magic)]
        AggregateBySource, //目标(Target)上的每个源(Source)ASC都有一个单独的堆栈实例, 每个源(Source)可以应用堆栈中的X个GameplayEffect.

        [LabelText("目标", SdfIconType.Person)]
        AggregateByTarget //目标(Target)上只有一个堆栈实例而不管源(Source)如何, 每个源(Source)都可以在共享堆栈限制(Shared Stack Limit)内应用堆栈.
    }

    public enum DurationRefreshPolicy
    {
        [LabelText("NeverRefresh - 不刷新Effect的持续时间", SdfIconType.XCircleFill)]
        NeverRefresh, //不刷新Effect的持续时间

        [LabelText(
            "RefreshOnSuccessfulApplication - 每次apply成功后刷新持续时间",
            SdfIconType.HourglassTop)]
        RefreshOnSuccessfulApplication //每次apply成功后刷新Effect的持续时间, denyOverflowApplication如果为True则多余的Apply不会刷新Duration
    }

    public enum PeriodResetPolicy
    {
        [LabelText("NeverReset - 不重置Effect的周期计时", SdfIconType.XCircleFill)]
        NeverRefresh, //不重置Effect的周期计时

        [LabelText("ResetOnSuccessfulApplication - 每次apply成功后重置Effect的周期计时", SdfIconType.HourglassTop)]
        ResetOnSuccessfulApplication //每次apply成功后重置Effect的周期计时
    }

    public enum ExpirationPolicy
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

    // GE堆栈数据结构
    public struct GameplayEffectStacking
    {
        public string stackingCodeName; // 实际允许不会使用，而是使用stackingCodeName的hash值, 即stackingHashCode
        public int stackingHashCode;
        public StackingType stackingType;
        public int limitCount;
        public DurationRefreshPolicy durationRefreshPolicy;
        public PeriodResetPolicy periodResetPolicy;
        public ExpirationPolicy expirationPolicy;

        // Overflow 溢出逻辑处理
        public bool denyOverflowApplication; //对应于StackDurationRefreshPolicy，如果为True则多余的Apply不会刷新Duration
        public bool clearStackOnOverflow; //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
        public GameplayEffect[] overflowEffects; // 超过StackLimitCount数量的Effect被Apply时将会调用该OverflowEffects

        public void SetStackingCodeName(string stackingCodeName)
        {
            this.stackingCodeName = stackingCodeName;
            this.stackingHashCode = stackingCodeName?.GetHashCode() ?? 0; // 兼容旧的SO数据
        }

        public void SetStackingHashCode(int stackingHashCode)
        {
            this.stackingHashCode = stackingHashCode;
        }

        public void SetStackingType(StackingType stackingType)
        {
            this.stackingType = stackingType;
        }

        public void SetLimitCount(int limitCount)
        {
            this.limitCount = limitCount;
        }

        public void SetDurationRefreshPolicy(DurationRefreshPolicy durationRefreshPolicy)
        {
            this.durationRefreshPolicy = durationRefreshPolicy;
        }

        public void SetPeriodResetPolicy(PeriodResetPolicy periodResetPolicy)
        {
            this.periodResetPolicy = periodResetPolicy;
        }

        public void SetExpirationPolicy(ExpirationPolicy expirationPolicy)
        {
            this.expirationPolicy = expirationPolicy;
        }

        public void SetOverflowEffects(GameplayEffect[] overflowEffects)
        {
            this.overflowEffects = overflowEffects;
        }

        public void SetOverflowEffects(GameplayEffectAsset[] overflowEffectAssets)
        {
            overflowEffects = new GameplayEffect[overflowEffectAssets.Length];
            for (var i = 0; i < overflowEffectAssets.Length; ++i)
            {
                overflowEffects[i] = new GameplayEffect(overflowEffectAssets[i]);
            }
        }

        public void SetDenyOverflowApplication(bool denyOverflowApplication)
        {
            this.denyOverflowApplication = denyOverflowApplication;
        }

        public void SetClearStackOnOverflow(bool clearStackOnOverflow)
        {
            this.clearStackOnOverflow = clearStackOnOverflow;
        }

        public static GameplayEffectStacking None
        {
            get
            {
                var stack = new GameplayEffectStacking();
                stack.SetStackingType(StackingType.None);
                return stack;
            }
        }
    }

    [Serializable]
    public sealed class GameplayEffectStackingConfig
    {
        private const int LABEL_WIDTH = 100;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_TYPE)]
        [EnumToggleButtons]
        public StackingType stackingType;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [HideIf("IsNoStacking")]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_CODENAME)]
        [InlineButton(@"@stackingCodeName = """"", SdfIconType.EraserFill, "")]
        public string stackingCodeName;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_COUNT)]
        [HideIf("IsNoStacking")]
        [InlineButton(@"@limitCount = int.MaxValue", SdfIconType.Hammer, "max")]
        [InlineButton(@"@limitCount = 0", SdfIconType.Hammer, "min")]
        [ValidateInput("@limitCount >= 0", "必须>=0")]
        public int limitCount;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_DURATION_REFRESH_POLICY)]
        [HideIf("IsNoStacking")]
        [InfoBox(GASTextDefine.LABEL_GE_STACKING_DENY_OVERFLOW_APPLICATION+"为True时多余的Apply不会刷新Duration", InfoMessageType.None,
            VisibleIf =
                "@durationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication && denyOverflowApplication")]
        public DurationRefreshPolicy durationRefreshPolicy;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_PERIOD_RESET_POLICY)]
        [HideIf("IsNoStacking")]
        public PeriodResetPolicy periodResetPolicy;

        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_EXPIRATION_POLICY)]
        [HideIf("IsNoStacking")]
        public ExpirationPolicy expirationPolicy;

        // Overflow 溢出逻辑处理
        [LabelWidth(LABEL_WIDTH)]
        [VerticalGroup]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_DENY_OVERFLOW_APPLICATION)]
        [HideIf("@IsNoStacking() || IsNeverRefreshDuration()")]
        public bool denyOverflowApplication;

        [VerticalGroup]
        [LabelWidth(LABEL_WIDTH)]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_CLEAR_STACK_ON_OVERFLOW)]
        [ShowIf("IsDenyOverflowApplication")]
        public bool clearStackOnOverflow;

        [VerticalGroup]
        [LabelWidth(LABEL_WIDTH)]
        [LabelText(GASTextDefine.LABEL_GE_STACKING_CLEAR_OVERFLOW_EFFECTS)]
        [HideIf("IsNoStacking")]
        public GameplayEffectAsset[] overflowEffects;

        /// <summary>
        /// 转换为运行时数据
        /// </summary>
        /// <returns></returns>
        public GameplayEffectStacking ToRuntimeData()
        {
            var stack = new GameplayEffectStacking();
            stack.SetStackingCodeName(stackingCodeName);
            stack.SetStackingType(stackingType);
            stack.SetLimitCount(limitCount);
            stack.SetDurationRefreshPolicy(durationRefreshPolicy);
            stack.SetPeriodResetPolicy(periodResetPolicy);
            stack.SetExpirationPolicy(expirationPolicy);
            stack.SetOverflowEffects(overflowEffects);
            stack.SetDenyOverflowApplication(denyOverflowApplication);
            stack.SetClearStackOnOverflow(clearStackOnOverflow);
            return stack;
        }

        #region UTIL FUNCTION FOR ODIN INSPECTOR

        public bool IsNoStacking() => stackingType == StackingType.None;

        public bool IsNeverRefreshDuration() =>
            IsNoStacking() || durationRefreshPolicy == DurationRefreshPolicy.NeverRefresh;

        public bool IsDenyOverflowApplication() => !IsNoStacking() && denyOverflowApplication;

        #endregion
    }
}