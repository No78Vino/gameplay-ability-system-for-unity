using System;
using System.IO;
using System.Linq;
using GAS.Runtime;
using GAS.Editor;

namespace GAS.Editor
{
    public static class CodeGeneratorLubanPart
    {
        // TODO
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
            writer.WriteLine("using GAS.RuntimeWithECS;");
            writer.WriteLine("using GAS.RuntimeWithECS.AbilitySystemCell;");
            writer.WriteLine("using GAS.RuntimeWithECS.ComponentConfig;");
            writer.WriteLine("using GAS.RuntimeWithECS.GameplayEffect;");
            writer.WriteLine("using GAS.RuntimeWithECS.Static;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class EXLuban");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine(
                        $"public const string GAME_CONF_DIR = \"{GASSettingAsset.Instance.TableOutpuPath}\";");
                    writer.WriteLine("private static Tables _tables;");
                    writer.WriteLine("public static Tables Tables");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("get");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (_tables == null) LoadTables();");
                        writer.WriteLine("return _tables;");
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");

                    writer.WriteLine("public static void LoadTables()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine("if (_tables != null) return;");
                    writer.WriteLine(
                        "_tables = new Tables(file => JSON.Parse(File.ReadAllText($\"{GAME_CONF_DIR}/{file}.json\")));");
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
                            "return new AbilitySystemCellConfig(Array.Empty<int>(), Array.Empty<int>(),Array.Empty<AbilityConfig>(), 0);");
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
                        writer.WriteLine(
                            "return new AbilitySystemCellConfig(data.Tag, data.AttrSet, abilities, data.Level);");
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
                        writer.WriteLine("var cueParam = Activator.CreateInstance(cueParamType) as ICueParameter;");
                        writer.WriteLine("if (cueParam != null)");
                        writer.WriteLine("{");
                        writer.Indent++;

                        var allCue = EditorCueHelper.GetCachedCueTypes();
                        var cueTypes = allCue as Type[] ?? allCue.ToArray();
                        foreach (var cueType in cueTypes)
                        {
                            var cueName = cueType.Name;
                            // 首字母小写，作为变量名
                            var cueFieldName = char.ToLower(cueName[0]) + cueName.Substring(1);
                            var cueParaName = char.ToLower(cueName[0]) + cueName.Substring(1) + "Param";
                            var cueParamType = EditorCueHelper.CueToCueParamTypeMap()[cueName];

                            writer.WriteLine($"if (cueLogic is cfg.{cueName} {cueFieldName})");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine($"var {cueParaName} = cueParam as {cueParamType.FullName};");
                            // 通过类名的字符串，获取类的Type
                            var typeFullName = $"cfg.{cueName}, {cueType.Assembly.GetName().Name}";
                            var tType = Type.GetType(typeFullName);
                            // 获取只读字段
                            var readOnlyFields = EXEditorHelper.GetAllReadOnlyFieldNames(tType);
                            foreach (var fieldName in readOnlyFields)
                            {
                                writer.WriteLine($"{cueParaName}?.Set{fieldName}({cueFieldName}.{fieldName});");
                            }

                            writer.WriteLine($"cueParam = {cueParaName};");
                            writer.Indent--;
                            writer.WriteLine("}");
                        }

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
                        writer.WriteLine("    configs.Add(new ConfEffectGrantedTags { tags = data.GrantedTags.ToArray() });");
                        writer.WriteLine("// applicationRequiredTags");
                        writer.WriteLine("if (data.ApplicationRequiredTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfApplicationRequiredTags { tags = data.ApplicationRequiredTags.ToArray() });");
                        writer.WriteLine("// ongoingRequiredTags");
                        writer.WriteLine("if (data.OngoingRequiredTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfOngoingRequiredTags { tags = data.OngoingRequiredTags.ToArray() });");
                        writer.WriteLine("// removeGameplayEffectsWithTags");
                        writer.WriteLine("if (data.RemoveGameplayEffectsWithTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfRemoveEffectWithTags { tags = data.RemoveGameplayEffectsWithTags.ToArray() });");
                        writer.WriteLine("// immunityTags");
                        writer.WriteLine("if (data.ImmunityTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfEffectImmunityTags { tags = data.ImmunityTags.ToArray() });");
                        
