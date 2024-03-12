
using FairyGUI;
namespace UIGen
{
    internal static class U
    {
        internal static GComponent G(object o) => o is Window w ? w.contentPane : (GComponent)o;
        internal static GObject G(object o, string name) => G(o).GetChild(name);
    }

    namespace Demo
    {
        
        internal interface I_BuffButton { }
        internal struct BuffButton_Proxy
        {
            internal readonly GButton Target { get; }
            internal BuffButton_Proxy(GButton o) => Target = o;
            
            
        }
        internal static class BuffButton_Extensions
        {
            internal static string GetUIPackageName(this I_BuffButton _) => "Demo";
            internal static string GetUIPackageItemName(this I_BuffButton _) => "BuffButton";
            internal static BuffButton_Proxy GetUIDefine(this I_BuffButton @this) => new BuffButton_Proxy((GButton)U.G(@this));
        }

        internal interface I_ButtonA { }
        internal struct ButtonA_Proxy
        {
            internal readonly GButton Target { get; }
            internal ButtonA_Proxy(GButton o) => Target = o;
            
            
        }
        internal static class ButtonA_Extensions
        {
            internal static string GetUIPackageName(this I_ButtonA _) => "Demo";
            internal static string GetUIPackageItemName(this I_ButtonA _) => "ButtonA";
            internal static ButtonA_Proxy GetUIDefine(this I_ButtonA @this) => new ButtonA_Proxy((GButton)U.G(@this));
        }

        internal interface I_ControllerKey { }
        internal struct ControllerKey_Proxy
        {
            internal readonly GComponent Target { get; }
            internal ControllerKey_Proxy(GComponent o) => Target = o;
            
            
            internal readonly GTextField action => (GTextField)U.G(Target, "action");
            internal readonly GTextField key => (GTextField)U.G(Target, "key");
        }
        internal static class ControllerKey_Extensions
        {
            internal static string GetUIPackageName(this I_ControllerKey _) => "Demo";
            internal static string GetUIPackageItemName(this I_ControllerKey _) => "ControllerKey";
            internal static ControllerKey_Proxy GetUIDefine(this I_ControllerKey @this) => new ControllerKey_Proxy((GComponent)U.G(@this));
        }

        internal interface I_EnemyHp { }
        internal struct EnemyHp_Proxy
        {
            internal readonly GComponent Target { get; }
            internal EnemyHp_Proxy(GComponent o) => Target = o;
            
            
            internal readonly GTextField name => (GTextField)U.G(Target, "name");
            internal readonly HpBar_Proxy hp => new HpBar_Proxy((GProgressBar)U.G(Target, "hp"));
        }
        internal static class EnemyHp_Extensions
        {
            internal static string GetUIPackageName(this I_EnemyHp _) => "Demo";
            internal static string GetUIPackageItemName(this I_EnemyHp _) => "EnemyHp";
            internal static EnemyHp_Proxy GetUIDefine(this I_EnemyHp @this) => new EnemyHp_Proxy((GComponent)U.G(@this));
        }

        internal interface I_HpBar { }
        internal struct HpBar_Proxy
        {
            internal readonly GProgressBar Target { get; }
            internal HpBar_Proxy(GProgressBar o) => Target = o;
            
            internal readonly Controller Controller_type => U.G(Target).GetController("type");
            
            internal readonly GGraph bar => (GGraph)U.G(Target, "bar");
            internal readonly GTextField title => (GTextField)U.G(Target, "title");
            internal readonly GTextField AttrText => (GTextField)U.G(Target, "AttrText");
        }
        internal static class HpBar_Extensions
        {
            internal static string GetUIPackageName(this I_HpBar _) => "Demo";
            internal static string GetUIPackageItemName(this I_HpBar _) => "HpBar";
            internal static HpBar_Proxy GetUIDefine(this I_HpBar @this) => new HpBar_Proxy((GProgressBar)U.G(@this));
        }
        internal static class HpBar_Pages
        {
            
            internal static readonly string type_hp = "hp";
            internal static readonly string type_mp = "mp";
            internal static readonly string type_stamina = "stamina";
            internal static readonly string type_posture = "posture";
        }

