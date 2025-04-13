using GAS.RuntimeWithECS.Modifier.MmcParameter;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Modifier.CommonUsage
{
    public class MMCNone : ModMagnitudeCalculationBase<MmcParamNone>
    {
        public override float CalculateMagnitude(Entity geEntity, float magnitude)
        {
            return magnitude;
        }
    }
}