                        writer.WriteLine("// duration");
                        writer.WriteLine("if (data.Duration != null && data.Duration.Time != 0)");
                        writer.Indent++;
                        writer.WriteLine("configs.Add(new ConfDuration");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("duration = data.Duration.Time,");
                        writer.WriteLine("timeUnit = (TimeUnit)data.Duration.TimeUnit,");
                        writer.WriteLine("ResetStartTimeWhenActivated = data.Duration.ResetStartTimeWhenActivated");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;

                        writer.WriteLine("// period");
                        writer.WriteLine("if (data.Period is { Time: > 0 })");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var gameplayEffectSettings = new List<GameplayEffectComponentConfig[]>();");
                        writer.WriteLine("foreach (var effectID in data.Period.Effects)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effect = GetGameplayEffectConfig(effectID);");
                        writer.WriteLine("gameplayEffectSettings.Add(effect.ComponentConfigs);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new ConfPeriod");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("Period = data.Period.Time,");
                        writer.WriteLine("ResetTimeCountWhenDeactivated = data.Period.FirstTrigger,");
                        writer.WriteLine("GameplayEffectSettings = gameplayEffectSettings.ToArray()");
                        writer.Indent--;
                        writer.WriteLine("});");
                        writer.Indent--;
                        writer.WriteLine("}");
                        
                        writer.WriteLine("// modifiers");    
                        writer.WriteLine("if (data.Modifiers != null && data.Modifiers.Count > 0)");    
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("ModifierSetting[] modifierSettings = new ModifierSetting[data.Modifiers.Count];"); 
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
                        writer.WriteLine("AbilityConfig = GetAbilityConfig(info.Id),");
                        writer.WriteLine("ActivationPolicy = (GrantedAbilityActivationPolicy)info.ActivationPolicy,");
                        writer.WriteLine("DeactivationPolicy = (GrantedAbilityDeactivationPolicy)info.DeactivationPolicy,");
                        writer.WriteLine("Level = info.Level,");
                        writer.WriteLine("RemovePolicy = (GrantedAbilityRemovePolicy)info.RemovePolicy,");
                        writer.Indent--;
                        writer.WriteLine("};");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new MCConfGrantedAbility() { GrantedAbilities = grantedAbilities });"); 
                        writer.Indent--;
                        writer.WriteLine("}");
                        
                        writer.WriteLine("// stacking"); 
                        writer.WriteLine("if (data.Stacking.StackCode != 0)"); 
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effectConfigs = new List<GameplayEffectConfig>();"); 
                        writer.WriteLine("foreach (var effectID in data.Stacking.OverflowEffects)"); 
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("var effect = GetGameplayEffectConfig(effectID);"); 
                        writer.WriteLine("effectConfigs.Add(effect);"); 
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("configs.Add(new ConfStacking()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("StackingCode = data.Stacking.StackCode,");
                        writer.WriteLine("StackType = (EffectStackType)data.Stacking.StackingType,");
                        writer.WriteLine("LimitCount = data.Stacking.LimitCount,");
                        writer.WriteLine("EffectDurationRefreshPolicy = (EffectDurationRefreshPolicy)data.Stacking.DurationRefreshPolicy,");
                        writer.WriteLine("EffectPeriodResetPolicy = (EffectPeriodResetPolicy)data.Stacking.PeriodResetPolicy,");
                        writer.WriteLine("EffectExpirationPolicy = (EffectExpirationPolicy)data.Stacking.ExpirationPolicy,");
                        writer.WriteLine("denyOverflowApplication = data.Stacking.DenyOverflowApplication,");
                        writer.WriteLine("clearStackOnOverflow = data.Stacking.ClearStackOnOverflow,");
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


                    #region Ability

                    

                    #endregion
                    
                    #region MMC

                    

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