using GAS.Runtime;

namespace DemoForESC._Script.Gen
{
    public static class GEN_GASLauncher
    {
        public static void LoadGenCache()
        {
            GEN_AbilityCode.LoadAbilityCode();
            GEN_MmcCode.LoadMmcType();
            GEN_CueCode.LoadCueType();
        }
        
        public static void Launch()
        {
            LoadGenCache();
            GASManager.Initialize();
            GEN_GameplayTagCode.InitTagList();
        }
    }
}