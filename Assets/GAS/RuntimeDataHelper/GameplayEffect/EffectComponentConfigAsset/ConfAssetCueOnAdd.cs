using System;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetCueOnAdd: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("Base","效果添加时Cue",SdfIconType.Clock)]
        [LabelText("持续时间(-1=永久)")]
        public float duration;
        
        // TODO
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfCueOnAdd();
        }
    }
}