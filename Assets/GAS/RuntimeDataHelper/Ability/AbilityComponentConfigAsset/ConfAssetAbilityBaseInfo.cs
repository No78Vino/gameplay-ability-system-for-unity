using System;
using GAS.General;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityBaseInfo:BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("Base", "能力代码", TextColor = "#45B1FF", Order = 1)]
        [LabelText("能力代码")]
        public int Code;
        
        [TabGroup("Base", "等级", TextColor = "#45B1FF", Order = 2)]
        [LabelText("等级")]
        public int Level;
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityBaseInfo
            {
                Code = Code,
                Level = Level
            };
        }
    }
}