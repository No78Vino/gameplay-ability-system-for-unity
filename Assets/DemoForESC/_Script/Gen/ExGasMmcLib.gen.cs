///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XMmc
    {
        public const string MMC_MMCNone = "MMCNone";
        public const string MMC_MMCScalableFloat = "MMCScalableFloat";

        public static void LoadMmcType()
        {
            var MMCNone = typeof(GAS.RuntimeWithECS.MMCNone);
            MmcHelper.RegisterMmc(MMC_MMCNone, MMCNone);
            var MMCScalableFloat = typeof(GAS.Runtime.MMCScalableFloat);
            MmcHelper.RegisterMmc(MMC_MMCScalableFloat, MMCScalableFloat);
        }
    }
}
