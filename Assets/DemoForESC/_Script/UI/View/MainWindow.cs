using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class MainWindow: BaseView<VMMainWindow>
    {
        private Text _labelPlayer;
        private Text _labelHp;
        private Text _labelMp;
        private Text _labelSp;
        private Image _imgHp;
        private Image _imgMp;
        private Image _imgSp;
        
        protected override void BindData()
        {
            var bindingDynamic = new BindingSet<MainWindow, VMMainWindow>(_bindingContext, this);
            bindingDynamic.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bindingDynamic.Bind(_labelPlayer).For(v => v.text).To(vm => vm.LabelPlayer.Value).OneWay();
            // bindingDynamic.Bind(btnStartText).For(v => v.text).To(vm => vm.btnStartText.Value).OneWay();
            // bindingDynamic.Bind(btnStart).For(v => v.onClick).To(vm => vm.GameStart);
            bindingDynamic.Build();
        }

        protected override void InitViewComponents()
        {
            _labelPlayer = GetComponentByNode<Text>("label_player");
            _labelHp = GetComponentByNode<Text>("state_info/hp_bar/label_value");
            _labelMp = GetComponentByNode<Text>("state_info/mp_bar/label_value");
            _labelSp = GetComponentByNode<Text>("state_info/sp_bar/label_value");
            _imgHp = GetComponentByNode<Image>("state_info/hp_bar/value");
            _imgMp = GetComponentByNode<Image>("state_info/mp_bar/value");
            _imgSp = GetComponentByNode<Image>("state_info/sp_bar/value");
        }
    }
}