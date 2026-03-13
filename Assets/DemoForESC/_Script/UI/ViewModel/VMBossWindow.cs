///////////////////////////////////
//// This is a generated file. ////
////   Modify BindData() only.  ////
///////////////////////////////////

using DemoForESC._Script;
using EXUI;
using GAS.Runtime;
using Loxodon.Framework.Extension;

namespace UI.ViewModel
{
    public class VMBossWindow : ViewModelCommon
    {
        public ObservableVariable<string> LabelBoss { get; } = new();
        public ObservableVariable<float> Value { get; } = new();
        public ObservableVariable<string> LabelValue { get; } = new();
        public ObservableVariable<string> LabelName { get; } = new();

        private BaseUnit _unit;
        
        public override void OnShow()
        {
            base.OnShow();
            LabelBoss.Value = "【训练假人】";
        }

        public override void OnHide()
        {
            base.OnHide();
            UnregisterUpdateEvent();
        }

        private void RefreshState()
        {
            var asc = _unit.AbilitySystemComponent;
            var hp = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.Hp);
            var hpMax = asc.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            LabelValue.Value = $"{hp}/{hpMax}";
            Value.Value = hp / hpMax;
        }
        
        public void BindTargetHp(BaseUnit unit)
        {
            _unit = unit;
            RegisterUpdateEvent();
            RefreshState();
        }
        
        private void RegisterUpdateEvent()
        {
            var asc = _unit.AbilitySystemComponent;
            GASEventCenter.RegisterOnAttrCurrentValueChangeAfter(asc.Cell, XAttrSet.FightUnit, XAttribute.Hp,
                OnHpChange);
        }

        private void UnregisterUpdateEvent()
        {
            var asc = _unit.AbilitySystemComponent;
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(asc.Cell, XAttrSet.FightUnit, XAttribute.Hp,
                OnHpChange);
        }

        private void OnHpChange(float lastValue, float newValue)
        {
            var hpMax = _unit.AbilitySystemComponent.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            LabelValue.Value = $"{newValue}/{hpMax}";
            Value.Value = newValue / hpMax;
        }
    }
}
