using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.General;

namespace GAS.Editor
{
    public static class CodeGeneratorLubanPart
    {
        // C# 运行时类型 FullName → Luban cfg 类型转换模板（{0} 为字段访问表达式）    
        private static readonly Dictionary<string, string> LubanTypeConversionMap = new()  
        {  
            ["UnityEngine.Vector3"] = "new UnityEngine.Vector3({0}.X, {0}.Y, {0}.Z)",  
            ["UnityEngine.Vector2"] = "new UnityEngine.Vector2({0}.X, {0}.Y)",  
            ["UnityEngine.Vector4"] = "new UnityEngine.Vector4({0}.X, {0}.Y, {0}.Z, {0}.W)",  
        };

        /// <summary>  
        /// 将字段名首字母大写，匹配 Luban 的 format_property_name PascalCase 规则  
        /// </summary>  
        private static string ToPascalCase(string name)  
        {  
            if (string.IsNullOrEmpty(name)) return name;  
            return char.ToUpperInvariant(name[0]) + name.Substring(1);  
        }
        
        private static void WriteFieldAssignment(
            IndentedWriter writer,
            string paramVar,
            string setter, // BeanFieldAttribute.Setter  
            string dataAccessExpr,
            Type fieldType)
        {
            var typeName = fieldType.FullName ?? fieldType.Name;
            if (LubanTypeConversionMap.TryGetValue(typeName, out var template))
            {
                var converted = string.Format(template, dataAccessExpr);
                writer.WriteLine($"{paramVar}?.{setter}({converted});");
            }
            else
            {
                writer.WriteLine($"{paramVar}?.{setter}({dataAccessExpr});");
            }
        }

        /// <summary>  
        /// 生成多态 Bean 的拆解代码（通用逻辑）  
        /// 包括: 设置 TypeName、创建 Param 实例、switch-case 逐子类赋值、设置 Param  
        /// </summary>  
        private static void WritePolymorphicFieldAssignment(
            IndentedWriter writer,
            string paramVar, // 宿主变量名, e.g. "tp"  
            string dataAccessExpr, // 数据访问路径, e.g. "taskData.Param.CueLogic"  
            EXEditorHelper.BeanPolymorphicFieldInfo polyInfo,
            IEnumerable<Type> subtypes, // 多态子类列表  
            Func<string, Type> getParamTypeByName) // subtypeName → runtime XParam type  
        {
            writer.WriteLine($"// [BeanPolymorphicField] {ToPascalCase(polyInfo.BeanFieldName)}");
            writer.WriteLine($"var polyBean = {dataAccessExpr};");
            writer.WriteLine($"{paramVar}?.{polyInfo.TypeSetter}(polyBean.GetType().Name);");
            writer.WriteLine($"var resolvedParamType = {polyInfo.ParamTypeResolver}(polyBean.GetType().Name);");
            writer.WriteLine("var resolvedParam = Activator.CreateInstance(resolvedParamType) as XParam;");
            writer.WriteLine("if (resolvedParam != null)");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("switch (polyBean)");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var subtype in subtypes)
                    {
                        var subtypeName = subtype.Name;
                        var runtimeParamType = getParamTypeByName(subtypeName);

                        // 检查 cfg 侧是否有 Param 成员  
                        var tType = ReflectionHelper.GetMemberType($"cfg.{subtypeName}", "Param");
                        if (tType == null) continue;

                        writer.WriteLine($"case cfg.{subtypeName} pData:");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine($"var rp = resolvedParam as {runtimeParamType.FullName};");

                        var beanFields = EXEditorHelper.GetBeanFields(runtimeParamType);
                        foreach (var bf in beanFields)
                            WriteFieldAssignment(writer, "rp", bf.Setter, $"pData.Param.{ToPascalCase(bf.Name)}", bf.MemberType);

                        writer.WriteLine("resolvedParam = rp;");
                        writer.WriteLine("break;");
                        writer.Indent--;
                        writer.WriteLine("}");
                    }

                    writer.WriteLine("default:");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine(
                        $"Debug.LogError($\"[XLuban] Unknown {ToPascalCase(polyInfo.BeanFieldName)} type: {{polyBean.GetType().Name}}\");");
                    writer.WriteLine("break;");
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
            writer.WriteLine($"{paramVar}?.{polyInfo.ParamSetter}(resolvedParam);");
        }


