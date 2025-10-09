///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using cfg;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.ComponentConfig;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Static;
using SimpleJSON;
using UnityEngine;

namespace GAS.Runtime
{
    public static class XLuban
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
            if (_tables != null) return; // Already loaded
            _tables = new Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
        }

        public static AbilitySystemCellConfig GetAscConfig(int id)
        {
            var data = Tables.Tbasc.Get(id);
            if (data == null)
            {
                Debug.LogError($"ASC_ID:{id}  不存在.");
                return new AbilitySystemCellConfig(
                    Array.Empty<int>(), Array.Empty<int>(),
                    Array.Empty<AbilityConfig>(), 0);
            }

            var abilityIds = data.Ability;
            var abilities = new AbilityConfig[abilityIds.Length];
            for (var i = 0; i < abilityIds.Length; i++)
            {
                var abilityId = abilityIds[i];
                abilities[i] = GetAbilityConfig(abilityId);
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
                    var gameplayCueLogParam = cueParam as CueParamString;
                    gameplayCueLogParam?.SetValue(gameplayCueLog.Value);
                    cueParam = gameplayCueLogParam;
                }
                if (cueLogic is cfg.CueLoging cueLoging)
                {
                    var cueLogingParam = cueParam as CueParamString;
                    cueLogingParam?.SetValue(cueLoging.Value);
                    //cueLogingParam?.SetD(cueLoging.Duration);
                    cueParam = cueLogingParam;
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

            // TODO
            // duration
            if (data.Duration != null && data.Duration.Time != 0)
                configs.Add(new ConfDuration
                {
                    duration = data.Duration.Time,
                    timeUnit = (TimeUnit)data.Duration.TimeUnit
                    //ResetStartTimeWhenActivated = data.Duration.
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
            // TODO
            // modifiers

            // cueOnApply
            if (data.CueOnApply is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnApply.Count];
                for (var i = 0; i < data.CueOnApply.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnApply[i]);
                configs.Add(new ConfCueOnApply { cues = cues });
            }
            // TODO
            // cueOnTick
            // if (data.CueOnTick is { Count: > 0 })
            // {
            //     var cues = new GameplayCueConfig[data.CueOnTick.Count];
            //     for (var i = 0; i < data.CueOnTick.Count; i++)
            //         cues[i] = GetGameplayCueConfig(data.CueOnTick[i]);
            //     configs.Add(new confcueti() { cues = cues });
            // }

            // cueOnAdd
            if (data.CueOnAdd is { Count: > 0 })
            {
                var cues = new GameplayCueConfig[data.CueOnAdd.Count];
                for (var i = 0; i < data.CueOnAdd.Count; i++)
                    cues[i] = GetGameplayCueConfig(data.CueOnAdd[i]);
                configs.Add(new ConfCueOnAdd { cues = cues });
            }

            // TODO
            // cueOnRemove

            // TODO
            // cueOnActivate

            // TODO
            // cueOnDeactivate

            // TODO
            // grantedAbility
            if (data.GrantedAbility.Count > 0)
            {
                // configs.Add(new Confabili);
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

        public static AbilityConfig GetAbilityConfig(int id)
        {
            var data = Tables.Tbability.Get(id);
            if (data == null)
            {
                Debug.LogError($"Ability_ID:{id}  不存在.");
                return new AbilityConfig(Array.Empty<GameplayAbilityComponentConfig>());
            }

            var configs = new List<GameplayAbilityComponentConfig>();
            // cost								
            if (data.Cost != 0)
                configs.Add(new ConfAbilityCost
                {
                    CostComponentConfigs = GetGameplayEffectConfig(data.Cost).ComponentConfigs
                });

            // assetTags
            if (data.AssetTags is { Count: > 0 })
                configs.Add(new ConfAbilityAssetTags { tags = data.AssetTags.ToArray() });

            // cancelAbilityWithTags
            if (data.CancelAbilityWithTags is { Count: > 0 })
                configs.Add(new ConfCancelAbilityTags { tags = data.CancelAbilityWithTags.ToArray() });

            // blockAbilityWithTags
            if (data.BlockAbilityWithTags is { Count: > 0 })
                configs.Add(new ConfBlockAbilityTags { tags = data.BlockAbilityWithTags.ToArray() });

            // activationOwnedTags
            if (data.ActivationOwnedTags is { Count: > 0 })
                configs.Add(new ConfAbilityActivationOwnedTags { tags = data.ActivationOwnedTags.ToArray() });

            // activationRequiredTags
            if (data.ActivationRequiredTags is { Count: > 0 })
                configs.Add(new ConfAbilityActivationRequiredTags { tags = data.ActivationRequiredTags.ToArray() });

            // activationBlockedTags
            if (data.ActivationBlockedTags is { Count: > 0 })
                configs.Add(new ConfAbilityActivationBlockedTags { tags = data.ActivationBlockedTags.ToArray() });

            // cdEffect cd
            if (data.Cd != 0)
                configs.Add(new ConfAbilityCooldown
                {
                    Cooldown = data.Cd,
                    CooldownComponentConfigs = GetGameplayEffectConfig(data.CdEffect).ComponentConfigs
                });
            
            // abilityLogic						
            var abilityLogicType = AbilityHelper.GetAbilityLogicType(data.AbilityLogic.GetType().Name);
            if (abilityLogicType == null)
            {
                Debug.LogError($"Ability_ID:{id}  AbilityLogicType:{data.AbilityLogic.GetType().Name} 不存在.");
            }
            else
            {
                var abilityLogic = data.AbilityLogic;
                var abilityLogicName = abilityLogic.GetType().Name;
                var abilityLogicParamType = AbilityHelper.GetAbilityLogicParamType(abilityLogicName);
                var abilityParam = Activator.CreateInstance(abilityLogicParamType) as IAbilityParam;
                if (abilityParam != null)
                {
                    if (abilityLogic is cfg.ALApplyEffect alApplyEffect)
                    {
                        var alApplyEffectParam = abilityParam as AbilityParamArrayInt;
                        alApplyEffectParam?.SetValue(alApplyEffect.Value);
                        abilityParam = alApplyEffectParam;
                    }
                    if (abilityLogic is cfg.ALDebugLog alDebugLog)
                    {
                        var alDebugLogParam = abilityParam as AbilityParamString;
                        alDebugLogParam?.SetValue(alDebugLog.Value);
                        abilityParam = alDebugLogParam;
                    }
                }
                configs.Add(new MCConfAbilityLogic()
                {
                    abilityParam = abilityParam
                });
            }

            return new AbilityConfig(configs.ToArray());
        }
    }
}