using System;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetEffectDuration: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("Base","持续时间",SdfIconType.Clock)]
        [LabelText("持续时间(-1=永久)")]
        public float duration;
        
        [TabGroup("Base","持续时间")]
        [LabelText("计时单位")]
        public TimeUnit timeUnit;

        [TabGroup("Base", "持续时间")]
        [ShowInInspector]
        [HideIf(nameof(IsTurnBased))]
        [LabelText("游戏当前设置运行帧率")]
        public int FrameRate => EXEditorHelper.GetFrameRate();
        
        [TabGroup("Base", "持续时间")]
        [ShowInInspector]
        [LabelText("持续时间实际计数")]
        [Tooltip("不同模式的持续时间实际计数规则:\n【回合制】: 回合数\n【即时制】: 秒数*游戏运行帧率")]
        public int DurationCount => IsTurnBased() ? (int)duration : (int)(duration * FrameRate);
        
        [TabGroup("Base", "持续时间")]
        [LabelText("激活时，刷新计时起始时间")]
        public bool ResetStartTimeWhenActivated;
        
        [TabGroup("Base", "持续时间")]
        [LabelText("失活时，停止计时")]
        public bool StopTickWhenDeactivated;
        
        private bool IsTurnBased()
        {
            return timeUnit == TimeUnit.Turn;
        }
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfDuration()
            {
                duration = DurationCount,
                timeUnit = timeUnit,
                ResetStartTimeWhenActivated = ResetStartTimeWhenActivated,
                StopTickWhenDeactivated = StopTickWhenDeactivated
            };
        }
    }
}