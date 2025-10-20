using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum StackingType
    {
        [LabelText("来源", SdfIconType.Magic)]
        AggregateBySource, //目标(Target)上的每个源(Source)ASC都有一个单独的堆栈实例, 每个源(Source)可以应用堆栈中的X个GameplayEffect.

        [LabelText("目标", SdfIconType.Person)]
        AggregateByTarget //目标(Target)上只有一个堆栈实例而不管源(Source)如何, 每个源(Source)都可以在共享堆栈限制(Shared Stack Limit)内应用堆栈.
    }
}