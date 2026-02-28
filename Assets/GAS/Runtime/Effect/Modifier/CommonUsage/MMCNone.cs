using Unity.Entities;

namespace GAS.Runtime
{
    public class MMCNone : ModMagnitudeCalculationBase<XParamNone>
    {
        public override float CalculateMagnitude(MmcContext context, float magnitude)
        {
            return magnitude;
        }
    }
}