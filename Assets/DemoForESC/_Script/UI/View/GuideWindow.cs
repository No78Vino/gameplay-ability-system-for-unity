using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;

namespace DemoForESC._Script.UI.View
{
    public class GuideWindow: BaseView<VMGuideWindow>
    {
        protected override void BindData()
        {
            var bindingDynamic = new BindingSet<GuideWindow, VMGuideWindow>(_bindingContext, this);
            bindingDynamic.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);

            bindingDynamic.Build();
        }

        protected override void InitViewComponents()
        {
        }
    }
}