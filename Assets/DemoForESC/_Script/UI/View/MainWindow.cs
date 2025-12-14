using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class MainWindow: BaseView<VMMainWindow>
    {
        private Text _labelPlayer;
        private UIProgressBar _hpBar;
        private UIProgressBar _mpBar;
        private UIProgressBar _spBar;
        
        protected override void BindData()
        {
            var bindingDynamic = new BindingSet<MainWindow, VMMainWindow>(_bindingContext, this);
            bindingDynamic.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bindingDynamic.Bind(_labelPlayer).For(v => v.text).To(vm => vm.LabelPlayer.Value).OneWay();
            bindingDynamic.Bind(_hpBar.LabelValue).For(v => v.text).To(vm => vm.HpText.Value).OneWay();
            bindingDynamic.Bind(_mpBar.LabelValue).For(v => v.text).To(vm => vm.MpText.Value).OneWay();
            bindingDynamic.Bind(_spBar.LabelValue).For(v => v.text).To(vm => vm.SpText.Value).OneWay();
            bindingDynamic.Bind(_hpBar.ValueBar).For(v => v.fillAmount).To(vm => vm.Hp.Value).OneWay();
            bindingDynamic.Bind(_mpBar.ValueBar).For(v => v.fillAmount).To(vm => vm.Mp.Value).OneWay();
            bindingDynamic.Bind(_spBar.ValueBar).For(v => v.fillAmount).To(vm => vm.Sp.Value).OneWay();
            
            bindingDynamic.Build();
        }

        protected override void InitViewComponents()
        {
            _labelPlayer = GetComponentByNode<Text>("label_player");
            _hpBar = new UIProgressBar(transform.Node("state_info/hp_bar"));
            _mpBar = new UIProgressBar(transform.Node("state_info/mp_bar"));
            _spBar = new UIProgressBar(transform.Node("state_info/sp_bar"));
        }
    }
}