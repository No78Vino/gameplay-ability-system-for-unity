
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
            
            
            internal readonly GGraph bar => (GGraph)U.G(Target, "bar");
            internal readonly GTextField title => (GTextField)U.G(Target, "title");
        }
        internal static class HpBar_Extensions
        {
            internal static string GetUIPackageName(this I_HpBar _) => "Demo";
            internal static string GetUIPackageItemName(this I_HpBar _) => "HpBar";
            internal static HpBar_Proxy GetUIDefine(this I_HpBar @this) => new HpBar_Proxy((GProgressBar)U.G(@this));
        }

        internal interface I_Main { }
        internal struct Main_Proxy
        {
            internal readonly GComponent Target { get; }
            internal Main_Proxy(GComponent o) => Target = o;
            
            
            internal readonly GTextField Control_Intro => (GTextField)U.G(Target, "Control Intro");
            internal readonly HpBar_Proxy Hp => new HpBar_Proxy((GProgressBar)U.G(Target, "Hp"));
            internal readonly GTextField HPText => (GTextField)U.G(Target, "HPText");
            internal readonly SkillCDBar_Proxy Fireball => new SkillCDBar_Proxy((GProgressBar)U.G(Target, "Fireball"));
            internal readonly SkillCDBar_Proxy FreezeBeam => new SkillCDBar_Proxy((GProgressBar)U.G(Target, "FreezeBeam"));
            internal readonly BuffButton_Proxy HealingBuff => new BuffButton_Proxy((GButton)U.G(Target, "HealingBuff"));
        }
        internal static class Main_Extensions
        {
            internal static string GetUIPackageName(this I_Main _) => "Demo";
            internal static string GetUIPackageItemName(this I_Main _) => "Main";
            internal static Main_Proxy GetUIDefine(this I_Main @this) => new Main_Proxy((GComponent)U.G(@this));
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