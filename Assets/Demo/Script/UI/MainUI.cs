using FairyGUI;
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
            CreateContentPane(new MainUIVM(), this.GetUIPackageName(), this.GetUIPackageItemName(), true);
            var ui = this.GetUIDefine();

            var bindingSet = new BindingSet<MainUI, MainUIVM>(bindingContext, this);

            bindingSet.Bind(this).For(v => v.Msg_Common).To(vm => vm.commonRequest);
            bindingSet.Bind(this).For(v => v.Msg_Transition).To(vm => vm.transitionRequest);

            bindingSet.Bind(ui.Hp.Target).For(v => v.value).To(vm => vm.playerHp.Value);
            bindingSet.Bind(ui.Hp.Target).For(v => v.max).To(vm => vm.playerHpMax.Value);

            bindingSet.Bind(ui.Mp.Target).For(v => v.value).To(vm => vm.playerMp.Value);
            bindingSet.Bind(ui.Mp.Target).For(v => v.max).To(vm => vm.playerMpMax.Value);

            bindingSet.Bind(ui.Stamina.Target).For(v => v.value).To(vm => vm.playerStamina.Value);
            bindingSet.Bind(ui.Stamina.Target).For(v => v.max).To(vm => vm.playerStaminaMax.Value);

            bindingSet.Bind(ui.Posture.Target).For(v => v.value).To(vm => vm.playerPosture.Value);
            bindingSet.Bind(ui.Posture.Target).For(v => v.max).To(vm => vm.playerPostureMax.Value);

            bindingSet.Bind(contentPane.GetChild("Boss") as GGroup).For(v => v.visible)
                .To(vm => vm.BossUiVisible.Value);
            bindingSet.Bind(ui.BossName).For(v => v.text).To(vm => vm.BossName.Value);
            bindingSet.Bind(ui.BossHp.Target).For(v => v.value).To(vm => vm.BossHp.Value);
            bindingSet.Bind(ui.BossHp.Target).For(v => v.max).To(vm => vm.BossHpMax.Value);
            bindingSet.Bind(ui.BossPosture.Target).For(v => v.value).To(vm => vm.BossPosture.Value);
            bindingSet.Bind(ui.BossPosture.Target).For(v => v.max).To(vm => vm.BossPostureMax.Value);

            #region skill and buff

            bindingSet.Bind(ui.FireBulletCD.skillName).For(v => v.text).To(vm => vm.FireBulletName.Value);
            bindingSet.Bind(ui.FireBulletCD.Target).For(v => v.visible).To(vm => vm.FireBulletCDVisible.Value);
            bindingSet.Bind(ui.FireBulletCD.Target).For(v => v.value).To(vm => vm.FireBulletCD.Value);
            bindingSet.Bind(ui.FireBulletCD.Target).For(v => v.max).To(vm => vm.FireBulletCDMax.Value);
            bindingSet.Bind(ui.FireBulletCD.cd).For(v => v.text).To(vm => vm.FireBulletCD.Value);

            bindingSet.Bind(ui.DodgeCD.skillName).For(v => v.text).To(vm => vm.DodgeName.Value);
            bindingSet.Bind(ui.DodgeCD.Target).For(v => v.visible).To(vm => vm.DodgeCDVisible.Value);
            bindingSet.Bind(ui.DodgeCD.Target).For(v => v.value).To(vm => vm.DodgeCD.Value);
            bindingSet.Bind(ui.DodgeCD.Target).For(v => v.max).To(vm => vm.DodgeCDMax.Value);
            bindingSet.Bind(ui.DodgeCD.cd).For(v => v.text).To(vm => vm.DodgeCD.Value);

            bindingSet.Bind(ui.StunStateTimer.skillName).For(v => v.text).To(vm => vm.PlayerBuffName.Value);
            bindingSet.Bind(ui.StunStateTimer.Target).For(v => v.visible).To(vm => vm.PlayerBuffVisible.Value);
            bindingSet.Bind(ui.StunStateTimer.Target).For(v => v.value).To(vm => vm.PlayerBuffTime.Value);
            bindingSet.Bind(ui.StunStateTimer.Target).For(v => v.max).To(vm => vm.PlayerBuffTimeMax.Value);
            bindingSet.Bind(ui.StunStateTimer.cd).For(v => v.text).To(vm => vm.PlayerBuffTime.Value);
            
            bindingSet.Bind(ui.BossStunTimer.skillName).For(v => v.text).To(vm => vm.BossBuffName.Value);
            bindingSet.Bind(ui.BossStunTimer.Target).For(v => v.visible).To(vm => vm.BossBuffVisible.Value);
            bindingSet.Bind(ui.BossStunTimer.Target).For(v => v.value).To(vm => vm.BossBuffTime.Value);
            bindingSet.Bind(ui.BossStunTimer.Target).For(v => v.max).To(vm => vm.BossBuffTimeMax.Value);
            bindingSet.Bind(ui.BossStunTimer.cd).For(v => v.text).To(vm => vm.BossBuffTime.Value);

            #endregion

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
            if (msg =="shakeSmall")
            {
                var t = contentPane.GetTransition("shakeSmall");
                t.Play();
            }
            else if (msg == "shakeBig")
            {
                var t = contentPane.GetTransition("shakeBig");
                t.Play();
            }
        }
    }
}