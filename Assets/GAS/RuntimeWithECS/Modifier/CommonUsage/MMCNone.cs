using GAS.RuntimeWithECS.Modifier;
using GAS.Runtime;
using Unity.Entities;

namespace GAS.RuntimeWithECS
{
    public class MMCNone : ModMagnitudeCalculationBase<MmcParamNone>
    {
        public override float CalculateMagnitude(Entity geEntity, float magnitude)
        {
            return magnitude;
        }
    }
}