        private static (IEnumerable<Type> subtypes, Func<string, Type> getParamType)
            GetPolymorphicHelperInfo(string helperCategory)
        {
            switch (helperCategory)
            {
                case "Cue":
                    return (
                        EditorCueHelper.GetCachedCueTypes(),
                        name => EditorCueHelper.CueToCueParamTypeMap()[name]
                    );
                case "TargetCatcher":
                    return (
                        EditorTargetCatcherHelper.GetCachedTargetCatcherTypes(),
                        name => EditorTargetCatcherHelper.CatcherToParamTypeMap()[name]
                    );
                default:
                    throw new ArgumentException($"Unknown HelperCategory: {helperCategory}");
            }
        }

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
            writer.WriteLine("using System.Linq;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XLuban");
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

                    writer.WriteLine("public static void LoadTablesForEditor()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine(
                        "_tables = new Tables(file => JSON.Parse(File.ReadAllText($\"{GAME_CONF_DIR}/{file}.json\")));");
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

                                // 标准 BeanField 赋值  
                                var beanFields = EXEditorHelper.GetBeanFields(cueParamType);
                                foreach (var bf in beanFields)
                                    WriteFieldAssignment(writer, "cp", bf.Setter, $"cData.Param.{ToPascalCase(bf.Name)}",
                                        bf.MemberType);

