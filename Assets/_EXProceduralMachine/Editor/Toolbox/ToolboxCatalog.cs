using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EXProceduralMachine.Editor
{
    /// <summary>
    /// 工具箱目录源：内置工具定义 + 预制菜扫描 + 预设注入 + 清单导出（JSON，网页端数据源）
    /// </summary>
    public static class ToolboxCatalog
    {
        /// <summary>Kit 扫描目录（预制菜放在这里自动收录）</summary>
        public const string KitScanFolder = "Assets/_EXProceduralMachine/Examples";

        // ==================== 构建清单 ====================

        /// <summary>构建完整清单：内置工具 + 扫描预制菜（含预设注入）</summary>
        public static ToolboxManifest BuildManifest()
        {
            var manifest = new ToolboxManifest { version = 1 };
            manifest.items.AddRange(BuildTools());
            manifest.items.AddRange(ScanKits());
            return manifest;
        }

        // ==================== 内置工具定义 ====================

        private static IEnumerable<ToolboxItem> BuildTools()
        {
            return new List<ToolboxItem>
            {
                new ToolboxItem
                {
                    id = "tool.secondorder",
                    name = "次级运动系统",
                    category = ToolboxCategory.Tool,
                    family = ToolboxFamily.Follow,
                    description = "二阶动力学次级运动容器。挂载后添加实例（位置/旋转/四元数/缩放/Custom），模拟惯性、回弹、空气阻力。",
                    componentType = typeof(SecondOrderDynamicsComponent).FullName
                },
                new ToolboxItem
                {
                    id = "tool.rhythm",
                    name = "呼吸/节奏系统",
                    category = ToolboxCategory.Tool,
                    family = ToolboxFamily.Breath,
                    description = "多周期混合节奏驱动（加/乘/最大/最小），驱动缩放/位置/旋转，适合待机呼吸、悬浮、节奏动画。",
                    componentType = typeof(RhythmController).FullName
                },
                new ToolboxItem
                {
                    id = "tool.gun",
                    name = "命中式枪械",
                    category = ToolboxCategory.Tool,
                    family = ToolboxFamily.Bounce,
                    description = "简易 Hitscan 枪械：射速/射程/伤害/命中特效/后坐回弹，带 IDamageable 接口。",
                    componentType = typeof(Gun).FullName
                },
                new ToolboxItem
                {
                    id = "tool.visualaid",
                    name = "可视化辅助",
                    category = ToolboxCategory.Tool,
                    family = ToolboxFamily.Debug,
                    description = "Box/Sphere/Line 调试标记（Editor Gizmo），用于可视化程序化动画的关键点与轨迹。",
                    componentType = typeof(XVisualAid).FullName
                }
            };
        }

        // ==================== 预制菜扫描 + 预设注入 ====================

        private static IEnumerable<ToolboxItem> ScanKits()
        {
            var items = new List<ToolboxItem>();
            if (!Directory.Exists(KitScanFolder))
                return items;

            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { KitScanFolder });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;

                var item = new ToolboxItem
                {
                    id = "kit." + prefab.name,
                    name = prefab.name,
                    category = ToolboxCategory.Kit,
                    family = GuessFamily(prefab),
                    description = $"模板预制体：{prefab.name}。拖入场景即用（已含默认参数与调试驱动）。",
                    prefabPath = path
                };
                AttachKitPresets(item);
                items.Add(item);
            }

            return items;
        }

        /// <summary>按名称启发式猜测家族（后续可在窗口手动调整）</summary>
        private static ToolboxFamily GuessFamily(GameObject prefab)
        {
            var name = prefab.name.ToLowerInvariant();
            if (name.Contains("drone") || name.Contains("fly") || name.Contains("aerial")) return ToolboxFamily.Follow;
            if (name.Contains("spider") || name.Contains("walk") || name.Contains("leg") || name.Contains("quad")) return ToolboxFamily.Locomotion;
            if (name.Contains("gun") || name.Contains("recoil")) return ToolboxFamily.Bounce;
            if (name.Contains("breath") || name.Contains("rhythm")) return ToolboxFamily.Breath;
            if (name.Contains("sway") || name.Contains("tail") || name.Contains("cloth")) return ToolboxFamily.Sway;
            return ToolboxFamily.Debug;
        }

        /// <summary>按模板名注入预设（预设 = 参数覆写集合，实例化时一键应用）</summary>
        private static void AttachKitPresets(ToolboxItem item)
        {
            if (item.name.Contains("Drone"))
            {
                item.presets = new List<ToolboxPreset>
                {
                    new ToolboxPreset
                    {
                        id = "drone.default",
                        name = "轻盈巡航（默认）",
                        description = "默认手感：阻力 1.8、浮动 8cm @ 1.2Hz"
                    },
                    new ToolboxPreset
                    {
                        id = "drone.heavy",
                        name = "重载手感",
                        description = "更大空气阻力 + 更明显悬停浮动，机身更“粘稠”。",
                        overrides = new List<ToolboxParamOverride>
                        {
                            new ToolboxParamOverride { componentType = "DroneFlyDriver", field = "drag", value = "3" },
                            new ToolboxParamOverride { componentType = "DroneFlyDriver", field = "hoverBobAmplitude", value = "0.15" }
                        }
                    },
                    new ToolboxPreset
                    {
                        id = "drone.snappy",
                        name = "轻快跟手",
                        description = "更快速度响应 + 更高浮动频率，适合需要精准操控的演示。",
                        overrides = new List<ToolboxParamOverride>
                        {
                            new ToolboxParamOverride { componentType = "DroneFlyDriver", field = "drag", value = "2.6" },
                            new ToolboxParamOverride { componentType = "DroneFlyDriver", field = "hoverBobFrequency", value = "1.8" }
                        }
                    }
                };
            }
            else if (item.name.Contains("Spider"))
            {
                item.presets = new List<ToolboxPreset>
                {
                    new ToolboxPreset
                    {
                        id = "spider.default",
                        name = "对角步态（默认）",
                        description = "四足对角交替步态，自动行走调试驱动已开启。"
                    }
                };
            }
        }

        // ==================== 导出 / 导入（网页端数据源） ====================

        /// <summary>导出清单为 JSON（DTO 化：分类/家族为字符串，网页端零解析成本）</summary>
        public static void ExportJson(ToolboxManifest manifest, string path)
        {
            var dto = ToDto(manifest);
            File.WriteAllText(path, JsonUtility.ToJson(dto, true));
            AssetDatabase.Refresh();
            Debug.Log($"[EXMach] 工具箱清单已导出：{path}");
        }

        /// <summary>从 JSON 导入清单（Unity 侧浏览器的数据源，网页端产物可回读）</summary>
        public static ToolboxManifest ImportJson(string path)
        {
            if (!File.Exists(path)) return null;
            var dto = JsonUtility.FromJson<ToolboxManifestDto>(File.ReadAllText(path));
            if (dto == null) return null;
            return FromDto(dto);
        }

        private static ToolboxManifestDto ToDto(ToolboxManifest m)
        {
            var dto = new ToolboxManifestDto { version = m.version, module = m.module, items = new List<ToolboxItemDto>() };
            foreach (var item in m.items)
            {
                var idto = new ToolboxItemDto
                {
                    id = item.id,
                    name = item.name,
                    category = item.category.ToString(),
                    family = item.family.ToString(),
                    description = item.description,
                    prefabPath = item.prefabPath,
                    componentType = item.componentType,
                    presets = new List<ToolboxPresetDto>()
                };
                foreach (var p in item.presets)
                {
                    var pdto = new ToolboxPresetDto
                    {
                        id = p.id,
                        name = p.name,
                        description = p.description,
                        overrides = new List<ToolboxOverrideDto>()
                    };
                    foreach (var o in p.overrides)
                    {
                        pdto.overrides.Add(new ToolboxOverrideDto
                        {
                            componentPath = o.componentPath,
                            componentType = o.componentType,
                            field = o.field,
                            value = o.value
                        });
                    }
                    idto.presets.Add(pdto);
                }
                dto.items.Add(idto);
            }
            return dto;
        }

        private static ToolboxManifest FromDto(ToolboxManifestDto dto)
        {
            var m = new ToolboxManifest { version = dto.version, module = dto.module };
            foreach (var idto in dto.items)
            {
                var item = new ToolboxItem
                {
                    id = idto.id,
                    name = idto.name,
                    category = ParseEnum(idto.category, ToolboxCategory.Kit),
                    family = ParseEnum(idto.family, ToolboxFamily.Follow),
                    description = idto.description,
                    prefabPath = idto.prefabPath,
                    componentType = idto.componentType
                };
                if (idto.presets != null)
                    foreach (var pdto in idto.presets)
                    {
                        var preset = new ToolboxPreset { id = pdto.id, name = pdto.name, description = pdto.description };
                        if (pdto.overrides != null)
                            foreach (var odto in pdto.overrides)
                                preset.overrides.Add(new ToolboxParamOverride
                                {
                                    componentPath = odto.componentPath,
                                    componentType = odto.componentType,
                                    field = odto.field,
                                    value = odto.value
                                });
                        item.presets.Add(preset);
                    }
                m.items.Add(item);
            }
            return m;
        }

        private static TEnum ParseEnum<TEnum>(string s, TEnum fallback) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(s, true, out var result) ? result : fallback;
        }

        // ==================== 实例化辅助 ====================

        /// <summary>实例化条目（Tool 创建空物体挂组件；Kit/Toy 实例化预制体），返回创建的 GameObject</summary>
        public static GameObject InstantiateItem(ToolboxItem item, ToolboxPreset preset)
        {
            GameObject result = null;
            if (item.category == ToolboxCategory.Tool)
            {
                var type = ResolveType(item.componentType);
                if (type == null)
                {
                    Debug.LogError($"[EXMach] 无法解析组件类型：{item.componentType}");
                    return null;
                }
                var go = new GameObject($"EXMach_{item.name}");
                go.AddComponent(type);
                Undo.RegisterCreatedObjectUndo(go, $"Add Tool {item.name}");
                result = go;
            }
            else
            {
                var prefab = item.LoadPrefab();
                if (prefab == null)
                {
                    Debug.LogError($"[EXMach] 预制体不存在：{item.prefabPath}");
                    return null;
                }
                result = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                if (result == null) return null;
                Undo.RegisterCreatedObjectUndo(result, $"Instantiate {item.name}");
            }

            if (preset != null && preset.overrides != null && preset.overrides.Count > 0)
                ApplyOverrides(result, preset.overrides);

            return result;
        }

        /// <summary>按全名或短名解析组件类型（遍历已加载程序集）</summary>
        public static Type ResolveType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName, false);
                if (t != null) return t;
                // 兼容短名：按 Type.Name 匹配
                if (t == null)
                {
                    t = asm.GetTypes().FirstOrDefault(x => x.Name == fullName);
                    if (t != null) return t;
                }
            }
            return null;
        }

        /// <summary>按"组件路径 + 类型名"查找实例上的组件</summary>
        private static Component FindComponent(GameObject root, string componentPath, string componentType)
        {
            Transform t = root.transform;
            if (!string.IsNullOrEmpty(componentPath))
            {
                foreach (var seg in componentPath.Split('/'))
                {
                    if (seg.Length == 0) continue;
                    t = t.Find(seg);
                    if (t == null) return null;
                }
            }

            foreach (var c in t.GetComponents<Component>())
            {
                if (c == null) continue;
                if (c.GetType().Name == componentType || c.GetType().FullName == componentType)
                    return c;
            }
            return null;
        }

        /// <summary>应用一组参数覆写（值按字段类型解析）</summary>
        public static void ApplyOverrides(GameObject root, List<ToolboxParamOverride> overrides)
        {
            foreach (var o in overrides)
            {
                var comp = FindComponent(root, o.componentPath, o.componentType);
                if (comp == null)
                {
                    Debug.LogWarning($"[EXMach] 未找到覆写目标：{o.componentType} @ {o.componentPath ?? "(root)"}，跳过字段 {o.field}");
                    continue;
                }
                var so = new SerializedObject(comp);
                var prop = so.FindProperty(o.field);
                if (prop == null)
                {
                    Debug.LogWarning($"[EXMach] 字段不存在：{comp.GetType().Name}.{o.field}");
                    continue;
                }
                if (!ApplyValue(prop, o.value))
                    Debug.LogWarning($"[EXMach] 值解析失败：{o.field} = '{o.value}'（类型 {prop.propertyType}）");
                so.ApplyModifiedProperties();
            }
        }

        private static bool ApplyValue(SerializedProperty prop, string value)
        {
            try
            {
                switch (prop.propertyType)
                {
                    case SerializedPropertyType.Float:
                        if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f)) return false;
                        prop.floatValue = f;
                        return true;
                    case SerializedPropertyType.Integer:
                        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)) return false;
                        prop.intValue = i;
                        return true;
                    case SerializedPropertyType.Boolean:
                        if (value == "1") { prop.boolValue = true; return true; }
                        if (value == "0") { prop.boolValue = false; return true; }
                        if (!bool.TryParse(value, out var b)) return false;
                        prop.boolValue = b;
                        return true;
                    case SerializedPropertyType.Vector3:
                        var parts = value.Split(',');
                        if (parts.Length != 3) return false;
                        if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var vx)) return false;
                        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var vy)) return false;
                        if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var vz)) return false;
                        prop.vector3Value = new Vector3(vx, vy, vz);
                        return true;
                    case SerializedPropertyType.Enum:
                        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ei)) return false;
                        prop.enumValueIndex = ei;
                        return true;
                    case SerializedPropertyType.String:
                        prop.stringValue = value;
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}