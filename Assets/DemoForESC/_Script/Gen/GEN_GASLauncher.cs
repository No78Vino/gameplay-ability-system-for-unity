using GAS.Runtime;

namespace DemoForESC._Script.Gen
{
    public static class GEN_GASLauncher
    {
        public static void LoadGenCache()
        {
            GEN_AbilityCode.LoadAbilityCode();
            GEN_MmcCode.LoadMmcType();
        }
        
        public static void Launch()
        {
            LoadGenCache();
            GASManager.Initialize();
            GEN_GameplayTagCode.InitTagList();
        }
    }
}