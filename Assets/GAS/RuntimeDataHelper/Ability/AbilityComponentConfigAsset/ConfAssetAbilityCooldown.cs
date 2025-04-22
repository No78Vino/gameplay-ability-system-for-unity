using System;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityCooldown : BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("Cooldown", "【GE】冷却CD", SdfIconType.Clock, TextColor = "#667788")] [LabelText("计时方式")]
        public TimeUnit timeUnit = TimeUnit.Frame;

        [TabGroup("Cooldown", "【GE】冷却CD")]
        [LabelText("冷却时间(秒/回合)")]
        public float cooldown;


        [TabGroup("Cooldown", "【GE】冷却CD")]
        [LabelText("使用默认冷却效果")]
        [Tooltip("默认冷却效果（GE）是个空的GE，只有2个组件：\n1. 1.基础信息\n2. 计时器（Duration）")]
        public bool useDefaultCooldown = true;

        [TabGroup("Cooldown", "【GE】冷却CD")]
        [LabelText("[GE]冷却效果")]
        [AssetSelector]
        [HideIf(nameof(useDefaultCooldown))]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public GameplayEffectConfigAsset geAssetCooldown;

        [TabGroup("Cooldown", "【GE】冷却CD")]
        [ShowInInspector]
        [HideIf(nameof(IsTurnBased))]
        [LabelText("游戏当前设置运行帧率")]
        public int FrameRate => EXEditorHelper.GetFrameRate();

        [TabGroup("Cooldown", "【GE】冷却CD")]
        [ShowInInspector]
        [LabelText("冷却时间实际计数")]
        [Tooltip("不同模式的冷却时间实际计数规则:\n【回合制】: 回合数\n【即时制】: 秒数*游戏运行帧率")]
        public int CdCount => IsTurnBased() ? (int)cooldown : (int)(cooldown * FrameRate);

        public override GameplayAbilityComponentConfig GetConfig()
        {
            var config = useDefaultCooldown
                ? new GameplayEffectComponentConfig[]
                {
                    new ConfEffectBasicInfo
                    {
                        Name = "DefaultCooldown"
                    },
                    new ConfDuration
                    {
                        timeUnit = timeUnit
                    }
                }
                : geAssetCooldown.GetConfig().ComponentConfigs;
            return new ConfAbilityCooldown
            {
                Cooldown = CdCount,
                CooldownComponentConfigs = config
            };
        }


        private bool IsTurnBased()
        {
            return timeUnit == TimeUnit.Turn;
        }
    }
}