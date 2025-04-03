using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.Modifier;

namespace DemoForESC._Script.Gen
{
    public static class GEN_GASLauncher
    {
        public static void LoadGenCache()
        {
            MmcHub.Init();
            GEN_AbilityCode.LoadAbilityCode();
        }
        
        public static void Launch()
        {
            LoadGenCache();
            GASManager.Initialize();
            GTagList.InitTagList();
        }
    }
}