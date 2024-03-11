using FairyGUI.Extension;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UIGen.Demo;

namespace Demo.Script.UI
{
    public class RetryWindow:AbstractFGUIWindow,I_RetryWindow
    {
        public RetryWindow()
        {
            CreateContentPane(new RetryWindowVM(),this.GetUIPackageName(), this.GetUIPackageItemName(), true);
            var ui = this.GetUIDefine();

            var bindingSet = new BindingSet<RetryWindow, RetryWindowVM>(bindingContext, this);

            bindingSet.Bind(this).For(v => v.Msg_Common).To(vm => vm.commonRequest);
            bindingSet.Bind(this).For(v => v.Msg_Transition).To(vm => vm.transitionRequest);

            bindingSet.Bind(ui.Controller_windowState).For(v => v.selectedPage).To(vm => vm.WindowState.Value);
            bindingSet.Bind(ui.btnReturn.Target).For(v => v.onClick).To(vm => vm.ReturnMenu);
            bindingSet.Bind(ui.btnRetry.Target).For(v => v.onClick).To(vm => vm.Retry);
            
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
        }
    }
}