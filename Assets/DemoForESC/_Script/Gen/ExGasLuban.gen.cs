///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
using cfg;
using SimpleJSON;
using System.IO;
using UnityEngine;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Static;

namespace GAS.Runtime
{
    public static class EXLuban
    {
        public const string GAME_CONF_DIR = "Assets/DemoForESC/Resources/Tables";
        private static Tables _tables;
        public static Tables Tables
        {
            get
            {
                if (_tables == null) LoadTables();
                return _tables;
            }
        }

        public static void LoadTables()
        {
            if (_tables != null) return;
            _tables = new Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
        }

        public static AbilitySystemCellConfig GetAscConfig(int id)
        {
            var data = Tables.Tbasc.Get(id);
            if (data == null)
            {
                Debug.LogError($"ASC_ID:{id}  不存在.");
                return new AbilitySystemCellConfig(Array.Empty<int>(), Array.Empty<int>(),Array.Empty<AbilityConfig>(), 0);
            }
            var abilityIds = data.Ability;
            var abilities = new AbilityConfig[abilityIds.Length];
            for (var i = 0; i < abilityIds.Length; i++)
            {
                var abilityId = abilityIds[i];
                //abilities[i] = GetAbilityConfig(abilityId);
            }
            return new AbilitySystemCellConfig(data.Tag, data.AttrSet, abilities, data.Level);
        }

        public static GameplayCueConfig GetGameplayCueConfig(int id)
        {
            var data = Tables.TbgameplayCue.Get(id);
            if (data == null)
            {
                Debug.LogError($"Cue_ID:{id}  不存在.");
                return null;
            }
            var cueType = CueHelper.GetCueType(data.CueLogic.GetType().Name);
            if (cueType == null)
            {
                Debug.LogError($"Cue_ID:{id}  CueType:{data.CueLogic.GetType().Name} 不存在.");
                return null;
            }
            var cueLogic = data.CueLogic;
            var cueLogicName = cueLogic.GetType().Name;
            var cueParamType = CueHelper.GetCueLogicParamType(cueLogicName);
            var cueParam = Activator.CreateInstance(cueParamType) as ICueParameter;
            if (cueParam != null)
            {
                if (cueLogic is cfg.GameplayCueLog gameplayCueLog)
                {
                    var gameplayCueLogParam = cueParam as GAS.Runtime.CueParamString;
                    gameplayCueLogParam?.SetValue(gameplayCueLog.Value);
                    cueParam = gameplayCueLogParam;
                }
            }
            return new GameplayCueConfig(cueType, cueParam, data.RequiredTag.ToArray(), data.ImmunityTag.ToArray());
        }

