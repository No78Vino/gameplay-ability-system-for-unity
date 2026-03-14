///////////////////////////////////
//// This is a generated file. ////
////   Modify BindData() only.  ////
///////////////////////////////////

using EXUI;
using UnityEngine.UI;
using Loxodon.Framework.Binding.Builder;
using UI.ViewModel;

namespace UI.View
{
    public class BossWindow : BaseView<VMBossWindow>
    {
        [BindOneWay("boss_info/label_boss", nameof(VMBossWindow.LabelBoss), "text")]
        private Text _labelLabelBoss;

        [BindOneWay("boss_info/hp_bar/value", nameof(VMBossWindow.Value), "fillAmount")]
        private Image _imgValue;

        [BindOneWay("boss_info/hp_bar/label_value", nameof(VMBossWindow.LabelValue), "text")]
        private Text _labelLabelValue;

        [BindOneWay("boss_info/hp_bar/label_name", nameof(VMBossWindow.LabelName), "text")]
        private Text _labelLabelName;


        protected override void InitViewComponents()
        {
        }

        protected override void BindData()
        {
            // 所有绑定已由 [BindOneWay]/[BindTwoWay] Attribute 自动处理
        }
    }
}
