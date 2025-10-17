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
            if (_tables != null) return;
            _tables = new Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
        }

        public static void Init()
        {
            LoadTables();
            GameplayEffectHelper.RegisterGetConfigByIDFunc(GetGameplayEffectConfig);
        }

        public static AbilitySystemCellConfig GetAscConfig(int id)
        {
            var data = Tables.Tbasc.Get(id);
            if (data == null)
            {
                Debug.LogError($"ASC_ID:{id}  不存在.");
                return new AbilitySystemCellConfig(Array.Empty<int>(), Array.Empty<AttributeSetConfig>(),Array.Empty<AbilityConfig>(), 0);
            }
            var abilityIds = data.Ability;
            var abilities = new AbilityConfig[abilityIds.Length];
            for (var i = 0; i < abilityIds.Length; i++)
            {
                var abilityId = abilityIds[i];
                abilities[i] = GetAbilityConfig(abilityId);
            }
            var attrSets = new AttributeSetConfig[data.AttrSet.Length];
            for (var i = 0; i < data.AttrSet.Length; i++)
                attrSets[i] = XAttrSet.AttributeSetMap[data.AttrSet[i]];
            return new AbilitySystemCellConfig(data.Tag, attrSets, abilities, data.Level);
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
                switch (cueLogic)
                {
                    case cfg.GameplayCueLog cData:
                    {
                        var cp = cueParam as GAS.Runtime.CueParamString;
                        cp?.SetValue(cData.Value);
                        cueParam = cp;
                        break;
                    }
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
            if (data.Duration != null && data.Duration.Value.Time != 0)
                configs.Add(new ConfDuration
                {
                    duration = data.Duration.Value.Time,
                    timeUnit = (TimeUnit)data.Duration.Value.TimeUnit,
                    ResetStartTimeWhenActivated = data.Duration.Value.ResetStartTimeWhenActivated
                });
            // period
            if (data.Period is { Time: > 0 })
            {
                var gameplayEffectSettings = new List<GameplayEffectComponentConfig[]>();
                foreach (var effectID in data.Period.Value.Effects)
                {
                    var effect = GetGameplayEffectConfig(effectID);
                    gameplayEffectSettings.Add(effect.ComponentConfigs);
                }
                configs.Add(new ConfPeriod
                {
                    Period = data.Period.Value.Time,
                    ResetTimeCountWhenDeactivated = data.Period.Value.FirstTrigger,
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
                        MMC = GetMmcConfig(info.Mmc)
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
                        AbilityConfig = GetAbilityConfig(info.Id),
                        ActivationPolicy = (GrantedAbilityActivationPolicy)info.ActivationPolicy,
                        DeactivationPolicy = (GrantedAbilityDeactivationPolicy)info.DeactivationPolicy,
                        Level = info.Level,
                        RemovePolicy = (GrantedAbilityRemovePolicy)info.RemovePolicy,
                    };
                }
                configs.Add(new MCConfGrantedAbility() { GrantedAbilities = grantedAbilities });
            }
            // stacking
            if (data.Stacking!=null && data.Stacking.Value.StackCode != 0)
            {
                var effectConfigs = new List<GameplayEffectConfig>();
                foreach (var effectID in data.Stacking.Value.OverflowEffects)
                {
                    var effect = GetGameplayEffectConfig(effectID);
                    effectConfigs.Add(effect);
                }
                configs.Add(new ConfStacking()
                {
                    StackingCode = data.Stacking.Value.StackCode,
                    StackType = (EffectStackType)data.Stacking.Value.StackingType,
                    LimitCount = data.Stacking.Value.LimitCount,
                    EffectDurationRefreshPolicy = (EffectDurationRefreshPolicy)data.Stacking.Value.DurationRefreshPolicy,
                    EffectPeriodResetPolicy = (EffectPeriodResetPolicy)data.Stacking.Value.PeriodResetPolicy,
                    EffectExpirationPolicy = (EffectExpirationPolicy)data.Stacking.Value.ExpirationPolicy,
                    denyOverflowApplication = data.Stacking.Value.DenyOverflowApplication,
                    clearStackOnOverflow = data.Stacking.Value.ClearStackOnOverflow,
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
                return new AbilityConfig(Array.Empty<AbilityComponentConfig>());
            }

            var configs = new List<AbilityComponentConfig>();

            // baseInfo
            configs.Add(new ConfAbilityBaseInfo { Code = id, Level = 0 });
            // cost
            if (data.Cost != 0)
                configs.Add(new ConfAbilityCost{ CostComponentConfigs = GetGameplayEffectConfig(data.Cost).ComponentConfigs });
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
            {
                configs.Add(new ConfAbilityCooldown
                {
                    Cooldown = data.Cd,
                    CooldownComponentConfigs = GetGameplayEffectConfig(data.CdEffect).ComponentConfigs
                });
            }
            // abilityLogic
            var abilityLogicType = AbilityHelper.GetAbilityLogicType(data.AbilityLogic.GetType().Name);
            if (abilityLogicType == null)
                Debug.LogError($"Ability_ID:{id}  AbilityLogicType:{data.AbilityLogic.GetType().Name} 不存在.");
            else
            {
                var abilityLogic = data.AbilityLogic;
                var abilityLogicName = abilityLogic.GetType().Name;
                var abilityLogicParamType = AbilityHelper.GetAbilityLogicParamType(abilityLogicName);
                var abilityParam = Activator.CreateInstance(abilityLogicParamType) as IAbilityParam;
                if (abilityParam != null)
                {
                    switch (abilityLogic)
                    {
                        case cfg.ALMove aData:
                        {
                            var ap = abilityParam as DemoForESC._Script.Gas.Ability.AbilityParamMove;
                            ap?.SetRotationOffset(aData.RotationOffset);
                            abilityParam = ap;
                            break;
                        }
                        case cfg.ALApplyEffect aData:
                        {
                            var ap = abilityParam as GAS.Runtime.AbilityParamArrayInt;
                            ap?.SetValue(aData.Value);
                            abilityParam = ap;
                            break;
                        }
                        case cfg.ALDebugLog aData:
                        {
                            var ap = abilityParam as GAS.Runtime.AbilityParamString;
                            ap?.SetValue(aData.Value);
                            abilityParam = ap;
                            break;
                        }
                    }
                }
                configs.Add(new MCConfAbilityLogic()
                {
                    AbilityLogicType = abilityLogicName,
                    abilityParam = abilityParam
                });
            }

            return new AbilityConfig(configs.ToArray());
        }

        public static MMCConfig GetMmcConfig(int id)
        {
            var data = Tables.Tbmmc.Get(id);
            if (data == null)
            {
                Debug.LogError($"MMC_ID:{id}  不存在.");
                return new MMCConfig() { };
            }

            var mmcLogic = data.MmcLogic;
            var mmcLogicName = data.MmcLogic.GetType().Name;
            var mmcLogicParamType = MmcHelper.GetMmcParamTypeByMmcType(mmcLogicName);
            IMmcParameter mmcParam = Activator.CreateInstance(mmcLogicParamType) as IMmcParameter;
            if (mmcParam != null)
            {
                switch (mmcLogic)
                {
                    case cfg.MMCScalableFloat mmcData:
                    {
                        var mp = mmcParam as GAS.Runtime.MmcParaFloatScale;
                        mp?.SetK(mmcData.K);
                        mp?.SetB(mmcData.B);
                        mmcParam = mp;
                        break;
                    }
                }
            }

            return new MMCConfig()
            {
                MmcType = MmcHelper.GetMmcType(mmcLogicName),
                MmcParameter = mmcParam
            };
        }

        public static string GetAbilityNameByCode(int id)
        {
            var data = Tables.Tbability.Get(id);
            if (data != null) return data.Name;
            Debug.LogError($"Ability_ID:{id}  不存在.");
            return string.Empty;
        }

        public static string GetAttrSetNameByCode(int code)
        {
            var data = Tables.TbattributeSet.Get(code);
            if (data != null) return data.Name;
            Debug.LogError($"AttrSet_Code:{code}  不存在.");
            return string.Empty;
        }

        public static string GetAttributeNameByCode(int code)
        {
            var data = Tables.Tbattribute.Get(code);
            if (data != null) return data.Name;
            Debug.LogError($"Attribute_Code:{code}  不存在.");
            return string.Empty;
        }
    }
}
