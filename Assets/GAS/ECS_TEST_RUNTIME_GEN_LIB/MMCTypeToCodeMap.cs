using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Modifier.CommonUsage;

namespace GAS.ECS_TEST_RUNTIME_GEN_LIB
{
    public static class MMCTypeToCode
    {
        public static Dictionary<Type, int> Map = new()
        {
            { typeof(MMCScalableFloat), 1 }
        };
    }
}