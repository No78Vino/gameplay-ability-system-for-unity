using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GAS.Runtime;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    /// 自动化更新 __beans__.xlsx 文件
    /// 扫描自定义类（Cue、MMC、AbilityLogic、AbilityTask）及其参数类型
    /// 自动生成/更新对应的Luban Bean定义
    /// </summary>
    public static class BeanUpdater
    {
        private const string BEANS_XLSX_NAME = "__beans__.xlsx";

        #region 数据结构

        /// <summary>
        /// Bean定义信息
        /// </summary>
        private class BeanDefinition
        {
            public string Name;                           // Bean名称
            public string Parent;                         // 父类Bean名称
            public string Comment;                        // 注释
            public List<BeanField> Fields = new();        // 字段列表
            public bool IsAbstract;                       // 是否抽象类
        }

        /// <summary>
        /// Bean字段定义
        /// </summary>
        private class BeanField
        {
            public string Name;           // 字段名
            public string Type;           // 字段类型
            public string Comment;        // 注释
            public bool IsList;           // 是否数组
        }

        #endregion

        #region 主入口

        [MenuItem("EXTool/EX-GAS/生成脚本/更新Bean定义")]
        public static void UpdateBeans()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var beansPath = Path.Combine(setting.ConfigProjectPath, "Datas", BEANS_XLSX_NAME);

            if (!File.Exists(beansPath))
            {
                EditorUtility.DisplayDialog("错误", $"__beans__.xlsx 文件不存在:\n{beansPath}", "确定");
                return;
            }

            // 收集所有Bean定义
            var beans = CollectAllBeans();

            // 生成Python脚本并执行
            GenerateAndUpdateBeans(beansPath, beans);

            Debug.Log($"[BeanUpdater] Bean定义更新完成，共 {beans.Count} 个Bean");
        }

        #endregion

        #region 收集Bean定义

        private static List<BeanDefinition> CollectAllBeans()
        {
            var beans = new List<BeanDefinition>();

            // 1. 收集XParam参数类
            CollectXParamBeans(beans);

            // 2. 收集Cue逻辑类
            CollectCueBeans(beans);

            // 3. 收集MMC逻辑类
            CollectMmcBeans(beans);

            // 4. 收集AbilityLogic逻辑类
            CollectAbilityLogicBeans(beans);

            // 5. 收集AbilityTask类
            CollectAbilityTaskBeans(beans);

            // 6. 收集TargetCatcher类
            CollectTargetCatcherBeans(beans);

            return beans;
        }

        /// <summary>
        /// 收集XParam参数类Bean定义
        /// </summary>
        private static void CollectXParamBeans(List<BeanDefinition> beans)
        {
            var xParamTypes = GetTypesImplementingInterface(typeof(XParam));

            foreach (var type in xParamTypes)
            {
                if (type.IsAbstract) continue;

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "XParam",
                    Comment = GetXmlComment(type) ?? $"参数类型: {type.Name}",
                    IsAbstract = false
                };

                // 收集字段
                CollectFieldsFromType(type, bean);

                beans.Add(bean);
            }

            // 添加XParam抽象基类
            beans.Insert(0, new BeanDefinition
            {
                Name = "XParam",
                Parent = "",
                Comment = "EX-GAS内部通用的参数类型，所有自定义泛型类的参数都需要继承自XParam",
                IsAbstract = true
            });
        }

        /// <summary>
        /// 收集Cue逻辑类Bean定义
        /// </summary>
        private static void CollectCueBeans(List<BeanDefinition> beans)
        {
            var cueTypes = GetTypesInheritingFrom(typeof(GameplayCueBase));

            // 添加抽象基类
            beans.Add(new BeanDefinition
            {
                Name = "CueLogic",
                Parent = "",
                Comment = "Cue逻辑基类",
                IsAbstract = true
            });

            foreach (var type in cueTypes)
            {
                if (type.IsAbstract) continue;

                var paramType = GetGenericParamType(type, typeof(GameplayCueBase<>));
                if (paramType == null) continue;

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "CueLogic",
                    Comment = GetXmlComment(type) ?? $"Cue: {type.Name}",
                    IsAbstract = false
                };

                bean.Fields.Add(new BeanField
                {
                    Name = "Param",
                    Type = paramType.Name,
                    Comment = "参数"
                });

                beans.Add(bean);
            }
        }

        /// <summary>
        /// 收集MMC逻辑类Bean定义
        /// </summary>
        private static void CollectMmcBeans(List<BeanDefinition> beans)
        {
            var mmcTypes = GetTypesInheritingFrom(typeof(ModMagnitudeCalculationBase));

            // 添加抽象基类
            beans.Add(new BeanDefinition
            {
                Name = "MmcLogic",
                Parent = "",
                Comment = "MMC逻辑基类",
                IsAbstract = true
            });

            foreach (var type in mmcTypes)
            {
                if (type.IsAbstract) continue;

                var paramType = GetGenericParamType(type, typeof(ModMagnitudeCalculationBase<>));
                if (paramType == null) continue;

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "MmcLogic",
                    Comment = GetXmlComment(type) ?? $"MMC: {type.Name}",
                    IsAbstract = false
                };

                bean.Fields.Add(new BeanField
                {
                    Name = "Param",
                    Type = paramType.Name,
                    Comment = "参数"
                });

                beans.Add(bean);
            }
        }

        /// <summary>
        /// 收集AbilityLogic逻辑类Bean定义
        /// </summary>
        private static void CollectAbilityLogicBeans(List<BeanDefinition> beans)
        {
            var abilityTypes = GetTypesInheritingFrom(typeof(AbilityLogicBase));

            // 添加抽象基类
            beans.Add(new BeanDefinition
            {
                Name = "AbilityLogic",
                Parent = "",
                Comment = "Ability逻辑基类",
                IsAbstract = true
            });

            foreach (var type in abilityTypes)
            {
                if (type.IsAbstract) continue;

                var paramType = GetGenericParamType(type, typeof(AbilityLogicBase<>));
                if (paramType == null) continue;

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "AbilityLogic",
                    Comment = GetXmlComment(type) ?? $"Ability: {type.Name}",
                    IsAbstract = false
                };

                bean.Fields.Add(new BeanField
                {
                    Name = "Param",
                    Type = paramType.Name,
                    Comment = "参数"
                });

                beans.Add(bean);
            }
        }

        /// <summary>
        /// 收集AbilityTask类Bean定义
        /// </summary>
        private static void CollectAbilityTaskBeans(List<BeanDefinition> beans)
        {
            var taskTypes = GetTypesInheritingFrom(typeof(AbilityTaskBase));

            // 添加抽象基类
            beans.Add(new BeanDefinition
            {
                Name = "AbilityTask",
                Parent = "",
                Comment = "AbilityTask基类",
                IsAbstract = true
            });

            foreach (var type in taskTypes)
            {
                if (type.IsAbstract) continue;

                var paramType = GetGenericParamType(type, typeof(AbilityTaskBase<>));

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "AbilityTask",
                    Comment = GetXmlComment(type) ?? $"Task: {type.Name}",
                    IsAbstract = false
                };

                if (paramType != null)
                {
                    bean.Fields.Add(new BeanField
                    {
                        Name = "Param",
                        Type = paramType.Name,
                        Comment = "参数"
                    });
                }

                beans.Add(bean);
            }
        }

        /// <summary>
        /// 收集TargetCatcher类Bean定义
        /// </summary>
        private static void CollectTargetCatcherBeans(List<BeanDefinition> beans)
        {
            // 查找TargetCatcherBase类型
            var catcherBaseType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == "TargetCatcherBase" || t.Name == "TargetCatcher`1");

            if (catcherBaseType == null) return;

            var catcherTypes = GetTypesInheritingFrom(catcherBaseType);

            // 添加抽象基类
            beans.Add(new BeanDefinition
            {
                Name = "TargetCatcher",
                Parent = "",
                Comment = "TargetCatcher基类",
                IsAbstract = true
            });

            foreach (var type in catcherTypes)
            {
                if (type.IsAbstract) continue;

                var paramType = GetGenericParamType(type, catcherBaseType);

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "TargetCatcher",
                    Comment = GetXmlComment(type) ?? $"TargetCatcher: {type.Name}",
                    IsAbstract = false
                };

                if (paramType != null)
                {
                    bean.Fields.Add(new BeanField
                    {
                        Name = "Param",
                        Type = paramType.Name,
                        Comment = "参数"
                    });
                }

                beans.Add(bean);
            }
        }

        #endregion

        #region 类型扫描辅助方法

        /// <summary>
        /// 获取实现指定接口的所有非抽象类型
        /// </summary>
        private static List<Type> GetTypesImplementingInterface(Type interfaceType)
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => !t.IsAbstract && interfaceType.IsAssignableFrom(t) && t != interfaceType);
                    result.AddRange(types);
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略加载异常
                }
            }

            return result;
        }

        /// <summary>
        /// 获取继承自指定基类的所有非抽象类型
        /// </summary>
        private static List<Type> GetTypesInheritingFrom(Type baseType)
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => !t.IsAbstract && t != baseType && baseType.IsAssignableFrom(t));
                    result.AddRange(types);
                }
                catch (ReflectionTypeLoadException)
                {
                    // 忽略加载异常
                }
            }

            return result;
        }

        /// <summary>
        /// 获取泛型基类的类型参数
        /// </summary>
        private static Type GetGenericParamType(Type type, Type genericBaseType)
        {
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == genericBaseType)
                {
                    return current.GetGenericArguments()[0];
                }
                current = current.BaseType;
            }
            return null;
        }

        /// <summary>
        /// 从类型收集字段信息
        /// </summary>
        private static void CollectFieldsFromType(Type type, BeanDefinition bean)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.IsInitOnly) continue; // 跳过readonly字段

                bean.Fields.Add(new BeanField
                {
                    Name = field.Name,
                    Type = MapCSharpTypeToLubanType(field.FieldType),
                    Comment = GetFieldComment(field),
                    IsList = typeof(IList<>).IsAssignableFrom(field.FieldType) ||
                             field.FieldType.IsArray
                });
            }

            foreach (var prop in properties)
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                bean.Fields.Add(new BeanField
                {
                    Name = prop.Name,
                    Type = MapCSharpTypeToLubanType(prop.PropertyType),
                    Comment = GetPropertyComment(prop),
                    IsList = typeof(IList<>).IsAssignableFrom(prop.PropertyType) ||
                             prop.PropertyType.IsArray
                });
            }
        }

        /// <summary>
        /// C#类型映射到Luban类型
        /// </summary>
        private static string MapCSharpTypeToLubanType(Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(string)) return "string";
            if (type == typeof(Vector2)) return "vector2";
            if (type == typeof(Vector3)) return "vector3";
            if (type == typeof(Vector4)) return "vector4";

            // 数组类型
            if (type.IsArray)
            {
                var elemType = type.GetElementType();
                return $"array<{MapCSharpTypeToLubanType(elemType)}>";
            }

            // List类型
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elemType = type.GetGenericArguments()[0];
                return $"array<{MapCSharpTypeToLubanType(elemType)}>";
            }

            // 默认返回类型名（自定义类型）
            return type.Name;
        }

        /// <summary>
        /// 获取XML注释
        /// </summary>
        private static string GetXmlComment(MemberInfo member)
        {
            // TODO: 从XML文档文件读取注释
            return null;
        }

        /// <summary>
        /// 获取字段注释
        /// </summary>
        private static string GetFieldComment(FieldInfo field)
        {
            // TODO: 从XML文档文件读取注释
            return field.Name;
        }

        /// <summary>
        /// 获取属性注释
        /// </summary>
        private static string GetPropertyComment(PropertyInfo prop)
        {
            // TODO: 从XML文档文件读取注释
            return prop.Name;
        }

        #endregion

        #region 生成和更新Excel

        private static void GenerateAndUpdateBeans(string beansPath, List<BeanDefinition> beans)
        {
            // 生成Python脚本内容
            var pythonScript = GeneratePythonScript(beans);

            // 写入临时Python脚本
            var tempScriptPath = Path.Combine(Application.temporaryCachePath, "update_beans_temp.py");
            File.WriteAllText(tempScriptPath, pythonScript);

            // 执行Python脚本
            ExecutePythonScript(tempScriptPath, beansPath);

            // 删除临时脚本
            File.Delete(tempScriptPath);
        }

        private static string GeneratePythonScript(List<BeanDefinition> beans)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("#!/usr/bin/env python3");
            sb.AppendLine("# -*- coding: utf-8 -*-");
            sb.AppendLine("# Auto-generated by EX-GAS BeanUpdater");
            sb.AppendLine();
            sb.AppendLine("import sys");
            sb.AppendLine("try:");
            sb.AppendLine("    import openpyxl");
            sb.AppendLine("except ImportError:");
            sb.AppendLine("    print('[ERROR] 请先运行: pip install openpyxl')");
            sb.AppendLine("    sys.exit(1)");
            sb.AppendLine();
            sb.AppendLine("BEANS_PATH = sys.argv[1] if len(sys.argv) > 1 else ''");
            sb.AppendLine();
            sb.AppendLine("# Bean定义数据");
            sb.AppendLine("BEANS = [");

            foreach (var bean in beans)
            {
                sb.AppendLine($"    {{'name': '{bean.Name}', 'parent': '{bean.Parent}', 'comment': '{EscapeString(bean.Comment)}', 'abstract': {bean.IsAbstract.ToString().ToLower()}, 'fields': [");
                foreach (var field in bean.Fields)
                {
                    sb.AppendLine($"        {{'name': '{field.Name}', 'type': '{field.Type}', 'comment': '{EscapeString(field.Comment)}', 'is_list': {field.IsList.ToString().ToLower()}}},");
                }
                sb.AppendLine("    ]},");
            }

            sb.AppendLine("]");
            sb.AppendLine();
            sb.AppendLine("def update_beans_xlsx():");
            sb.AppendLine("    wb = openpyxl.load_workbook(BEANS_PATH)");
            sb.AppendLine("    ws = wb.worksheets[0]");
            sb.AppendLine();
            sb.AppendLine("    # 清空现有数据（保留表头）");
            sb.AppendLine("    for row in range(4, ws.max_row + 1):");
            sb.AppendLine("        for col in range(1, 20):");
            sb.AppendLine("            ws.cell(row=row, column=col).value = None");
            sb.AppendLine();
            sb.AppendLine("    # 写入Bean定义");
            sb.AppendLine("    row = 4");
            sb.AppendLine("    for bean in BEANS:");
            sb.AppendLine("        # 第1列: name");
            sb.AppendLine("        ws.cell(row=row, column=1).value = bean['name']");
            sb.AppendLine("        # 第2列: parent");
            sb.AppendLine("        ws.cell(row=row, column=2).value = bean['parent'] if bean['parent'] else ''");
            sb.AppendLine("        # 第3列: comment");
            sb.AppendLine("        ws.cell(row=row, column=3).value = bean['comment']");
            sb.AppendLine();
            sb.AppendLine("        # 写入字段");
            sb.AppendLine("        col = 4");
            sb.AppendLine("        for field in bean['fields']:");
            sb.AppendLine("            # 字段名");
            sb.AppendLine("            ws.cell(row=row, column=col).value = field['name']");
            sb.AppendLine("            col += 1");
            sb.AppendLine("            # 字段类型");
            sb.AppendLine("            ws.cell(row=row, column=col).value = field['type']");
            sb.AppendLine("            col += 1");
            sb.AppendLine("            # 字段注释");
            sb.AppendLine("            ws.cell(row=row, column=col).value = field['comment']");
            sb.AppendLine("            col += 1");
            sb.AppendLine();
            sb.AppendLine("        row += 1");
            sb.AppendLine();
            sb.AppendLine("    wb.save(BEANS_PATH)");
            sb.AppendLine("    wb.close()");
            sb.AppendLine("    print(f'[SUCCESS] Updated {len(BEANS)} beans')");
            sb.AppendLine();
            sb.AppendLine("if __name__ == '__main__':");
            sb.AppendLine("    if not BEANS_PATH:");
            sb.AppendLine("        print('[ERROR] No beans path provided')");
            sb.AppendLine("        sys.exit(1)");
            sb.AppendLine("    update_beans_xlsx()");

            return sb.ToString();
        }

        private static string EscapeString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Replace("'", "\\'").Replace("\n", "\\n");
        }

        private static void ExecutePythonScript(string scriptPath, string beansPath)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{beansPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
                Debug.Log(output);
            if (!string.IsNullOrEmpty(error))
                Debug.LogError(error);
        }

        #endregion
    }
}