                                // 多态 BeanPolymorphicField 赋值  
                                var polyFields = EXEditorHelper.GetBeanPolymorphicFields(cueParamType);
                                foreach (var pf in polyFields)
                                {
                                    var (subtypes, getParamType) = GetPolymorphicHelperInfo(pf.HelperCategory);
                                    WritePolymorphicFieldAssignment(writer, "cp", $"cData.Param.{pf.BeanFieldName}", pf,
                                        subtypes, getParamType);
                                }

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
                        writer.WriteLine("(int[] all, int[] any, int[] none) ParseTagRequirement(cfg.TagRequirementData? requirement)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (requirement == null) return (Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());");
                        writer.WriteLine("var r = requirement.Value;");
                        writer.WriteLine("var all = r.All?.Where(x => x > 0).ToArray() ?? Array.Empty<int>();");
                        writer.WriteLine("var any = r.Any?.Where(x => x > 0).ToArray() ?? Array.Empty<int>();");
                        writer.WriteLine("var none = r.None?.Where(x => x > 0).ToArray() ?? Array.Empty<int>();");
                        writer.WriteLine("return (all, any, none);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("var requiredTag = ParseTagRequirement(data.RequiredTag);");
                        writer.WriteLine("var immunityTag = ParseTagRequirement(data.ImmunityTag);");
                        writer.WriteLine(
                            "return new GameplayCueConfig(cueType, cueParam, requiredTag.all, requiredTag.any, requiredTag.none, immunityTag.all, immunityTag.any, immunityTag.none);");
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
                        writer.WriteLine("(int[] all, int[] any, int[] none)? ParseTagRequirement(cfg.TagRequirementData requirement)");
                        writer.WriteLine("{");
                        writer.WriteLine("");
                        writer.WriteLine("    int[] all = null, any = null, none = null;");
                        writer.WriteLine("    if(requirement.All is {Count: > 0})");
                        writer.WriteLine("        all = requirement.All.Where(x => x > 0).ToArray();");
                        writer.WriteLine("    if(requirement.Any is {Count: > 0})");
                        writer.WriteLine("        any = requirement.Any.Where(x => x > 0).ToArray();");
                        writer.WriteLine("    if(requirement.None is {Count: > 0})");
                        writer.WriteLine("        none = requirement.None.Where(x => x > 0).ToArray();");
                        writer.WriteLine("");
                        writer.WriteLine("    if(all != null && all.Length == 0) all = null;");
                        writer.WriteLine("    if(any != null && any.Length == 0) any = null;");
                        writer.WriteLine("    if(none != null && none.Length == 0) none = null;");
                        writer.WriteLine("");
                        writer.WriteLine("    if(all == null && any == null && none == null) return null;");
                        writer.WriteLine("    return (all, any, none);");
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("// assetTags");
                        writer.WriteLine("if (data.AssetTags is { Count: > 0 })");
                        writer.WriteLine("    configs.Add(new ConfAssetTags { tags = data.AssetTags.ToArray() });");
                        writer.WriteLine("// grantedTags");
                        writer.WriteLine("if (data.GrantedTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfEffectGrantedTags { tags = data.GrantedTags.ToArray() });");
                        writer.WriteLine("// application tags condition");
                        writer.WriteLine("if (data.ApplicationRequiredTags != null)");
                        writer.WriteLine("{");
                        writer.WriteLine("    var result = ParseTagRequirement(data.ApplicationRequiredTags.Value);");
                        writer.WriteLine("    if(result != null)");
                        writer.WriteLine("        configs.Add(new ConfApplicationRequiredTags{ all = result.Value.all, any = result.Value.any, none = result.Value.none });");
                        writer.WriteLine("}");
                        writer.WriteLine("// ongoing tags condition");
                        writer.WriteLine("if (data.OngoingRequiredTags != null)");
                        writer.WriteLine("{");
                        writer.WriteLine("    var result = ParseTagRequirement(data.OngoingRequiredTags.Value);");
                        writer.WriteLine("    if(result != null)");
                        writer.WriteLine("        configs.Add(new ConfOngoingRequiredTags{ all = result.Value.all, any = result.Value.any, none = result.Value.none });");
                        writer.WriteLine("}");
                        writer.WriteLine("// remove-effect tags condition");
                        writer.WriteLine("if (data.RemoveGameplayEffectsWithTags != null)");
                        writer.WriteLine("{");
                        writer.WriteLine("    var result = ParseTagRequirement(data.RemoveGameplayEffectsWithTags.Value);");
                        writer.WriteLine("    if(result != null)");
                        writer.WriteLine("        configs.Add(new ConfRemoveEffectWithTags{ all = result.Value.all, any = result.Value.any, none = result.Value.none });");
                        writer.WriteLine("}");
                        writer.WriteLine("// immunity tags condition");
                        writer.WriteLine("if (data.ImmunityTags != null)");
                        writer.WriteLine("{");
                        writer.WriteLine("    var result = ParseTagRequirement(data.ImmunityTags.Value);");
                        writer.WriteLine("    if(result != null)");
                        writer.WriteLine("        configs.Add(new ConfEffectImmunityTags{ all = result.Value.all, any = result.Value.any, none = result.Value.none });");
                        writer.WriteLine("}");

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

                        writer.WriteLine("(int[] all, int[] any, int[] none)? ParseTagRequirement(cfg.TagRequirementData? requirement)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (requirement == null) return null;");
                        writer.WriteLine("var r = requirement.Value;");
                        writer.WriteLine("int[] all = null, any = null, none = null;");
                        writer.WriteLine("if (r.All is { Count: > 0 }) all = r.All.Where(x => x > 0).ToArray();");
                        writer.WriteLine("if (r.Any is { Count: > 0 }) any = r.Any.Where(x => x > 0).ToArray();");
                        writer.WriteLine("if (r.None is { Count: > 0 }) none = r.None.Where(x => x > 0).ToArray();");
                        writer.WriteLine("if (all == null && any == null && none == null) return null;");
                        writer.WriteLine("return (all, any, none);");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");
                        writer.WriteLine("int[] PickSimpleTagSet((int[] all, int[] any, int[] none) req)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (req.any is { Length: > 0 }) return req.any;");
                        writer.WriteLine("if (req.all is { Length: > 0 }) return req.all;");
                        writer.WriteLine("if (req.none is { Length: > 0 }) return req.none;");
                        writer.WriteLine("return Array.Empty<int>();");
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("");

                        writer.WriteLine("// cancelAbilityWithTags");
                        writer.WriteLine("var cancelTags = ParseTagRequirement(data.CancelAbilityWithTags);");
                        writer.WriteLine("if (cancelTags != null)");
                        writer.WriteLine(
                            "    configs.Add(new ConfCancelAbilityWithTags { tags = PickSimpleTagSet(cancelTags.Value) });");

                        writer.WriteLine("// blockAbilityWithTags");
                        writer.WriteLine("var blockTags = ParseTagRequirement(data.BlockAbilityWithTags);");
                        writer.WriteLine("if (blockTags != null)");
                        writer.WriteLine(
                            "    configs.Add(new ConfBlockAbilityWithTags { tags = PickSimpleTagSet(blockTags.Value) });");

                        writer.WriteLine("// activationOwnedTags");
                        writer.WriteLine("if (data.ActivationOwnedTags is { Count: > 0 })");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationOwnedTags { tags = data.ActivationOwnedTags.ToArray() });");