        public static GameplayEffectConfig GetGameplayEffectConfig(int id)
        {
            var data = Tables.TbgameplayEffect.Get(id);
            if (data == null)
            {
                Debug.LogError($"GameplayEffect_ID:{id}  不存在.");
                return null;
            }

            var configs = new List<GameplayEffectComponentConfig>();

            // assetTags
            if (data.AssetTags is { Count: > 0 })
                configs.Add(new ConfAssetTags { tags = data.AssetTags.ToArray() });
            // grantedTags
            if (data.GrantedTags is { Count: > 0 })
                configs.Add(new ConfEffectGrantedTags { tags = data.GrantedTags.ToArray() });
            // applicationRequiredTags
            if (data.ApplicationRequiredTags is { Count: > 0 })
                configs.Add(new ConfApplicationRequiredTags { tags = data.ApplicationRequiredTags.ToArray() });
            // ongoingRequiredTags
            if (data.OngoingRequiredTags is { Count: > 0 })
                configs.Add(new ConfOngoingRequiredTags { tags = data.OngoingRequiredTags.ToArray() });
            // removeGameplayEffectsWithTags
            if (data.RemoveGameplayEffectsWithTags is { Count: > 0 })
                configs.Add(new ConfRemoveEffectWithTags { tags = data.RemoveGameplayEffectsWithTags.ToArray() });
            // immunityTags
            if (data.ImmunityTags is { Count: > 0 })
                configs.Add(new ConfEffectImmunityTags { tags = data.ImmunityTags.ToArray() });
            // duration
            if (data.Duration != null && data.Duration.Time != 0)
                configs.Add(new ConfDuration
                {
                    duration = data.Duration.Time,
                    timeUnit = (TimeUnit)data.Duration.TimeUnit,
                    ResetStartTimeWhenActivated = data.Duration.ResetStartTimeWhenActivated
                });
            // period
            if (data.Period is { Time: > 0 })
            {
                var gameplayEffectSettings = new List<GameplayEffectComponentConfig[]>();
                foreach (var effectID in data.Period.Effects)
                {
                    var effect = GetGameplayEffectConfig(effectID);
                    gameplayEffectSettings.Add(effect.ComponentConfigs);
                }
                configs.Add(new ConfPeriod
                {
                    Period = data.Period.Time,
                    ResetTimeCountWhenDeactivated = data.Period.FirstTrigger,
                    GameplayEffectSettings = gameplayEffectSettings.ToArray()
                });
            }
            // modifiers
            if (data.Modifiers != null && data.Modifiers.Count > 0)
            {
                ModifierSetting[] modifierSettings = new ModifierSetting[data.Modifiers.Count];
                for (var i = 0; i < data.Modifiers.Count; i++)
                {
                    var info = data.Modifiers[i];
                    modifierSettings[i] = new ModifierSetting()
                    {
                        AttrSetCode = info.AttrSet,
                        AttrCode = info.Attribute,
                        Magnitude = info.Magnitude,
                        Operation = (GEOperation)info.Operation,
                        //MMC = GetMmcConfig(info.Mmc)
                    };
                }
                configs.Add(new MCConfModifiers(){ modifierSettings = modifierSettings });
            }
            // CueOnApply
            if (data.CueOnApply is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnApply.Count];
                for (var i = 0; i < data.CueOnApply.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnApply[i]);
                configs.Add(new ConfCueOnApply { cues = cues });
            }
            // CueOnTick
            if (data.CueOnTick is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnTick.Count];
                for (var i = 0; i < data.CueOnTick.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnTick[i]);
                configs.Add(new ConfCueOnTick { cues = cues });
            }
            // CueOnAdd
            if (data.CueOnAdd is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnAdd.Count];
                for (var i = 0; i < data.CueOnAdd.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnAdd[i]);
                configs.Add(new ConfCueOnAdd { cues = cues });
            }
            // CueOnRemove
            if (data.CueOnRemove is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnRemove.Count];
                for (var i = 0; i < data.CueOnRemove.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnRemove[i]);
                configs.Add(new ConfCueOnRemove { cues = cues });
            }
            // CueOnActivate
            if (data.CueOnActivate is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnActivate.Count];
                for (var i = 0; i < data.CueOnActivate.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnActivate[i]);
                configs.Add(new ConfCueOnActivate { cues = cues });
            }
            // CueOnDeactivate
            if (data.CueOnDeactivate is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnDeactivate.Count];
                for (var i = 0; i < data.CueOnDeactivate.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnDeactivate[i]);
                configs.Add(new ConfCueOnDeactivate { cues = cues });
            }
            // grantedAbility
            if (data.GrantedAbility.Count > 0)
            {
                var grantedAbilities = new GrantedAbility[data.GrantedAbility.Count];
                for (var i = 0; i < data.GrantedAbility.Count; i++)
                {
                    var info = data.GrantedAbility[i];
                    grantedAbilities[i] = new GrantedAbility()
                    {
                        //AbilityConfig = GetAbilityConfig(info.Id),
                        ActivationPolicy = (GrantedAbilityActivationPolicy)info.ActivationPolicy,
                        DeactivationPolicy = (GrantedAbilityDeactivationPolicy)info.DeactivationPolicy,
                        Level = info.Level,
                        RemovePolicy = (GrantedAbilityRemovePolicy)info.RemovePolicy,
                    };
                }
                configs.Add(new MCConfGrantedAbility() { GrantedAbilities = grantedAbilities });
            }
            // stacking
            if (data.Stacking.StackCode != 0)
            {
                var effectConfigs = new List<GameplayEffectConfig>();
                foreach (var effectID in data.Stacking.OverflowEffects)
                {
                    var effect = GetGameplayEffectConfig(effectID);
                    effectConfigs.Add(effect);
                }
                configs.Add(new ConfStacking()
                {
                    StackingCode = data.Stacking.StackCode,
                    StackType = (EffectStackType)data.Stacking.StackingType,
                    LimitCount = data.Stacking.LimitCount,
                    EffectDurationRefreshPolicy = (EffectDurationRefreshPolicy)data.Stacking.DurationRefreshPolicy,
                    EffectPeriodResetPolicy = (EffectPeriodResetPolicy)data.Stacking.PeriodResetPolicy,
                    EffectExpirationPolicy = (EffectExpirationPolicy)data.Stacking.ExpirationPolicy,
                    denyOverflowApplication = data.Stacking.DenyOverflowApplication,
                    clearStackOnOverflow = data.Stacking.ClearStackOnOverflow,
                    overflowEffects = effectConfigs.ToArray()
                });
            }

            return new GameplayEffectConfig(configs.ToArray());
        }
    }
}
