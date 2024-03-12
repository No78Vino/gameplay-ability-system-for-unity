using FairyGUI.Extension;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UIGen.Demo;

namespace Demo.Script.UI
{
    public class MenuWindow:AbstractFGUIWindow,I_Menu
    {
        public MenuWindow()
        {
            CreateContentPane(new MenuWindowVM(),this.GetUIPackageName(), this.GetUIPackageItemName(), true);
            var ui = this.GetUIDefine();

            var bindingSet = new BindingSet<MenuWindow, MenuWindowVM>(bindingContext, this);

            bindingSet.Bind(this).For(v => v.Msg_Common).To(vm => vm.commonRequest);
            bindingSet.Bind(this).For(v => v.Msg_Transition).To(vm => vm.transitionRequest);

            bindingSet.Bind(ui.BtnQuit.Target).For(v => v.onClick).To(vm => vm.Quit);
            bindingSet.Bind(ui.BtnStart.Target).For(v => v.onClick).To(vm => vm.StartGame);
            bindingSet.Bind(ui.BtnGithub.Target).For(v => v.onClick).To(vm => vm.VisitExgasGithub);
            
            bindingSet.Build();
        }

        protected override void Msg_Common(object sender, InteractionEventArgs args)
        {
            base.Msg_Common(sender, args);
            var msg = args.Context.ToString();
    
            if (msg == "close")
                DoHideAnimation();
        }
    
        protected override void Msg_Transition(object sender, InteractionEventArgs args)
        {
            base.Msg_Common(sender, args);
        }

        protected override void DoShowAnimation()
        {
            base.DoShowAnimation();
            var transition= contentPane.GetTransition("open");
            transition.Play();
        }

        protected override void DoHideAnimation()
        {
            var transition= contentPane.GetTransition("close");
            transition.Play(base.DoHideAnimation);
        }
    }
}