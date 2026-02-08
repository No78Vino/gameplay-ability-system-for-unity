using System;
using System.IO;
using System.Linq;
using GAS.General;

namespace GAS.Editor
{
    public static class CodeGeneratorLubanPart
    {
        public static void GenerateLubanExtension()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeLubanExtesion;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using cfg;");
            writer.WriteLine("using SimpleJSON;");
            writer.WriteLine("using System.IO;");
            writer.WriteLine("using UnityEngine;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XLuban");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("private static Tables _tables;");
                    writer.WriteLine("public static Tables Tables");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("get");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (_tables != null) return _tables;");
                        writer.WriteLine("Debug.LogError(\"XLuban.Tables 未初始化!\");");
                        writer.WriteLine("return null;");
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");

                    writer.WriteLine("public static void LoadTables(Func<string, JSONNode> loader)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("if (_tables != null) return;");
                    writer.WriteLine(
                        "_tables = new Tables(loader);");
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    writer.WriteLine("public static void Init(Func<string, JSONNode> loader)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("LoadTables(loader);");
                    writer.WriteLine("GameplayEffectHelper.RegisterGetConfigByIDFunc(GetGameplayEffectConfig);");
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    #region ASC

                    writer.WriteLine("public static AbilitySystemCellConfig GetAscConfig(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.Tbasc.Get(id);");
                        writer.WriteLine("if (data == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Debug.LogError($\"ASC_ID:{id}  不存在.\");");
                        writer.WriteLine(
                            "return new AbilitySystemCellConfig(Array.Empty<int>(), Array.Empty<AttrSetConfig>(),Array.Empty<AbilityConfig>(), 0);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("var abilityIds = data.Ability;");
                        writer.WriteLine("var abilities = new AbilityConfig[abilityIds.Length];");
                        writer.WriteLine("for (var i = 0; i < abilityIds.Length; i++)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var abilityId = abilityIds[i];");
                        writer.WriteLine("abilities[i] = GetAbilityConfig(abilityId);");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("var attrSets = new AttrSetConfig[data.AttrSet.Length];");
                        writer.WriteLine("for (var i = 0; i < data.AttrSet.Length; i++)");
                        writer.WriteLine("    attrSets[i] = XAttrSet.AttributeSetMap[data.AttrSet[i]];");

                        writer.WriteLine(
                            "return new AbilitySystemCellConfig(data.Tag, attrSets, abilities, data.Level);");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion

                    writer.WriteLine("");

                    #region GameplayCue

                    writer.WriteLine("public static GameplayCueConfig GetGameplayCueConfig(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.TbgameplayCue.Get(id);");
                        writer.WriteLine("if (data == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Debug.LogError($\"Cue_ID:{id}  不存在.\");");
                        writer.WriteLine("return null;");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("var cueType = CueHelper.GetCueType(data.CueLogic.GetType().Name);");
                        writer.WriteLine("if (cueType == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine(
                            "Debug.LogError($\"Cue_ID:{id}  CueType:{data.CueLogic.GetType().Name} 不存在.\");");
                        writer.WriteLine("return null;");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("var cueLogic = data.CueLogic;");
                        writer.WriteLine("var cueLogicName = cueLogic.GetType().Name;");
                        writer.WriteLine("var cueParamType = CueHelper.GetCueLogicParamType(cueLogicName);");
                        writer.WriteLine("var cueParam = Activator.CreateInstance(cueParamType) as XParam;");
                        writer.WriteLine("if (cueParam != null)");
                        writer.WriteLine("{");
                        writer.Indent++;

                        writer.WriteLine("switch (cueLogic)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            var allCue = EditorCueHelper.GetCachedCueTypes();
                            var cueTypes = allCue as Type[] ?? allCue.ToArray();
                            foreach (var cueType in cueTypes)
                            {
                                var cueName = cueType.Name;
                                var cueParamType = EditorCueHelper.CueToCueParamTypeMap()[cueName];
                                Type tType = ReflectionHelper.GetMemberType($"cfg.{cueName}", "Param");
                                if (tType == null) continue;

                                writer.WriteLine($"case cfg.{cueName} cData:");
                                writer.WriteLine("{");
                                writer.Indent++;
                                writer.WriteLine($"var cp = cueParam as {cueParamType.FullName};");

                                var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                                foreach (var fieldName in readOnlyFields)
                                    writer.WriteLine($"cp?.Set{fieldName}(cData.Param.{fieldName});");

                                writer.WriteLine("cueParam = cp;");
                                writer.WriteLine("break;");
                                writer.Indent--;
                                writer.WriteLine("}");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine(
                            "return new GameplayCueConfig(cueType, cueParam, data.RequiredTag.ToArray(), data.ImmunityTag.ToArray());");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion

                    writer.WriteLine("");

                    #region GameplayEffect

                    writer.WriteLine("public static GameplayEffectConfig GetGameplayEffectConfig(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.TbgameplayEffect.Get(id);");
                        writer.WriteLine("if (data == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Debug.LogError($\"GameplayEffect_ID:{id}  不存在.\");");
                        writer.WriteLine("return null;");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("var configs = new List<GameplayEffectComponentConfig>();");
                        writer.WriteLine("");
                        writer.WriteLine("// assetTags");
                        writer.WriteLine("if (data.AssetTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfAssetTags { tags = data.AssetTags.ToArray() });");
                        writer.WriteLine("// grantedTags");
                        writer.WriteLine("if (data.GrantedTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfEffectGrantedTags { tags = data.GrantedTags.ToArray() });");
                        writer.WriteLine("// applicationRequiredTags");
                        writer.WriteLine("if (data.ApplicationRequiredTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfApplicationRequiredTags { tags = data.ApplicationRequiredTags.ToArray() });");
                        writer.WriteLine("// ongoingRequiredTags");
                        writer.WriteLine("if (data.OngoingRequiredTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfOngoingRequiredTags { tags = data.OngoingRequiredTags.ToArray() });");
                        writer.WriteLine("// removeGameplayEffectsWithTags");
                        writer.WriteLine("if (data.RemoveGameplayEffectsWithTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfRemoveEffectWithTags { tags = data.RemoveGameplayEffectsWithTags.ToArray() });");
                        writer.WriteLine("// immunityTags");
                        writer.WriteLine("if (data.ImmunityTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfEffectImmunityTags { tags = data.ImmunityTags.ToArray() });");

                        writer.WriteLine("// duration");
                        writer.WriteLine("if (data.Duration != null && data.Duration.Value.Time != 0)");
                        writer.Indent++;
                        writer.WriteLine("configs.Add(new ConfDuration");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("duration = data.Duration.Value.Time,");
                        writer.WriteLine("timeUnit = (TimeUnit)data.Duration.Value.TimeUnit,");
                        writer.WriteLine(
                            "ResetStartTimeWhenActivated = data.Duration.Value.ResetStartTimeWhenActivated");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;

                        writer.WriteLine("// period");
                        writer.WriteLine("if (data.Period is { Time: > 0 })");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var gameplayEffectSettings = new List<GameplayEffectComponentConfig[]>();");
                        writer.WriteLine("foreach (var effectID in data.Period.Value.Effects)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effect = GetGameplayEffectConfig(effectID);");
                        writer.WriteLine("gameplayEffectSettings.Add(effect.ComponentConfigs);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new ConfPeriod");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Period = data.Period.Value.Time,");
                        writer.WriteLine("ResetTimeCountWhenDeactivated = data.Period.Value.FirstTrigger,");
                        writer.WriteLine("GameplayEffectSettings = gameplayEffectSettings.ToArray()");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("// modifiers");
                        writer.WriteLine("if (data.Modifiers != null && data.Modifiers.Count > 0)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine(
                            "ModifierSetting[] modifierSettings = new ModifierSetting[data.Modifiers.Count];");
                        writer.WriteLine("for (var i = 0; i < data.Modifiers.Count; i++)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var info = data.Modifiers[i];");
                        writer.WriteLine("modifierSettings[i] = new ModifierSetting()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("AttrSetCode = info.AttrSet,");
                        writer.WriteLine("AttrCode = info.Attribute,");
                        writer.WriteLine("Magnitude = info.Magnitude,");
                        writer.WriteLine("Operation = (GEOperation)info.Operation,");
                        writer.WriteLine("MMC = GetMmcConfig(info.Mmc)");
                        writer.Indent--;
                        writer.WriteLine("};");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new MCConfModifiers(){ modifierSettings = modifierSettings });");
                        writer.Indent--;
                        writer.WriteLine("}");

                        var cueComNames = new[]
                        {
                            "CueOnApply",
                            "CueOnTick",
                            "CueOnAdd",
                            "CueOnRemove",
                            "CueOnActivate",
                            "CueOnDeactivate"
                        };
                        foreach (var cueComName in cueComNames)
                        {
                            writer.WriteLine($"// {cueComName}");
                            writer.WriteLine($"if (data.{cueComName} is {{ Count: > 0 }})");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine($"var cues = new GameplayCueConfig[data.{cueComName}.Count];");
                            writer.WriteLine($"for (var i = 0; i < data.{cueComName}.Count; i++)");
                            writer.WriteLine($"    cues[i] = GetGameplayCueConfig(data.{cueComName}[i]);");
                            writer.WriteLine($"configs.Add(new Conf{cueComName} {{ cues = cues }});");
                            writer.Indent--;
                            writer.WriteLine("}");
                        }

                        writer.WriteLine("// grantedAbility");
                        writer.WriteLine("if (data.GrantedAbility.Count > 0)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var grantedAbilities = new GrantedAbility[data.GrantedAbility.Count];");
                        writer.WriteLine("for (var i = 0; i < data.GrantedAbility.Count; i++)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var info = data.GrantedAbility[i];");
                        writer.WriteLine("grantedAbilities[i] = new GrantedAbility()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("AbilityConfig = GetAbilityConfig(info.ID),");
                        writer.WriteLine("ActivationPolicy = (GrantedAbilityActivationPolicy)info.ActivationPolicy,");
                        writer.WriteLine(
                            "DeactivationPolicy = (GrantedAbilityDeactivationPolicy)info.DeactivationPolicy,");
                        writer.WriteLine("Level = info.Level,");
                        writer.WriteLine("RemovePolicy = (GrantedAbilityRemovePolicy)info.RemovePolicy,");
                        writer.Indent--;
                        writer.WriteLine("};");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine(
                            "configs.Add(new MCConfGrantedAbility() { GrantedAbilities = grantedAbilities });");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("// stacking");
                        writer.WriteLine("if (data.Stacking!=null && data.Stacking.Value.StackCode != 0)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effectConfigs = new List<GameplayEffectConfig>();");
                        writer.WriteLine("foreach (var effectID in data.Stacking.Value.OverflowEffects)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effect = GetGameplayEffectConfig(effectID);");
                        writer.WriteLine("effectConfigs.Add(effect);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new ConfStacking()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("StackingCode = data.Stacking.Value.StackCode,");
                        writer.WriteLine("StackType = (EffectStackType)data.Stacking.Value.StackingType,");
                        writer.WriteLine("LimitCount = data.Stacking.Value.LimitCount,");
                        writer.WriteLine(
                            "EffectDurationRefreshPolicy = (EffectDurationRefreshPolicy)data.Stacking.Value.DurationRefreshPolicy,");
                        writer.WriteLine(
                            "EffectPeriodResetPolicy = (EffectPeriodResetPolicy)data.Stacking.Value.PeriodResetPolicy,");
                        writer.WriteLine(
                            "EffectExpirationPolicy = (EffectExpirationPolicy)data.Stacking.Value.ExpirationPolicy,");
                        writer.WriteLine("denyOverflowApplication = data.Stacking.Value.DenyOverflowApplication,");
                        writer.WriteLine("clearStackOnOverflow = data.Stacking.Value.ClearStackOnOverflow,");
                        writer.WriteLine("overflowEffects = effectConfigs.ToArray()");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("");
                        writer.WriteLine("return new GameplayEffectConfig(configs.ToArray());");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion

                    writer.WriteLine("");

                    #region Ability

                    writer.WriteLine("public static AbilityConfig GetAbilityConfig(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.Tbability.Get(id);");
                        writer.WriteLine("if (data == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Debug.LogError($\"Ability_ID:{id}  不存在.\");");
                        writer.WriteLine("return new AbilityConfig(Array.Empty<AbilityComponentConfig>());");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");

                        writer.WriteLine("var configs = new List<AbilityComponentConfig>();");
                        writer.WriteLine("");

                        writer.WriteLine("// baseInfo");
                        writer.WriteLine("configs.Add(new ConfAbilityBaseInfo { Code = id, Level = 0 });");

                        writer.WriteLine("// cost");
                        writer.WriteLine("if (data.Cost != 0)");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityCost{ CostComponentConfigs = GetGameplayEffectConfig(data.Cost).ComponentConfigs });");

                        writer.WriteLine("// assetTags");
                        writer.WriteLine("if (data.AssetTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityAssetTags { tags = data.AssetTags.ToArray() });");

                        writer.WriteLine("// cancelAbilityWithTags");
                        writer.WriteLine("if (data.CancelAbilityWithTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfCancelAbilityTags { tags = data.CancelAbilityWithTags.ToArray() });");

                        writer.WriteLine("// blockAbilityWithTags");
                        writer.WriteLine("if (data.BlockAbilityWithTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfBlockAbilityTags { tags = data.BlockAbilityWithTags.ToArray() });");

                        writer.WriteLine("// activationOwnedTags");
                        writer.WriteLine("if (data.ActivationOwnedTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationOwnedTags { tags = data.ActivationOwnedTags.ToArray() });");

                        writer.WriteLine("// activationRequiredTags");
                        writer.WriteLine("if (data.ActivationRequiredTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationRequiredTags { tags = data.ActivationRequiredTags.ToArray() });");

                        writer.WriteLine("// activationBlockedTags");
                        writer.WriteLine("if (data.ActivationBlockedTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationBlockedTags { tags = data.ActivationBlockedTags.ToArray() });");

                        writer.WriteLine("// cdEffect cd");
                        writer.WriteLine("if (data.Cd != 0)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("configs.Add(new ConfAbilityCooldown");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Cooldown = data.Cd,");
                        writer.WriteLine(
                            "CooldownComponentConfigs = GetGameplayEffectConfig(data.CdEffect).ComponentConfigs");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("// abilityLogic");
                        writer.WriteLine(
                            "var abilityLogicType = AbilityHelper.GetAbilityLogicType(data.AbilityLogic.GetType().Name);");
                        writer.WriteLine("if (abilityLogicType == null)");
                        writer.WriteLine(
                            "    Debug.LogError($\"Ability_ID:{id}  AbilityLogicType:{data.AbilityLogic.GetType().Name} 不存在.\");");
                        writer.WriteLine("else");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("var abilityLogic = data.AbilityLogic;");
                            writer.WriteLine("var abilityLogicName = abilityLogic.GetType().Name;");
                            writer.WriteLine(
                                "var abilityLogicParamType = AbilityHelper.GetAbilityLogicParamType(abilityLogicName);");
                            writer.WriteLine(
                                "var abilityParam = Activator.CreateInstance(abilityLogicParamType) as XParam;");
                            writer.WriteLine("if (abilityParam != null)");
                            writer.WriteLine("{");
                            writer.Indent++;
                            {
                                writer.WriteLine("switch (abilityLogic)");
                                writer.WriteLine("{");
                                writer.Indent++;


                                var allAbilities = EditorAbilityHelper.GetCachedAbilityLogicTypes();
                                var abilityTypes = allAbilities as Type[] ?? allAbilities.ToArray();
                                foreach (var abilityType in abilityTypes)
                                {
                                    var abilityTypeName = abilityType.Name;
                                    var abilityParamType =
                                        EditorAbilityHelper.AbilityToAbilityParamTypeMap()[abilityTypeName];
                                    Type tType = ReflectionHelper.GetMemberType($"cfg.{abilityTypeName}", "Param");
                                    if (tType == null) continue;

                                    writer.WriteLine($"case cfg.{abilityTypeName} aData:");
                                    writer.WriteLine("{");
                                    writer.Indent++;
                                    writer.WriteLine($"var ap = abilityParam as {abilityParamType.FullName};");
                                    var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                                    foreach (var fieldName in readOnlyFields)
                                    {
                                        writer.WriteLine($"ap?.Set{fieldName}(aData.Param.{fieldName});");
                                    }

                                    if (abilityParamType.Name == "XParamALTimelineID")
                                    {
                                        // 特殊处理TimelineAbility的Param，主动生成XParamTimeline的缓存
                                        writer.WriteLine("// 缓存Timeline参数");
                                        writer.WriteLine("if (ap != null)");
                                        writer.WriteLine("{");
                                        writer.Indent++;
                                        writer.WriteLine(
                                            "var xParamTimeline = GetTimelineAbilityParam(aData.Param.ID);");
                                        writer.WriteLine("ap.CacheTimelineParam(xParamTimeline);");
                                        writer.Indent--;
                                        writer.WriteLine("}");
                                    }

                                    writer.WriteLine("abilityParam = ap;");
                                    writer.WriteLine("break;");
                                    writer.Indent--;
                                    writer.WriteLine("}");
                                }

                                writer.Indent--;
                                writer.WriteLine("}");
                            }
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine("configs.Add(new MCConfAbilityLogic()");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine("AbilityLogicType = abilityLogicName,");
                            writer.WriteLine("Param = abilityParam");
                            writer.Indent--;
                            writer.WriteLine("});");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("return new AbilityConfig(configs.ToArray());");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion

                    writer.WriteLine("");

                    #region MMC

                    writer.WriteLine("public static MMCConfig GetMmcConfig(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.Tbmmc.Get(id);");
                        writer.WriteLine("if (data == null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Debug.LogError($\"MMC_ID:{id}  不存在.\");");
                        writer.WriteLine("return new MMCConfig() { };");
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("");

                        writer.WriteLine("var mmcLogic = data.MmcLogic;");
                        writer.WriteLine("var mmcLogicName = data.MmcLogic.GetType().Name;");
                        writer.WriteLine("var mmcLogicParamType = MmcHelper.GetMmcParamTypeByMmcType(mmcLogicName);");
                        writer.WriteLine(
                            "IMmcParameter mmcParam = Activator.CreateInstance(mmcLogicParamType) as IMmcParameter;");
                        writer.WriteLine("if (mmcParam != null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("switch (mmcLogic)");
                            writer.WriteLine("{");
                            writer.Indent++;


                            var mmcs = EditorMmcHelper.GetCachedMmcTypes();
                            var mmcTypes = mmcs as Type[] ?? mmcs.ToArray();
                            foreach (var mmcType in mmcTypes)
                            {
                                var mmcTypeName = mmcType.Name;
                                var mmcParamType = EditorMmcHelper.MmcToMmcParamTypeMap()[mmcTypeName];
                                var tType = ReflectionHelper.GetMemberType($"cfg.{mmcTypeName}", "Param");

                                if (tType == null) continue;

                                writer.WriteLine($"case cfg.{mmcTypeName} mmcData:");
                                writer.WriteLine("{");
                                writer.Indent++;
                                writer.WriteLine($"var mp = mmcParam as {mmcParamType.FullName};");
                                var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                                foreach (var fieldName in readOnlyFields)
                                {
                                    writer.WriteLine($"mp?.Set{fieldName}(mmcData.Param.{fieldName});");
                                }

                                writer.WriteLine("mmcParam = mp;");
                                writer.WriteLine("break;");
                                writer.Indent--;
                                writer.WriteLine("}");
                            }

                            writer.Indent--;
                            writer.WriteLine("}");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("return new MMCConfig()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("MmcType = MmcHelper.GetMmcType(mmcLogicName),");
                        writer.WriteLine("MmcParameter = mmcParam");
                        writer.Indent--;
                        writer.WriteLine("};");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion

                    writer.WriteLine("");

                    #region TimelineAbility

                    //                     public static XParamTimeline GetTimelineAbilityParam(int id)
                    // {
                    //     XParamTimeline timelineParam = new XParamTimeline();
                    //     var data = Tables.TbtimelineAbility.Get(id);
                    //     if (data == null)
                    //     {
                    //         Debug.LogError($"TimelineAbility_ID:{id}  不存在.");
                    //         return new XParamTimeline() { };
                    //     }
                    //
                    //     timelineParam.SetID(data.ID);
                    //     timelineParam.SetName(data.Name);
                    //     timelineParam.SetLifeTime(data.LifeTime);
                    //     timelineParam.SetManualEndAbility(data.ManualEndAbility);
                    //     
                    //     List<Track> tracks = new List<Track>();
                    //     foreach (var trackData in data.Tracks)
                    //     {
                    //         var track = new Track();
                    //         track.Name = trackData.Name;
                    //         track.TaskClips = new List<TaskClipData>();
                    //         foreach (var clipData in trackData.TaskClips)
                    //         {
                    //             var taskClip = new TaskClipData();
                    //             taskClip.Name = clipData.Name;
                    //             taskClip.StartTime = clipData.StartTime;
                    //             taskClip.EndTime = clipData.EndTime;
                    //             taskClip.TaskType = clipData.Task.GetType().Name;
                    //             
                    //             var taskParamType = AbilityHelper.GetAbilityTaskParamType(taskClip.TaskType);
                    //             var taskParam = Activator.CreateInstance(taskParamType) as XParam;
                    //             if (taskParam != null)
                    //             {
                    //                 switch (clipData.Task)
                    //                 {
                    //                     case cfg.TaskDoNothing taskData:
                    //                     {
                    //                         var tp = taskParam as GAS.Runtime.XParamNone;
                    //                         taskParam = tp;
                    //                         break;
                    //                     }
                    //                     case cfg.TaskDebug taskData:
                    //                     {
                    //                         var tp = taskParam as GAS.Runtime.XParamString;
                    //                         tp?.SetValue(taskData.Param.Value);
                    //                         taskParam = tp;
                    //                         break;
                    //                     }
                    //                 }
                    //             }
                    //             taskClip.Parameter = taskParam;
                    //             
                    //             track.TaskClips.Add(taskClip);
                    //         }
                    //         tracks.Add(track);
                    //     }
                    //     timelineParam.SetTracks(tracks);
                    //     return timelineParam;
                    // }
                    writer.WriteLine("");
                    writer.WriteLine("public static XParamTimeline GetTimelineAbilityParam(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("XParamTimeline timelineParam = new XParamTimeline();");
                    writer.WriteLine("var data = Tables.TbtimelineAbility.Get(id);");
                    writer.WriteLine("if (data == null)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("Debug.LogError($\"TimelineAbility_ID:{id}  不存在.\");");
                    writer.WriteLine("return new XParamTimeline() { };");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("timelineParam.SetID(data.ID);");
                    writer.WriteLine("timelineParam.SetName(data.Name);");
                    writer.WriteLine("timelineParam.SetLifeTime(data.LifeTime);");
                    writer.WriteLine("timelineParam.SetManualEndAbility(data.ManualEndAbility);");
                    writer.WriteLine("List<Track> tracks = new List<Track>();");
                    writer.WriteLine("foreach (var trackData in data.Tracks)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("var track = new Track();");
                    writer.WriteLine("track.Name = trackData.Name;");
                    writer.WriteLine("track.TaskClips = new List<TaskClipData>();");
                    writer.WriteLine("foreach (var clipData in trackData.TaskClips)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("var taskClip = new TaskClipData();");
                    writer.WriteLine("taskClip.Name = clipData.Name;");
                    writer.WriteLine("taskClip.StartTime = clipData.StartTime;");
                    writer.WriteLine("taskClip.EndTime = clipData.EndTime;");
                    writer.WriteLine("taskClip.TaskType = clipData.Task.GetType().Name;");
                    writer.WriteLine("var taskParamType = AbilityHelper.GetAbilityTaskParamType(taskClip.TaskType);");
                    writer.WriteLine("var taskParam = Activator.CreateInstance(taskParamType) as XParam;");
                    writer.WriteLine("if (taskParam != null)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("switch (clipData.Task)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        
                        
                        var allTasks = EditorAbilityHelper.GetCachedAbilityTaskTypes();
                        var taskTypes = allTasks as Type[] ?? allTasks.ToArray();
                        foreach (var taskType in taskTypes)
                        {
                            var taskTypeName = taskType.Name;
                            var taskParamType = EditorAbilityHelper.AbilityTaskToAbilityTaskParamTypeMap()[taskTypeName];
                            var tType = ReflectionHelper.GetMemberType($"cfg.{taskTypeName}", "Param");

                            if (tType == null) continue;

                            writer.WriteLine($"case cfg.{taskTypeName} taskData:");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine($"var tp = taskParam as {taskParamType.FullName};");
                            if (taskParamType.Name == "XParamCue")
                            {
                                // 特殊处理XParamCue的创建逻辑
                                writer.WriteLine($"tp?.SetCueLogic(CreateCueLogicUnit(taskData.Param.CueLogic,taskData.Param.RequiredTags,taskData.Param.ImmunityTags));");
                            }
                            else
                            {
                                var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                                foreach (var fieldName in readOnlyFields)
                                {
                                    writer.WriteLine($"tp?.Set{fieldName}(taskData.Param.{fieldName});");
                                }
                            }
                            

                            writer.WriteLine("taskParam = tp;");
                            writer.WriteLine("break;");
                            writer.Indent--;
                            writer.WriteLine("}");
                        }
                        
                        
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("taskClip.Parameter = taskParam;");
                    writer.WriteLine("track.TaskClips.Add(taskClip);");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("tracks.Add(track);");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("timelineParam.SetTracks(tracks);");
                    writer.WriteLine("return timelineParam;");
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.Indent--;
                    writer.WriteLine("");

                    #endregion

                    writer.WriteLine("");

                    #region 动态创建CueLogic
                    
                    writer.WriteLine("private static GameplayCueUnit CreateCueLogicUnit(cfg.CueLogic cueLogic, int[] requiredTags, int[] immunityTags)"); writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var cueLogicName = cueLogic.GetType().Name;");
                        writer.WriteLine("var cueParamType = CueHelper.GetCueLogicParamType(cueLogicName);");
                        writer.WriteLine("var cueParam = Activator.CreateInstance(cueParamType) as XParam;");
                        writer.WriteLine("if (cueParam != null)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("switch (cueLogic)");
                            writer.WriteLine("{");
                            writer.Indent++;
                            
                            
                            var allCueLogic = EditorCueHelper.GetCachedCueTypes();
                            var cueLogicTypes = allCueLogic as Type[] ?? allCueLogic.ToArray();
                            foreach (var cueLogicType in cueLogicTypes)
                            {
                                var cueLogicTypeName = cueLogicType.Name;
                                var cueParamType = EditorCueHelper.CueToCueParamTypeMap()[cueLogicTypeName];
                                var tType = ReflectionHelper.GetMemberType($"cfg.{cueLogicTypeName}", "Param");

                                if (tType == null) continue;

                                writer.WriteLine($"case cfg.{cueLogicTypeName} cData:");
                                writer.WriteLine("{");
                                writer.Indent++;
                                writer.WriteLine($"var cp = cueParam as {cueParamType.FullName};");
                                var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                                foreach (var fieldName in readOnlyFields)
                                {
                                    writer.WriteLine($"cp?.Set{fieldName}(cData.Param.{fieldName});");
                                }

                                writer.WriteLine("cueParam = cp;");
                                writer.WriteLine("break;");
                                writer.Indent--;
                                writer.WriteLine("}");
                            }
                            
                            
                            writer.Indent--;
                            writer.WriteLine("}");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("var cueLogicType = CueHelper.GetCueType(cueLogicName);");
                        writer.WriteLine("return new GameplayCueUnit(cueLogicType, cueParam, requiredTags, immunityTags);");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion
                    
                    #region Utils

                    writer.WriteLine("public static string GetAbilityNameByCode(int id)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.Tbability.Get(id);");
                        writer.WriteLine("if (data != null) return data.Name;");
                        writer.WriteLine("Debug.LogError($\"Ability_ID:{id}  不存在.\");");
                        writer.WriteLine("return string.Empty;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");

                    writer.WriteLine("public static string GetAttrSetNameByCode(int code)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.TbattributeSet.Get(code);");
                        writer.WriteLine("if (data != null) return data.Name;");
                        writer.WriteLine("Debug.LogError($\"AttrSet_Code:{code}  不存在.\");");
                        writer.WriteLine("return string.Empty;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");

                    writer.WriteLine("public static string GetAttributeNameByCode(int code)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("var data = Tables.Tbattribute.Get(code);");
                        writer.WriteLine("if (data != null) return data.Name;");
                        writer.WriteLine("Debug.LogError($\"Attribute_Code:{code}  不存在.\");");
                        writer.WriteLine("return string.Empty;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    #endregion
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}