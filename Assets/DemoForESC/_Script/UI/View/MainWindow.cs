using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class MainWindow : BaseView<VMMainWindow>  
    {  
        [BindOneWay("label_player", nameof(VMMainWindow.LabelPlayer), nameof(Text.text))]  
        private Text _labelPlayer;  
  
        [BindOneWay("state_info/hp_bar/label_value", nameof(VMMainWindow.HpText), nameof(Text.text))]  
        private Text _hpBarLabel;  
  
        [BindOneWay("state_info/hp_bar/value", nameof(VMMainWindow.Hp), nameof(Image.fillAmount))]  
        private Image _hpBarValue;  
  
        [BindOneWay("state_info/mp_bar/label_value", nameof(VMMainWindow.MpText), nameof(Text.text))]  
        private Text _mpBarLabel;  
  
        [BindOneWay("state_info/mp_bar/value", nameof(VMMainWindow.Mp), nameof(Image.fillAmount))]  
        private Image _mpBarValue;  
  
        [BindOneWay("state_info/sp_bar/label_value", nameof(VMMainWindow.SpText), nameof(Text.text))]  
        private Text _spBarLabel;  
  
        [BindOneWay("state_info/sp_bar/value", nameof(VMMainWindow.Sp), nameof(Image.fillAmount))]  
        private Image _spBarValue;

        [BindOneWay("skill_info/skill/value", nameof(VMMainWindow.DodgeCd), nameof(Image.fillAmount))]  
        private Image _dodgeCdImage;
        
        [BindOneWay("skill_info/skill/label_name", nameof(VMMainWindow.DodgeName), nameof(Text.text))]  
        private Text _dodgeName;
        
        [BindOneWay("skill_info/skill/label_cd", nameof(VMMainWindow.DodgeCdText), nameof(Text.text))]  
        private Text _dodgeCdText;
  
        protected override void BindData()  
        {  
            var bs = new BindingSet<MainWindow, VMMainWindow>(_bindingContext, this);  
            bs.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);  
            bs.Build();  
        }  
    }
}