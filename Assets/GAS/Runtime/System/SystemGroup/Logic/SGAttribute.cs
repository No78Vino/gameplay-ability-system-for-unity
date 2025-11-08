using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGLogic))]
    [UpdateAfter(typeof(SGEffect))]
    public partial class SGAttribute : ComponentSystemGroup
    {
    }
}