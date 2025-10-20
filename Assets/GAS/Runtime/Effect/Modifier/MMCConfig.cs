using System;

namespace GAS.Runtime
{
    public class MMCConfig
    {
        public Type MmcType;
        public IMmcParameter MmcParameter;
        
        public ModMagnitudeCalculationBase CreateMmc()
        {
            return MmcHelper.TryCreateMmc(MmcType, MmcParameter);
        }
    }
}