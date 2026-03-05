using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class GuideWindow: BaseView<VMGuideWindow>
    {
        public override UILayer Layer => UILayer.Modal; 
        
        private Text _labelTitle;
        private Text _labelContent;
        
        protected override void BindData()
        {
            var bindingDynamic = new BindingSet<GuideWindow, VMGuideWindow>(_bindingContext, this);
            bindingDynamic.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bindingDynamic.Bind(_labelTitle).For(v => v.text).To(vm => vm.Title.Value).OneWay();
            bindingDynamic.Bind(_labelContent).For(v => v.text).To(vm => vm.Content.Value).OneWay();

            bindingDynamic.Build();
        }

        protected override void InitViewComponents()
        {
            _labelTitle = GetComponentByNode<Text>("guide/label_guide");
            _labelContent = GetComponentByNode<Text>("guide/label_content");
        }
    }
}