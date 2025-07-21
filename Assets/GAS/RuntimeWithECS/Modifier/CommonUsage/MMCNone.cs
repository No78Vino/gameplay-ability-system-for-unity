using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.MmcParameter;
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