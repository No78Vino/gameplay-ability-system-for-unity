using FairyGUI.Extension;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UIGen.Demo;

namespace Demo.Script.UI
{
    public class MainUI:AbstractFGUIWindow,I_Main
    {
        public MainUI()
        {
            CreateContentPane(new MainUIVM(),this.GetUIPackageName(), this.GetUIPackageItemName(), true);
            var ui = this.GetUIDefine();

            var bindingSet = new BindingSet<MainUI, MainUIVM>(bindingContext, this);

            bindingSet.Bind(this).For(v => v.Msg_Common).To(vm => vm.commonRequest);
            bindingSet.Bind(this).For(v => v.Msg_Transition).To(vm => vm.transitionRequest);

            bindingSet.Bind(ui.Hp.Target).For(v => v.value).To(vm => vm.playerHp.Value);
            bindingSet.Bind(ui.Hp.Target).For(v => v.max).To(vm => vm.playerHpMax.Value);
            
            bindingSet.Bind(ui.HealingBuff.Target).For(v => v.selected).To(vm => vm.healingBuff.Value);
            
            bindingSet.Bind(ui.Fireball.Target).For(v => v.value).To(vm => vm.fireballCd.Value);
            bindingSet.Bind(ui.Fireball.Target).For(v => v.max).To(vm => vm.fireballCdMax.Value);
            bindingSet.Bind(ui.Fireball.skillName).For(v => v.text).To(vm => vm.fireballSkillName.Value);
            bindingSet.Bind(ui.Fireball.cd).For(v => v.text).To(vm => vm.fireballCdCount.Value);
            
            bindingSet.Bind(ui.FreezeBeam.Target).For(v => v.value).To(vm => vm.freezebeamCd.Value);
            bindingSet.Bind(ui.FreezeBeam.Target).For(v => v.max).To(vm => vm.freezebeamCdMax.Value);
            bindingSet.Bind(ui.FreezeBeam.skillName).For(v => v.text).To(vm => vm.freezebeamSkillName.Value);
            bindingSet.Bind(ui.FreezeBeam.cd).For(v => v.text).To(vm => vm.freezebeamCdCount.Value);
            
            bindingSet.Build();
        }

        protected override void Msg_Common(object sender, InteractionEventArgs args)
        {
            base.Msg_Common(sender, args);
            var msg = args.Context.ToString();
    
            if (msg == "close")
                Hide();
        }
    
        protected override void Msg_Transition(object sender, InteractionEventArgs args)
        {
            base.Msg_Common(sender, args);
            var msg = args.Context.ToString();
        }
    }
}