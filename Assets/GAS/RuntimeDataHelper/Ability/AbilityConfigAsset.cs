using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : GEN_AbilityConfigSO
    {
        //[ShowIf(nameof(IsShowErrorTip))]
        [PropertyOrder(-2)]
        [ShowInInspector]
        [ValidateInput(nameof(ValidateConfig), ContinuousValidationCheck = true)]
        [DisplayAsString(TextAlignment.Left)]
        [TitleGroup("能力配置",HorizontalLine = true)]
        [HideLabel]
        public string __ => "";
        
        public AbilityConfig GetConfig()
        {
            var configs = new List<GameplayAbilityComponentConfig>();
            foreach (var cfgType in configTypes)
            {
                var cfg = GetConfigAsset(cfgType);
                if (cfg != null) configs.Add(cfg.GetConfig());
            }

            return new AbilityConfig(configs.ToArray());
        }
        
        private bool ValidateConfig(string _, ref string errorMsg)
        { 
            return ShowErrorTip(out errorMsg);
        }
        
        bool ShowErrorTip(out string errorMsg)
        {
            var messages = new List<string>();
            if (!HasConfAssetAbilityBaseInfo)
                messages.Add("组件列表必须包含ConfAssetBasicInfo组件！");
            if(!HasMCConfAssetAbilityLogic)
                messages.Add("组件列表必须包含MCConfAssetAbilityLogic组件！");

            errorMsg = messages.Count > 0 ? string.Join("\n", messages) : null;
            return messages.Count == 0;
        }
        
        bool IsShowErrorTip()
        {
            return !ShowErrorTip(out string _);
        }
    }
}