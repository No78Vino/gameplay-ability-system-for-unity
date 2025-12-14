using GAS.Runtime;
using Loxodon.Framework.Extension;
using UnityEngine;

namespace DemoForESC._Script.UI.ViewModel
{
    public class VMMainWindow : ViewModelCommon
    {
        public ObservableVariable<string> LabelPlayer = new();
        public ObservableVariable<string> HpText = new();
        public ObservableVariable<string> MpText = new();
        public ObservableVariable<string> SpText = new();
        public ObservableVariable<float> Hp = new();
        public ObservableVariable<float> Mp = new();
        public ObservableVariable<float> Sp = new();
        
        public override void OnShow()
        {
            base.OnShow();
            LabelPlayer.Value = "[玩家]蜘蛛机器人";
            RefreshState();
        }

        private void RefreshState()
        {
            var player = DemoPlayer.Player();
            var asc = player.AbilitySystemCellMono;
            var hp = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Hp);
            var hpMax = 8000f; //asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            HpText.Value = $"{hp}/{hpMax}";
            Hp.Value = hp / hpMax;
        }
    }
}