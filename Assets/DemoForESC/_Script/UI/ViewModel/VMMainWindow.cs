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

        private AbilitySystemCellMono PlayerAsc
        {
            get
            {
                var player = DemoPlayer.Player();
                var asc = player.AbilitySystemCellMono;
                return asc;
            }
        }
        public override void OnShow()
        {
            base.OnShow();
            LabelPlayer.Value = "[玩家]蜘蛛机器人";
            RefreshState();

            RegisterUpdateEvent();
        }

        public override void OnHide()
        {
            base.OnHide();
            UnregisterUpdateEvent();
        }

        private void RefreshState()
        {
            var asc = PlayerAsc;
            var hp = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Hp);
            var hpMax = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            HpText.Value = $"{hp}/{hpMax}";
            Hp.Value = hp / hpMax;
            
            var mp = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Mp);
            var mpMax = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.MpMax);
            MpText.Value = $"{mp}/{mpMax}";
            Mp.Value = mp / mpMax;
            
            var sp = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Sp);
            var spMax = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.SpMax);
            SpText.Value = $"{sp}/{spMax}";
            Sp.Value = sp / spMax;
        }

        private void RegisterUpdateEvent()
        {
            var asc = PlayerAsc;
            GASEventCenter.RegisterOnAttrCurrentValueChangeAfter(asc.Cell,XAttrSet.FightUnit,XAttribute.Hp,OnHpChange);
        }
        
        private void UnregisterUpdateEvent()
        {
            var asc = PlayerAsc;
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(asc.Cell,XAttrSet.FightUnit,XAttribute.Hp,OnHpChange);
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(asc.Cell,XAttrSet.FightUnit,XAttribute.Mp,OnMpChange);
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(asc.Cell,XAttrSet.FightUnit,XAttribute.Sp,OnSpChange);
        }

        private void OnHpChange(float lastValue, float newValue)
        {
            var hpMax = PlayerAsc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            HpText.Value = $"{newValue}/{hpMax}";
            Hp.Value = newValue / hpMax;   
        }
        
        private void OnMpChange(float lastValue, float newValue)
        {
            var mpMax = PlayerAsc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.MpMax);
            MpText.Value = $"{newValue}/{mpMax}";
            Mp.Value = newValue / mpMax;   
        }
        
        private void OnSpChange(float lastValue, float newValue)
        {
            var spMax = PlayerAsc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.SpMax);
            SpText.Value = $"{newValue}/{spMax}";
            Sp.Value = newValue / spMax;   
        }
    }
}