///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XMmc
    {
        public const string MMC_MMCAttributeBased = "MMCAttributeBased";
        public const string MMC_MMCNone = "MMCNone";
        public const string MMC_MMCScalableFloat = "MMCScalableFloat";

        public static void LoadMmcType()
        {
            var MMCAttributeBased = typeof(GAS.Runtime.MMCAttributeBased);
            MmcHelper.RegisterMmc(MMC_MMCAttributeBased, MMCAttributeBased,typeof(GAS.Runtime.AttributeBasedMmcParam));
            var MMCNone = typeof(GAS.Runtime.MMCNone);
            MmcHelper.RegisterMmc(MMC_MMCNone, MMCNone,typeof(GAS.Runtime.XParamNone));
            var MMCScalableFloat = typeof(GAS.Runtime.MMCScalableFloat);
            MmcHelper.RegisterMmc(MMC_MMCScalableFloat, MMCScalableFloat,typeof(GAS.Runtime.MmcParaFloatScale));
        }
    }
}
