using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GAS.Editor
{
    public static class CodeGenerator
    {
        [MenuItem("EXTool/EX-GAS/生成脚本/GAS表配置",priority = 0)]
        public static void GenerateGasConfigTables()
        {
            var instance = GASSettingAsset.Instance;
            string fullBatPath = Path.GetFullPath(instance.FullGenBatPath());
            string fullOutputPath = Path.GetFullPath(instance.TableOutpuPath);
            string fullCodeOutputPath = Path.GetFullPath(instance.TableClassCodeOutpuPath);
            // 验证文件和输出路径是否存在
            if (!File.Exists(fullBatPath))
            {
                Debug.LogError($"BAT文件不存在: {fullBatPath}");
                return;
            }
            if (!Directory.Exists(fullOutputPath))
            {
                Debug.LogError($"输出路径不存在: {fullOutputPath}");
                return;
            }
            if (!Directory.Exists(fullCodeOutputPath))
            {
                Debug.LogError($"表类Class输出路径不存在: {fullCodeOutputPath}");
                return;
            }

            // 获取bat文件的文件夹路径
            string fullBuildPath = Path.GetDirectoryName(fullBatPath);
            // 创建进程配置
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = fullBatPath,
                WorkingDirectory = fullBuildPath,
                Arguments = $"\"{fullOutputPath}\" \"{fullCodeOutputPath}\"",  // 用引号包裹路径防空格问题
                UseShellExecute = false,              // 不使用系统shell
                RedirectStandardOutput = true,        // 重定向输出
                RedirectStandardError = true,         // 重定向错误
                CreateNoWindow = false                // 不创建窗口
            };

            // 注册输出事件
            process.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) 
                    Debug.Log(e.Data);
            };
        
            process.ErrorDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) 
                    Debug.LogError(e.Data);
            };

            try
            {
                // 启动进程
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(); // 等待执行完成
                Debug.Log($"BAT执行完成，退出代码: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"执行错误: {ex.Message}");
            }
            finally
            {
                process.Close();
            }
        }
        
        private static string MakeTagValidIdentifier(string name)
        {
            // Replace '.' with '_'
            name = name.Replace('.', '_');

            // If starts with a digit, add '_' at the beginning
            if (char.IsDigit(name[0])) name = "_" + name;

            // Ensure the identifier is valid
            return string.Join("",
                name.Split(
                    new[]
                    {
                        ' ', '-', '.', ':', ',', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}',
                        '/', '\\', '|'
                    }, StringSplitOptions.RemoveEmptyEntries));
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/GameplayTag")]
        public static void GenerateTagCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var tagJsonFilePath = setting.PathOfJsonTag;
            // 检查文件是否存在
            if (!File.Exists(tagJsonFilePath))
            {
                Debug.LogError($"JSON file not found at {tagJsonFilePath}");
                return;
            }
            var tagJsonText = File.ReadAllText(tagJsonFilePath);
            var tags = GasJsonReader.ReadTags(tagJsonText);

            var tagNameToIdMap = tags.ToDictionary(t => MakeTagValidIdentifier(t.name), t => t.id);
            var tagIdToNameMap = tags.ToDictionary(t => t.id, t => MakeTagValidIdentifier(t.name));

            // 预处理，获取所有Tag的父子关系
            // 父节点
            Dictionary<int, List<string>> parentMap = new Dictionary<int, List<string>>();
            foreach (var tag in tags)
            {
                var tagName = MakeTagValidIdentifier(tag.name);
                var parentTagNames = new List<string>();
                string[] parentTagNamesArray = tagName.Split('_');
                string tempParentName = "";
                for (var i = 0; i < parentTagNamesArray.Length - 1; i++)
                {
                    var parentName = parentTagNamesArray[i];
                    tempParentName += parentName;
                    parentTagNames.Add(tempParentName);
                    tempParentName += "_";
                }
                
                parentMap.Add(tag.id, parentTagNames);
            }
            // 子节点
            Dictionary<int, List<string>> childMap = new Dictionary<int, List<string>>();
            foreach (var tag in tags) childMap.Add(tag.id,new List<string>());

            foreach (var kp in parentMap)
            {
                var parents = kp.Value;
                foreach (var pName in parents)
                {
                    var pID = tagNameToIdMap[pName];
                    foreach (var tag in tags)
                    {
                        if (MakeTagValidIdentifier(tag.name) == pName)
                        {
                            childMap[pID].Add(tagIdToNameMap[kp.Key]);
                        }
                    }
                }
            }

            var filePath = setting.PathOfCodeTag;

            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using GAS.RuntimeWithECS.Tag;");
            writer.WriteLine("");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XTag");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allTagAsset = tags;
                    foreach (var tag in tags)
                    {
                        writer.WriteLine(
                            $"public const int {MakeTagValidIdentifier(tag.name)} = {tag.id};");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void InitTagList()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("GTagUtil.InitTagMap(new Dictionary<int, GameplayTag>()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var tagInEditor in allTagAsset)
                            {
                                var tag = tagInEditor.id;
                                var tagName = MakeTagValidIdentifier(tagInEditor.name);
                                writer.WriteLine(
                                    $"{{ {tagName}, " +
                                    $"new GameplayTag({tagName}, " +
                                    $"new int[] {{ {string.Join(", ", parentMap[tag])} }}, " +
                                    $"new int[] {{ {string.Join(", ", childMap[tag])} }}) }},");
                            }
                        }

                        writer.Indent--;
                        writer.WriteLine("},");

                        writer.WriteLine("new Dictionary<int, string>()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var tagAsset in allTagAsset)
                            {
                                var tagName = MakeTagValidIdentifier(tagAsset.name);
                                writer.WriteLine(
                                    $"{{ {tagName}, \"{tagAsset.name}\" }},");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine(");");
                    }

                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/Attribute")]
        public static void GenerateAttrCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var attrJsonFilePath = setting.PathOfJsonAttr;
            // 检查文件是否存在
            if (!File.Exists(attrJsonFilePath))
            {
                Debug.LogError($"JSON file not found at {attrJsonFilePath}");
                return;
            }
            var tagJsonText = File.ReadAllText(attrJsonFilePath);
            var attrs = GasJsonReader.ReadAttributes(tagJsonText);
            var filePath = setting.PathOfCodeAttr;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XAttribute");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var attr in attrs)
                    {
                        writer.WriteLine(
                            $"public const int {MakeTagValidIdentifier(attr.name)} = {attr.id};");
                    }
                }

                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/AttributeSet")]
        public static void GenerateAttrSetCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var attrSetJsonFilePath = setting.PathOfJsonAttrSet;
            // 检查文件是否存在
            if (!File.Exists(attrSetJsonFilePath))
            {
                Debug.LogError($"JSON file not found at {attrSetJsonFilePath}");
                return;
            }
            var attrSetJsonText = File.ReadAllText(attrSetJsonFilePath);
            var attrSets = GasJsonReader.ReadAttributeSets(attrSetJsonText);
            var filePath = setting.PathOfCodeAttrSet;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using GAS.RuntimeWithECS.Attribute;");
            writer.WriteLine("");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XAttrSet");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var attrSet in attrSets)
                    {
                        writer.WriteLine(
                            $"public const int {MakeTagValidIdentifier(attrSet.name)} = {attrSet.id};");
                    }
                }
                
                writer.Indent--;
                writer.WriteLine("");
                
                writer.Indent++;
                {
                    foreach (var attrSet in attrSets)
                    {
                        writer.WriteLine("");
                        writer.WriteLine(
                            $"public class AS_{MakeTagValidIdentifier(attrSet.name)}");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var attr in attrSet.attribute)
                                writer.WriteLine(
                                    $"public const int {MakeTagValidIdentifier(attr.GetAttrName())} = {attr.id};");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                }
                writer.WriteLine("");
                
                writer.WriteLine("private static Dictionary<int, NewAttributeSetConfig> _attributeSetMap = new Dictionary<int, NewAttributeSetConfig>();");
                writer.WriteLine("");
                writer.WriteLine("public static Dictionary<int, NewAttributeSetConfig> AttributeSetMap");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("get");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("if (_attributeSetMap.Count == 0)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("var datas = XLuban.Tables.TbattributeSet.DataList;");
                            writer.WriteLine("foreach (var attrSet in datas)");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine("var settings = new AttributeBaseSetting[attrSet.Attribute.Length];");
                            writer.WriteLine("for (var i = 0; i < attrSet.Attribute.Length; i++)");
                            writer.WriteLine("{");
                            writer.Indent++;
                            writer.WriteLine("var a = attrSet.Attribute[i];");
                            writer.WriteLine("settings[i] = new AttributeBaseSetting(a.Id, a.InitValue, a.UseMinValue,a.UseMaxValue, a.MinValue, a.MaxValue);");
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine("_attributeSetMap.Add(attrSet.Id,new NewAttributeSetConfig(attrSet.Id,settings));");
                            writer.Indent--;
                            writer.WriteLine("}");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("return _attributeSetMap;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
                
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/Ability")]
        public static void GenerateAbilityCode()
        {
            CodeGeneratorAbilityPart.GenerateAbilityCode();
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/GameplayCue")]
        public static void GenerateCueCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeCue;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XCue");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allCue = EditorCueHelper.GetCachedCueTypes();
                    var cueTypes = allCue as Type[] ?? allCue.ToArray();
                    foreach (var cueType in cueTypes)
                    {
                        var cueName = cueType.Name;
                        writer.WriteLine($"public const string CUE_{cueName} = \"{cueName}\";");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void LoadCueType()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var cueType in cueTypes)
                        {
                            var cueName = cueType.Name;
                            var typeFullName = cueType.FullName;
                            var cueParamType = EditorCueHelper.CueToCueParamTypeMap()[cueName];
                            var cueParamTypeFullName = cueParamType.FullName;
                            writer.WriteLine($"var {cueName} = typeof({typeFullName});");
                            writer.WriteLine($"CueHelper.RegisterCue(CUE_{cueName}, {cueName}, typeof({cueParamTypeFullName}));");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated CueCode at path: {filePath}");
            AssetDatabase.Refresh();
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/ModMagnitudeCalculation")]
        public static void GenerateMmcCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeMmc;
           using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XMmc");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allMmc = EditGameplayEffectHelper.GetCachedMmcTypes();
                    var mmcTypes = allMmc as Type[] ?? allMmc.ToArray();
                    foreach (var mmcType in mmcTypes)
                    {
                        var mmcName = mmcType.Name;
                        writer.WriteLine($"public const string MMC_{mmcName} = \"{mmcName}\";");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void LoadMmcType()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var mmcType in mmcTypes)
                        {
                            var mmcName = mmcType.Name;
                            var typeFullName = mmcType.FullName;
                            var mmcParaName = EditorMmcHelper.MmcToMmcParamTypeMap()[mmcName];
                            writer.WriteLine($"var {mmcName} = typeof({typeFullName});");
                            writer.WriteLine($"MmcHelper.RegisterMmc(MMC_{mmcName}, {mmcName},typeof({mmcParaName.FullName}));");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated MmcCode at path: {filePath}");
            AssetDatabase.Refresh();
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/配置表拓展类")]
        public static void GenerateLubanExtension()
        {
            CodeGeneratorLubanPart.GenerateLubanExtension();
        }
        
        /// <summary>
        ///  生成所有GAS相关代码
        /// </summary>
        [MenuItem("EXTool/EX-GAS/生成脚本/生成所有")]
        public static void GenerateAllCode()
        {
            GenerateTagCode();
            GenerateAttrCode();
            GenerateAttrSetCode();
            GenerateAbilityCode();
            GenerateCueCode();
            GenerateMmcCode();
            GenerateLubanExtension();
        }
    }
}