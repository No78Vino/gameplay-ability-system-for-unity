using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using System.Reflection;  
using GAS.Runtime;  
using OfficeOpenXml;  
using UnityEditor;  
using UnityEngine;
using OfficeOpenXml.Style;

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

        // __beans__.xlsx 列号定义（EPPlus 1-indexed）  
        private const int COL_FULL_NAME = 2; // B: full_name  
        private const int COL_PARENT = 3; // C: parent  
        private const int COL_COMMENT = 7; // G: comment  
        private const int COL_FIELD_NAME = 10; // J: field name  
        private const int COL_FIELD_TYPE = 12; // L: field type  
        private const int COL_FIELD_COMMENT = 14; // N: field comment  
        private const int DATA_START_ROW = 4; // 数据起始行  

        #region 数据结构

        /// <summary>  
        /// Bean定义信息  
        /// </summary>  
        private class BeanDefinition
        {
            public string Name; // Bean名称  
            public string Parent; // 父类Bean名称  
            public string Comment; // 注释  
            public List<BeanField> Fields = new(); // 字段列表  
            public bool IsAbstract; // 是否抽象类  
        }

        /// <summary>  
        /// Bean字段定义  
        /// </summary>  
        private class BeanField
        {
            public string Name; // 字段名  
            public string Type; // 字段类型  
            public string Comment; // 注释  
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

            try
            {
                // 收集所有Bean定义  
                var beans = CollectAllBeans();

                // 写入Bean定义到xlsx  
                GenerateAndUpdateBeans(beansPath, beans);

                Debug.Log($"[BeanUpdater] Bean定义更新完成，共 {beans.Count} 个Bean");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        #endregion

        #region 收集Bean定义

        private static List<BeanDefinition> CollectAllBeans()
        {
            var beans = new List<BeanDefinition>();

            const string title = "BeanUpdater - 收集Bean定义";

            // 1. 收集XParam参数类  
            EditorUtility.DisplayProgressBar(title, "收集 XParam 参数类...", 0f / 6f);
            CollectXParamBeans(beans);

            // 2. 收集Cue逻辑类  
            EditorUtility.DisplayProgressBar(title, "收集 Cue 逻辑类...", 1f / 6f);
            CollectCueBeans(beans);

            // 3. 收集MMC逻辑类  
            EditorUtility.DisplayProgressBar(title, "收集 MMC 逻辑类...", 2f / 6f);
            CollectMmcBeans(beans);

            // 4. 收集AbilityLogic逻辑类  
            EditorUtility.DisplayProgressBar(title, "收集 AbilityLogic 逻辑类...", 3f / 6f);
            CollectAbilityLogicBeans(beans);

            // 5. 收集AbilityTask类  
            EditorUtility.DisplayProgressBar(title, "收集 AbilityTask 类...", 4f / 6f);
            CollectAbilityTaskBeans(beans);

            // 6. 收集TargetCatcher类  
            EditorUtility.DisplayProgressBar(title, "收集 TargetCatcher 类...", 5f / 6f);
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
                Name = "GameplayCueBase",
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
                    Parent = "GameplayCueBase",
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
                Name = "ModMagnitudeCalculationBase",
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
                    Parent = "ModMagnitudeCalculationBase",
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
                Name = "AbilityLogicBase",
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
                    Parent = "AbilityLogicBase",
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
                Name = "AbilityTaskBase",
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
                    Parent = "AbilityTaskBase",
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
            var catcherTypes = GetTypesInheritingFrom(typeof(TargetCatcherBase));

            // 添加抽象基类  
            beans.Add(new BeanDefinition
            {
                Name = "TargetCatcherBase", // 统一命名  
                Parent = "",
                Comment = "TargetCatcher基类",
                IsAbstract = true
            });

            foreach (var type in catcherTypes)
            {
                if (type.IsAbstract) continue;

                // 关键修复：用 typeof(TargetCatcherBase<>) 提取泛型参数 T  
                var paramType = GetGenericParamType(type, typeof(TargetCatcherBase<>));

                var bean = new BeanDefinition
                {
                    Name = type.Name,
                    Parent = "TargetCatcherBase", // 统一命名  
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
        /// 从类型收集字段信息，按 Order 排序（默认为源码行号，由 [CallerLineNumber] 自动填入）    
        /// 同时收集 [BeanField] 和 [BeanPolymorphicField] 标注的成员    
        /// </summary>    
        private static void CollectFieldsFromType(Type type, BeanDefinition bean)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var items = new List<(int order, BeanField beanField)>();

            var fields = type.GetFields(bindingFlags);
            var properties = type.GetProperties(bindingFlags);

            // ── 扫描 [BeanField] ──  

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<BeanFieldAttribute>();
                if (attr == null) continue;

                items.Add((attr.Order, new BeanField
                {
                    Name = attr.Name ?? field.Name,
                    Type = attr.LubanType ?? MapCSharpTypeToLubanType(field.FieldType),
                    Comment = attr.Comment ?? field.Name,
                }));
            }

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<BeanFieldAttribute>();
                if (attr == null) continue;

                items.Add((attr.Order, new BeanField
                {
                    Name = attr.Name ?? prop.Name,
                    Type = attr.LubanType ?? MapCSharpTypeToLubanType(prop.PropertyType),
                    Comment = attr.Comment ?? prop.Name,
                }));
            }

            // ── 扫描 [BeanPolymorphicField] ──  

            foreach (var field in fields)
            {
                var polyAttr = field.GetCustomAttribute<BeanPolymorphicFieldAttribute>();
                if (polyAttr == null) continue;

                items.Add((polyAttr.Order, new BeanField
                {
                    Name = polyAttr.BeanFieldName,
                    Type = polyAttr.LubanPolymorphicType,
                    Comment = polyAttr.BeanFieldName,
                }));
            }

            foreach (var prop in properties)
            {
                var polyAttr = prop.GetCustomAttribute<BeanPolymorphicFieldAttribute>();
                if (polyAttr == null) continue;

                items.Add((polyAttr.Order, new BeanField
                {
                    Name = polyAttr.BeanFieldName,
                    Type = polyAttr.LubanPolymorphicType,
                    Comment = polyAttr.BeanFieldName,
                }));
            }

            // ── 按 Order 排序后写入 ──  
            // 使用 LINQ OrderBy（稳定排序），Order 相同时保持收集顺序  
            foreach (var item in items.OrderBy(x => x.order))
                bean.Fields.Add(item.beanField);
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

            // 数组类型 → Luban bean 定义格式  
            if (type.IsArray)
            {
                var elemType = type.GetElementType();
                return $"(array#sep=;),{MapCSharpTypeToLubanType(elemType)}";
            }

            // List类型 → Luban bean 定义格式  
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elemType = type.GetGenericArguments()[0];
                return $"(array#sep=;),{MapCSharpTypeToLubanType(elemType)}";
            }

            // 默认返回类型名（自定义类型，如 CueLogic、TargetCatcher 等）  
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

        #endregion

        #region 生成和更新Excel

        private static void GenerateAndUpdateBeans(string beansPath, List<BeanDefinition> beans)
        {
            const string title = "BeanUpdater - 写入Excel";
            // 护眼蓝：柔和的浅蓝色  
            var separatorColor = System.Drawing.Color.FromArgb(218, 238, 243);

            var fileInfo = new FileInfo(beansPath);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];

                // 整行删除：同时清除值和格式（背景色、字体等）  
                EditorUtility.DisplayProgressBar(title, "清除旧数据...", 0f);
                if (worksheet.Dimension != null && worksheet.Dimension.End.Row >= DATA_START_ROW)
                {
                    var delCount = worksheet.Dimension.End.Row - DATA_START_ROW + 1;
                    if (delCount > 0)
                        worksheet.DeleteRow(DATA_START_ROW, delCount);
                }

                // 写入 Bean 定义（垂直布局：每个字段占一行）  
                var currentRow = DATA_START_ROW;
                var isFirstBean = true;
                var totalBeans = beans.Count;

                for (var beanIndex = 0; beanIndex < totalBeans; beanIndex++)
                {
                    var bean = beans[beanIndex];

                    // 更新进度条  
                    EditorUtility.DisplayProgressBar(title,
                        $"写入 Bean: {bean.Name} ({beanIndex + 1}/{totalBeans})",
                        (float)(beanIndex + 1) / totalBeans);

                    // 抽象基类标志着一个新类别的开始，在其前面插入蓝色分隔行（第一个除外）  
                    if (bean.IsAbstract && !isFirstBean)
                    {
                        // 填充整行护眼蓝色作为类别分隔  
                        var separatorRange = worksheet.Cells[currentRow, 1, currentRow, COL_FIELD_COMMENT];
                        separatorRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        separatorRange.Style.Fill.BackgroundColor.SetColor(separatorColor);
                        currentRow++;
                    }

                    isFirstBean = false;

                    // Bean 首行：写 full_name, parent, comment  
                    worksheet.Cells[currentRow, COL_FULL_NAME].Value = bean.Name;
                    worksheet.Cells[currentRow, COL_PARENT].Value = bean.Parent ?? "";
                    worksheet.Cells[currentRow, COL_COMMENT].Value = bean.Comment ?? "";

                    if (bean.Fields.Count > 0)
                    {
                        // 首行同时写第一个字段  
                        var firstField = bean.Fields[0];
                        worksheet.Cells[currentRow, COL_FIELD_NAME].Value = firstField.Name;
                        worksheet.Cells[currentRow, COL_FIELD_TYPE].Value = firstField.Type;
                        worksheet.Cells[currentRow, COL_FIELD_COMMENT].Value = firstField.Comment ?? "";
                        currentRow++;

                        // 后续字段每个占一行（只填字段列）  
                        for (var i = 1; i < bean.Fields.Count; i++)
                        {
                            var field = bean.Fields[i];
                            worksheet.Cells[currentRow, COL_FIELD_NAME].Value = field.Name;
                            worksheet.Cells[currentRow, COL_FIELD_TYPE].Value = field.Type;
                            worksheet.Cells[currentRow, COL_FIELD_COMMENT].Value = field.Comment ?? "";
                            currentRow++;
                        }

                        // 多字段 Bean 后添加空行分隔  
                        if (bean.Fields.Count > 1)
                        {
                            currentRow++;
                        }
                    }
                    else
                    {
                        // 无字段的 Bean（如抽象基类）占一行  
                        currentRow++;
                    }
                }

                EditorUtility.DisplayProgressBar(title, "保存文件...", 1f);
                package.Save();
            }
        }

        #endregion
    }
}