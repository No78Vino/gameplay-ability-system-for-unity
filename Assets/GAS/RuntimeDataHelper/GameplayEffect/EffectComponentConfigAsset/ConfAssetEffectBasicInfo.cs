using System;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetEffectBasicInfo: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("Base","GE效果基础信息",SdfIconType.Activity)]
        [LabelText("名称")]
        public string Name;
        
        [TabGroup("Base","GE效果基础信息")]
        [LabelText("描述")]
        public string Desc;
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfEffectBasicInfo()
            {
                Name = Name
            };
        }
    }
}