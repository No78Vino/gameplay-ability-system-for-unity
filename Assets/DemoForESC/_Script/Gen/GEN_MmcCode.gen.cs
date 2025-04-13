///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class GEN_MmcCode
    {
        public const string MMC_MMCNone = "GAS.RuntimeWithECS.Modifier.CommonUsage.MMCNone";
        public const string MMC_MMCScalableFloat = "GAS.RuntimeWithECS.Modifier.CommonUsage.MMCScalableFloat";

        public static void LoadMmcType()
        {
            var MMCNone = typeof(GAS.RuntimeWithECS.Modifier.CommonUsage.MMCNone);
            MmcHelper.RegisterMmc(MMC_MMCNone, MMCNone);
            var MMCScalableFloat = typeof(GAS.RuntimeWithECS.Modifier.CommonUsage.MMCScalableFloat);
            MmcHelper.RegisterMmc(MMC_MMCScalableFloat, MMCScalableFloat);
        }
    }
}
