using System;
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
                // 刷新资源
                AssetDatabase.Refresh();
            }
        }
        
        public static string MakeTagValidIdentifier(string name)
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
            CodeGeneratorTagPart.GenerateTag();
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
                
                writer.WriteLine("private static Dictionary<int, AttrSetConfig> _attributeSetMap = new Dictionary<int, AttrSetConfig>();");
                writer.WriteLine("");
                writer.WriteLine("public static Dictionary<int, AttrSetConfig> AttributeSetMap");
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
                            writer.WriteLine("settings[i] = new AttributeBaseSetting(a.ID, a.InitValue, a.UseMinValue,a.UseMaxValue, a.MinValue, a.MaxValue);");
                            writer.Indent--;
                            writer.WriteLine("}");
                            writer.WriteLine("_attributeSetMap.Add(attrSet.ID,new AttrSetConfig(attrSet.ID,settings));");
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
                    var allMmc = EditorMmcHelper.GetCachedMmcTypes();
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
        
        [MenuItem("EXTool/EX-GAS/生成脚本/LubanExtension")]
        public static void GenerateLubanExtension()
        {
            CodeGeneratorLubanPart.GenerateLubanExtension();
        }
        
        [MenuItem("EXTool/EX-GAS/生成脚本/Launcher")]
        public static void GenerateLauncher()
        {
            // namespace GAS.Runtime
            // {
            //     public static class XLauncher
            //     {
            //         public static void InitCache()
            //         {
            //             XAbility.LoadAbilityCode();
            //             XMmc.LoadMmcType();
            //             XCue.LoadCueType();
            //             XLuban.Init();
            //         }
            //       
            //         public static void Launch()
            //         {
            //             InitCache();
            //             GASManager.Initialize();
            //         
            //             // 初始化Tag系统
            //             // 注意需要在GASManager.Initialize()之后调用
            //             // 因为XTag创建全Tag的图鉴单例来作为运行时缓存，需要EntityManager。
            //             XTag.InitTagList();
            //         }
            //     }
            // }
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeLauncher;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("");
            writer.WriteLine("using System;");
            writer.WriteLine("");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XLauncher");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("public static void InitCache()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("XAbility.LoadAbilityCode();");
                        writer.WriteLine("XMmc.LoadMmcType();");
                        writer.WriteLine("XCue.LoadCueType();");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");
                    
                    writer.WriteLine("public static void InitConfigTables(Func<string, SimpleJSON.JSONNode> loader)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("XLuban.Init(loader);");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    
                    writer.WriteLine("public static void Launch()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("InitCache();");
                        writer.WriteLine("GASManager.Initialize();");
                        writer.WriteLine("");
                        writer.WriteLine("// 初始化Tag系统");
                        writer.WriteLine("// 注意需要在GASManager.Initialize()之后调用");
                        writer.WriteLine("// 因为XTag创建全Tag的图鉴单例来作为运行时缓存，需要EntityManager。");
                        writer.WriteLine("XTag.InitTagList();");
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
        
        /// <summary>
        ///  生成所有GAS相关代码
        /// </summary>
        [MenuItem("EXTool/EX-GAS/生成脚本/生成所有")]
        public static void GenerateAllCode()
        {
            // 先更新Bean定义（因为其他代码生成可能依赖它）
            BeanUpdater.UpdateBeans();

            GenerateTagCode();
            GenerateAttrCode();
            GenerateAttrSetCode();
            GenerateAbilityCode();
            GenerateCueCode();
            GenerateMmcCode();
            GenerateLubanExtension();
            GenerateLauncher();
        }
    }
}