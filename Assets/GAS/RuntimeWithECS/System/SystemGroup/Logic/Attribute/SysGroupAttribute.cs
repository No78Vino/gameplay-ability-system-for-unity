using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupLogic))]
    public partial class SysGroupAttribute : ComponentSystemGroup
    {
    }
}