        internal interface I_Main { }
        internal struct Main_Proxy
        {
            internal readonly GComponent Target { get; }
            internal Main_Proxy(GComponent o) => Target = o;
            
            
            internal readonly GList buffList => (GList)U.G(Target, "buffList");
            internal readonly GTextField BossName => (GTextField)U.G(Target, "BossName");
            internal readonly HpBar_Proxy BossHp => new HpBar_Proxy((GProgressBar)U.G(Target, "BossHp"));
            internal readonly HpBar_Proxy BossPosture => new HpBar_Proxy((GProgressBar)U.G(Target, "BossPosture"));
            internal readonly GGroup Boss => (GGroup)U.G(Target, "Boss");
            internal readonly HpBar_Proxy Hp => new HpBar_Proxy((GProgressBar)U.G(Target, "Hp"));
            internal readonly HpBar_Proxy Mp => new HpBar_Proxy((GProgressBar)U.G(Target, "Mp"));
            internal readonly HpBar_Proxy Stamina => new HpBar_Proxy((GProgressBar)U.G(Target, "Stamina"));
            internal readonly HpBar_Proxy Posture => new HpBar_Proxy((GProgressBar)U.G(Target, "Posture"));
            internal readonly GGroup Player => (GGroup)U.G(Target, "Player");
        }
        internal static class Main_Extensions
        {
            internal static string GetUIPackageName(this I_Main _) => "Demo";
            internal static string GetUIPackageItemName(this I_Main _) => "Main";
            internal static Main_Proxy GetUIDefine(this I_Main @this) => new Main_Proxy((GComponent)U.G(@this));
        }

        internal interface I_Menu { }
        internal struct Menu_Proxy
        {
            internal readonly GComponent Target { get; }
            internal Menu_Proxy(GComponent o) => Target = o;
            
            
            internal readonly ButtonA_Proxy BtnStart => new ButtonA_Proxy((GButton)U.G(Target, "BtnStart"));
            internal readonly ButtonA_Proxy BtnQuit => new ButtonA_Proxy((GButton)U.G(Target, "BtnQuit"));
            internal readonly ButtonA_Proxy BtnGithub => new ButtonA_Proxy((GButton)U.G(Target, "BtnGithub"));
        }
        internal static class Menu_Extensions
        {
            internal static string GetUIPackageName(this I_Menu _) => "Demo";
            internal static string GetUIPackageItemName(this I_Menu _) => "Menu";
            internal static Menu_Proxy GetUIDefine(this I_Menu @this) => new Menu_Proxy((GComponent)U.G(@this));
        }

        internal interface I_RetryWindow { }
        internal struct RetryWindow_Proxy
        {
            internal readonly GComponent Target { get; }
            internal RetryWindow_Proxy(GComponent o) => Target = o;
            
            internal readonly Controller Controller_windowState => U.G(Target).GetController("windowState");
            
            internal readonly ButtonA_Proxy btnReturn => new ButtonA_Proxy((GButton)U.G(Target, "btnReturn"));
            internal readonly ButtonA_Proxy btnRetry => new ButtonA_Proxy((GButton)U.G(Target, "btnRetry"));
        }
        internal static class RetryWindow_Extensions
        {
            internal static string GetUIPackageName(this I_RetryWindow _) => "Demo";
            internal static string GetUIPackageItemName(this I_RetryWindow _) => "RetryWindow";
            internal static RetryWindow_Proxy GetUIDefine(this I_RetryWindow @this) => new RetryWindow_Proxy((GComponent)U.G(@this));
        }
        internal static class RetryWindow_Pages
        {
            
            internal static readonly string windowState_win = "win";
            internal static readonly string windowState_lose = "lose";
        }

        internal interface I_Setting { }
        internal struct Setting_Proxy
        {
            internal readonly GComponent Target { get; }
            internal Setting_Proxy(GComponent o) => Target = o;
            
            
            internal readonly GList intro => (GList)U.G(Target, "intro");
            internal readonly ButtonA_Proxy btnReturn => new ButtonA_Proxy((GButton)U.G(Target, "btnReturn"));
        }
        internal static class Setting_Extensions
        {
            internal static string GetUIPackageName(this I_Setting _) => "Demo";
            internal static string GetUIPackageItemName(this I_Setting _) => "Setting";
            internal static Setting_Proxy GetUIDefine(this I_Setting @this) => new Setting_Proxy((GComponent)U.G(@this));
        }

        internal interface I_SkillCDBar { }
        internal struct SkillCDBar_Proxy
        {
            internal readonly GProgressBar Target { get; }
            internal SkillCDBar_Proxy(GProgressBar o) => Target = o;
            
            
            internal readonly GGraph bar => (GGraph)U.G(Target, "bar");
            internal readonly GTextField cd => (GTextField)U.G(Target, "cd");
            internal readonly GTextField skillName => (GTextField)U.G(Target, "skillName");
        }
        internal static class SkillCDBar_Extensions
        {
            internal static string GetUIPackageName(this I_SkillCDBar _) => "Demo";
            internal static string GetUIPackageItemName(this I_SkillCDBar _) => "SkillCDBar";
            internal static SkillCDBar_Proxy GetUIDefine(this I_SkillCDBar @this) => new SkillCDBar_Proxy((GProgressBar)U.G(@this));
        }

    }

}