using GAS.Runtime;

namespace DemoForESC._Script.Gen
{
    public static class GEN_GASLauncher
    {
        public static void LoadGenCache()
        {
            GEN_AbilityCode.LoadAbilityCode();
            XMmc.LoadMmcType();
            XCue.LoadCueType();
        }
          
        public static void Launch()
        {
            LoadGenCache();
            XLubanExtension.LoadTables();
            GASManager.Initialize();
            GEN_GameplayTagCode.InitTagList();
            
            // 测试代码
            var testCue = XLubanExtension.GetGameplayCueConfig(1001);
            var cueUnit = new GameplayCueUnit(testCue);
            cueUnit.Create();
            cueUnit.Play();
        }
    }
}