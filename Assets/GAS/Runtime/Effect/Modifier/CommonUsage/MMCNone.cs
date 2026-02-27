using Unity.Entities;

namespace GAS.Runtime
{
    public class MMCNone : ModMagnitudeCalculationBase<XParamNone>
    {
        public override float CalculateMagnitude(Entity geEntity, float magnitude)
        {
            return magnitude;
        }
    }
}