using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGLogic))]
    [UpdateAfter(typeof(SGlobalTimer))]
    public partial class SGAbility : ComponentSystemGroup
    {
    }
}