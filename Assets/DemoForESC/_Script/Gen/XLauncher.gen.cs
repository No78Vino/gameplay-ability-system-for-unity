///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;

namespace GAS.Runtime
{
    public static class XLauncher
    {
        public static void InitCache()
        {
            XAbility.LoadAbilityCode();
            XMmc.LoadMmcType();
            XCue.LoadCueType();
        }

        public static void InitConfigTables(Func<string, SimpleJSON.JSONNode> loader)
        {
            XLuban.Init(loader);
        }
        public static void Launch()
        {
            InitCache();
            GASManager.Initialize();

            // 初始化Tag系统
            // 注意需要在GASManager.Initialize()之后调用
            // 因为XTag创建全Tag的图鉴单例来作为运行时缓存，需要EntityManager。
            XTag.InitTagList();
        }
    }
}
