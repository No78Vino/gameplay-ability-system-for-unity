using System;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityCooldown:BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("Cooldown","【GE】冷却CD",SdfIconType.Clock, TextColor = "#667788")]
        [LabelText("是回合制计数")]
        public bool isTurnBased = false;
        
        [TabGroup("Cooldown","【GE】冷却CD")]
        [LabelText("冷却时间(秒/回合)")]
        public float cooldown = 0.0f;
        
        [TabGroup("Cooldown","【GE】冷却CD")]
        [ShowInInspector]
        [HideIf(nameof(isTurnBased))]
        [LabelText("游戏当前设置运行帧率")]
        public int FrameRate => EXEditorHelper.GetFrameRate();
        
        [TabGroup("Cooldown","【GE】冷却CD")]
        [ShowInInspector]
        [LabelText("冷却时间实际计数")]
        [Tooltip("不同模式的冷却时间实际计数规则:\n【回合制】: 回合数\n【即时制】: 秒数*游戏运行帧率")]
        public int CdCount => isTurnBased ? (int)cooldown : (int)(cooldown * FrameRate);
        
        
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityCooldown
            {
                Cooldown = CdCount
            };
        }
    }
}