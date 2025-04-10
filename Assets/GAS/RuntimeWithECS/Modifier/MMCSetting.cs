using System;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Modifier
{
    [Serializable]
    public class MMCSettingConfig
    {
        public int TypeCode;
        public float[] floatParams;
        public int[] intParams;
        public string[] stringParams;
        
        // TODO
        public ModMagnitudeCalculationBase CreateMmc()
        {
            // var mmc = MmcHub.CreateMmc(TypeCode);
            // mmc.InitParameters(this);
            return null;
        }
    }
}