                        writer.WriteLine("// activationRequiredTags");
                        writer.WriteLine("var activationRequiredTags = ParseTagRequirement(data.ActivationRequiredTags);");
                        writer.WriteLine("if (activationRequiredTags != null)");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationRequiredTags { all = activationRequiredTags.Value.all, any = activationRequiredTags.Value.any, none = activationRequiredTags.Value.none });");

                        writer.WriteLine("// activationBlockedTags");
                        writer.WriteLine("var activationBlockedTags = ParseTagRequirement(data.ActivationBlockedTags);");
                        writer.WriteLine("if (activationBlockedTags != null)");
                        writer.WriteLine(
                            "    configs.Add(new ConfAbilityActivationBlockedTags { all = activationBlockedTags.Value.all, any = activationBlockedTags.Value.any, none = activationBlockedTags.Value.none });");

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

                                    // 标准 BeanField 赋值  
                                    var beanFields = EXEditorHelper.GetBeanFields(abilityParamType);
                                    foreach (var bf in beanFields)
                                        WriteFieldAssignment(writer, "ap", bf.Setter, $"aData.Param.{ToPascalCase(bf.Name)}",
                                            bf.MemberType);

                                    // 多态 BeanPolymorphicField 赋值  
                                    var polyFields = EXEditorHelper.GetBeanPolymorphicFields(abilityParamType);
                                    foreach (var pf in polyFields)
                                    {
                                        var (subtypes, getParamType) = GetPolymorphicHelperInfo(pf.HelperCategory);
                                        WritePolymorphicFieldAssignment(writer, "ap", $"aData.Param.{pf.BeanFieldName}",
                                            pf, subtypes, getParamType);
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
                            "XParam mmcParam = Activator.CreateInstance(mmcLogicParamType) as XParam;");
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

                                // 标准 BeanField 赋值  
                                var beanFields = EXEditorHelper.GetBeanFields(mmcParamType);
                                foreach (var bf in beanFields)
                                    WriteFieldAssignment(writer, "mp", bf.Setter, $"mmcData.Param.{ToPascalCase(bf.Name)}",
                                        bf.MemberType);

                                // 多态 BeanPolymorphicField 赋值  
                                var polyFields = EXEditorHelper.GetBeanPolymorphicFields(mmcParamType);
                                foreach (var pf in polyFields)
                                {
                                    var (subtypes, getParamType) = GetPolymorphicHelperInfo(pf.HelperCategory);
                                    WritePolymorphicFieldAssignment(writer, "mp", $"mmcData.Param.{pf.BeanFieldName}",
                                        pf, subtypes, getParamType);
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
                            var taskParamType =
                                EditorAbilityHelper.AbilityTaskToAbilityTaskParamTypeMap()[taskTypeName];
                            var tType = ReflectionHelper.GetMemberType($"cfg.{taskTypeName}", "Param");
                            if (tType == null) continue;

                            writer.WriteLine($"case cfg.{taskTypeName} taskData:");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine($"var tp = taskParam as {taskParamType.FullName};");

                            // 标准 BeanField 赋值  
                            var beanFields = EXEditorHelper.GetBeanFields(taskParamType);
                            foreach (var bf in beanFields)
                                WriteFieldAssignment(writer, "tp", bf.Setter, $"taskData.Param.{ToPascalCase(bf.Name)}",
                                    bf.MemberType);

                            // 多态 BeanPolymorphicField 赋值  
                            var polyFields = EXEditorHelper.GetBeanPolymorphicFields(taskParamType);
                            foreach (var pf in polyFields)
                            {
                                var (subtypes, getParamType) = GetPolymorphicHelperInfo(pf.HelperCategory);
                                WritePolymorphicFieldAssignment(writer, "tp", $"taskData.Param.{ToPascalCase(pf.BeanFieldName)}", pf,
                                    subtypes, getParamType);
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
