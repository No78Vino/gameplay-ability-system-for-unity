using System;
using System.IO;
using System.Linq;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Helper;

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
                        writer.WriteLine("Debug.LogError($\"Cue_ID:{id}  CueType:{data.CueLogic.GetType().Name} 不存在.\");");
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
                        writer.WriteLine("return new GameplayCueConfig(cueType, cueParam, data.RequiredTag.ToArray(), data.ImmunityTag.ToArray());");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    #endregion
                    
                    writer.WriteLine("");

                    #region GameplayEffect

                    

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