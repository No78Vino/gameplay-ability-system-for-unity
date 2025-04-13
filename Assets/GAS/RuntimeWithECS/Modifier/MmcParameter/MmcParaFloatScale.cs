using System;
using GAS.RuntimeWithECS.Modifier.CommonUsage;

namespace GAS.RuntimeWithECS.Modifier.MmcParameter
{
    [Serializable]
    public class MmcParaFloatScale:IMmcParameter
    {
        public float k;
        public float b;
        
        public MmcParaFloatScale(float k, float b)
        {
            this.k = k;
            this.b = b;
        }
    }
}