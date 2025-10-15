using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum StackingExpirationPolicy
    {
        [LabelText("清除所有层数", SdfIconType.TrashFill)]
        ClearEntireStack, //持续时间结束时,清除所有层数

        [LabelText("减少一层，然后重新经历一个Duration", SdfIconType.EraserFill)]
        RemoveSingleStackAndRefreshDuration, //持续时间结束时减少一层，然后重新经历一个Duration，一直持续到层数减为0

        [LabelText("再次刷新Duration[无限Duration]", SdfIconType.HourglassTop)]
        RefreshDuration //持续时间结束时,再次刷新Duration，这相当于无限Duration，
        //TODO :可以通过调用GameplayEffectsContainer的OnStackCountChange(GameplayEffect ActiveEffect, int OldStackCount, int NewStackCount)来处理层数，
        //TODO :可以达到Duration结束时减少两层并刷新Duration这样复杂的效果。
    }
}