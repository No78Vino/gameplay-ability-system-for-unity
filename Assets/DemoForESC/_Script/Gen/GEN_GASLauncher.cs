using GAS.Runtime;

namespace DemoForESC._Script.Gen
{
    public static class GEN_GASLauncher
    {
        public static void LoadGenCache()
        {
            XAbility.LoadAbilityCode();
            XMmc.LoadMmcType();
            XCue.LoadCueType();
        }
          
        public static void Launch()
        {
            LoadGenCache();
            XLuban.LoadTables();
            GASManager.Initialize();
            XTag.InitTagList();
            
            // 测试代码
            var testCue = XLuban.GetGameplayCueConfig(1001);
            var cueUnit = new GameplayCueUnit(testCue);
            cueUnit.Create();
            cueUnit.Play();
        }
    }
}