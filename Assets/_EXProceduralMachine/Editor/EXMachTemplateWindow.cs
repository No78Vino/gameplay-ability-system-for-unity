#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EXProceduralMachine.Editor
{
    /// <summary>
    /// EXMach 程序化动画工具箱浏览器（三层体系）：
    ///   🧰 工具（Tool）  —— 基础系统组件，点一下创建空物体并挂载
    ///   🥘 预制菜（Kit） —— 模板预制体，实例化即用（可选预设一键改手感）
    ///   🧸 玩具（Toy）  —— gameplay 小系统（预留分类）
    /// 支持搜索、分类/家族筛选、预设应用、清单导出（JSON，网页端数据源）。
    /// </summary>
    public class EXMachTemplateWindow : EditorWindow
    {
        private const string ExportDefaultPath = "Assets/_EXProceduralMachine/Docs/toolbox-manifest.json";

        // ==================== 状态 ====================

        private ToolboxManifest _manifest;
        private string _search = "";
        private ToolboxCategory? _categoryFilter;
        private ToolboxFamily? _familyFilter;
        private ToolboxItem _selected;
        private ToolboxPreset _selectedPreset;
        private Vector2 _scroll;

        [MenuItem("Tools/EXMach/程序化动画工具箱")]
        public static void Open()
        {
            var window = GetWindow<EXMachTemplateWindow>("EXMach 程序化动画工具箱");
            window.minSize = new Vector2(560, 400);
            window.Show();
        }

        private void OnEnable()
        {
            _manifest = ToolboxCatalog.BuildManifest();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawFilterBar();
            DrawList();
            DrawFooter();
        }

        // ==================== 头部 ====================

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("🧰 EXMach 程序化动画工具箱", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("刷新", GUILayout.Width(56)))
                    _manifest = ToolboxCatalog.BuildManifest();
            }

            // 搜索框
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("🔍 搜索", GUILayout.Width(44));
                _search = EditorGUILayout.TextField(_search);
                if (GUILayout.Button("清除", GUILayout.Width(52)))
                    _search = "";
            }

            EditorGUILayout.HelpBox(
                $"共 {_manifest.items.Count} 个条目：\n" +
                $"🧰 工具 {CountOf(ToolboxCategory.Tool)} 个 ｜ 🥘 预制菜 {CountOf(ToolboxCategory.Kit)} 个 ｜ 🧸 玩具 {CountOf(ToolboxCategory.Toy)} 个\n" +
                "点击条目名可定位资产；「实例化」一键放入当前场景（工具=创建空物体挂载，预制菜=实例化模板）。",
                MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        // ==================== 筛选栏 ====================

        private void DrawFilterBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawCategoryButton(null, "全部");
                DrawCategoryButton(ToolboxCategory.Tool, "🧰 工具");
                DrawCategoryButton(ToolboxCategory.Kit, "🥘 预制菜");
                DrawCategoryButton(ToolboxCategory.Toy, "🧸 玩具");

                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("家族", GUILayout.Width(36));
                var familyNames = new[] { "全部", "跟随", "摆动", "回弹", "步态", "呼吸", "调试" };
                var familyValues = new ToolboxFamily?[]
                {
                    null, ToolboxFamily.Follow, ToolboxFamily.Sway, ToolboxFamily.Bounce,
                    ToolboxFamily.Locomotion, ToolboxFamily.Breath, ToolboxFamily.Debug
                };
                var currentIndex = System.Array.IndexOf(familyValues, _familyFilter);
                if (currentIndex < 0) currentIndex = 0;
                var newIndex = EditorGUILayout.Popup(currentIndex, familyNames, GUILayout.Width(72));
                if (newIndex != currentIndex)
                {
                    _familyFilter = familyValues[newIndex];
                    _selected = null;
                    _selectedPreset = null;
                }
            }
        }

        private void DrawCategoryButton(ToolboxCategory? cat, string label)
        {
            var active = _categoryFilter == cat;
            var style = active ? "SelectionRect" : "Button";
            if (GUILayout.Button(label, style, GUILayout.Width(80)))
            {
                _categoryFilter = active ? null : cat;
                _selected = null;
                _selectedPreset = null;
            }
        }

        // ==================== 列表 ====================

        private void DrawList()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var visible = FilteredItems().ToList();

            if (visible.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    _search.Length > 0 || _categoryFilter != null || _familyFilter != null
                        ? "没有符合筛选条件的条目。试试清除搜索/筛选。"
                        : "工具箱为空。\n\n请确认：\n1. Examples/ 目录存在模板预制体\n2. 内置工具定义有效\n3. 点击「刷新」重建清单",
                    MessageType.Warning);
            }

            // 按分类分组展示
            DrawGroup(ToolboxCategory.Tool, "🧰 工具（基础系统）", visible);
            DrawGroup(ToolboxCategory.Kit, "🥘 预制菜（模板）", visible);
            DrawGroup(ToolboxCategory.Toy, "🧸 玩具（Gameplay 小系统）", visible);

            EditorGUILayout.EndScrollView();
        }

        private void DrawGroup(ToolboxCategory cat, string groupTitle, List<ToolboxItem> visible)
        {
            var group = visible.Where(i => i.category == cat).ToList();
            if (group.Count == 0)
            {
                if (_categoryFilter == null || _categoryFilter == cat)
                {
                    EditorGUILayout.LabelField(groupTitle, EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(
                        cat == ToolboxCategory.Toy
                            ? "玩具分类即将推出：震屏、受击抖动、相机跟随、武器摆动等 gameplay 小系统。"
                            : "该分类暂无条目。",
                        MessageType.Info);
                    GUILayout.Space(6);
                }
                return;
            }

            EditorGUILayout.LabelField($"{groupTitle}（{group.Count}）", EditorStyles.boldLabel);
            foreach (var item in group)
                DrawEntry(item);
            GUILayout.Space(8);
        }

        private void DrawEntry(ToolboxItem item)
        {
            var style = _selected == item ? "SelectionRect" : "box";
            EditorGUILayout.BeginVertical(style);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawThumbnail(item);

                using (new EditorGUILayout.VerticalScope())
                {
                    // 名称 + 家族徽章
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button(item.name, EditorStyles.boldLabel))
                        {
                            _selected = item;
                            _selectedPreset = item.presets.Count > 0 ? item.presets[0] : null;
                            PingItem(item);
                        }
                        GUILayout.FlexibleSpace();
                        DrawFamilyBadge(item.family);
                    }

                    EditorGUILayout.LabelField(item.description, EditorStyles.miniLabel, GUILayout.MaxWidth(420));

                    // 路径
                    var pathText = item.category == ToolboxCategory.Tool
                        ? $"组件: {ShortType(item.componentType)}"
                        : item.prefabPath;
                    EditorGUILayout.LabelField(pathText, EditorStyles.miniLabel);

                    GUILayout.Space(4);

                    // 操作行
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // 预设下拉（仅 Kit/Toy 且有预设时）
                        if (item.presets.Count > 0)
                        {
                            var presetNames = item.presets.Select(p => p.name).ToArray();
                            var curIdx = Mathf.Max(0, item.presets.FindIndex(p => p == _selectedPreset));
                            var newIdx = EditorGUILayout.Popup(curIdx, presetNames, GUILayout.Width(150));
                            if (newIdx != curIdx)
                            {
                                _selected = item;
                                _selectedPreset = item.presets[newIdx];
                            }
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("实例化", GUILayout.Width(72)))
                            Instantiate(item);
                        if (GUILayout.Button("定位", GUILayout.Width(52)))
                            PingItem(item);
                    }
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        private void DrawThumbnail(ToolboxItem item)
        {
            if (item.category == ToolboxCategory.Kit)
            {
                var prefab = item.LoadPrefab();
                var preview = prefab != null ? AssetPreview.GetAssetPreview(prefab) : null;
                if (preview != null)
                {
                    GUILayout.Label(preview, GUILayout.Width(72), GUILayout.Height(72));
                }
                else
                {
                    GUILayout.Box("预览…", GUILayout.Width(72), GUILayout.Height(72));
                    Repaint();
                }
            }
            else
            {
                var icon = item.category == ToolboxCategory.Tool ? "d_SettingsIcon" : "d_Toolbar Plus";
                var tex = EditorGUIUtility.IconContent(icon).image;
                GUILayout.Box(tex, GUILayout.Width(72), GUILayout.Height(72));
            }
        }

        private void DrawFamilyBadge(ToolboxFamily family)
        {
            var name = family switch
            {
                ToolboxFamily.Follow => "跟随",
                ToolboxFamily.Sway => "摆动",
                ToolboxFamily.Bounce => "回弹",
                ToolboxFamily.Locomotion => "步态",
                ToolboxFamily.Breath => "呼吸",
                _ => "调试"
            };
            var color = family switch
            {
                ToolboxFamily.Follow => new Color(0.2f, 0.6f, 1f),
                ToolboxFamily.Sway => new Color(0.9f, 0.4f, 0.7f),
                ToolboxFamily.Bounce => new Color(1f, 0.6f, 0.2f),
                ToolboxFamily.Locomotion => new Color(0.4f, 0.8f, 0.3f),
                ToolboxFamily.Breath => new Color(0.6f, 0.5f, 1f),
                _ => new Color(0.6f, 0.6f, 0.6f)
            };
            var prev = GUI.color;
            GUI.color = color;
            GUILayout.Label(name, EditorStyles.miniBoldLabel, GUILayout.Width(36));
            GUI.color = prev;
        }

        // ==================== 底部 ====================

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"显示 {FilteredItems().Count()} / {_manifest.items.Count} 条目", EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("导出清单 JSON（网页端数据源）", EditorStyles.miniButton))
                    ExportManifest();
                if (GUILayout.Button("打开 Examples 目录", EditorStyles.miniButton))
                    EditorUtility.RevealInFinder(ToolboxCatalog.KitScanFolder);
            }
        }

        // ==================== 逻辑 ====================

        private int CountOf(ToolboxCategory cat) => _manifest.items.Count(i => i.category == cat);

        private IEnumerable<ToolboxItem> FilteredItems()
        {
            IEnumerable<ToolboxItem> q = _manifest.items;

            if (_categoryFilter.HasValue)
                q = q.Where(i => i.category == _categoryFilter.Value);
            if (_familyFilter.HasValue)
                q = q.Where(i => i.family == _familyFilter.Value);

            if (!string.IsNullOrWhiteSpace(_search))
            {
                var kw = _search.Trim().ToLowerInvariant();
                q = q.Where(i =>
                    i.name.ToLowerInvariant().Contains(kw) ||
                    i.description.ToLowerInvariant().Contains(kw) ||
                    (i.componentType ?? "").ToLowerInvariant().Contains(kw) ||
                    (i.prefabPath ?? "").ToLowerInvariant().Contains(kw));
            }

            return q.OrderBy(i => i.category).ThenBy(i => i.name);
        }

        private static string ShortType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "";
            var dot = fullName.LastIndexOf('.');
            return dot >= 0 ? fullName.Substring(dot + 1) : fullName;
        }

        private static void PingItem(ToolboxItem item)
        {
            if (item.category == ToolboxCategory.Tool)
            {
                var type = ToolboxCatalog.ResolveType(item.componentType);
                if (type != null)
                {
                    var mono = FindMonoScript(type);
                    if (mono != null)
                    {
                        Selection.activeObject = mono;
                        EditorGUIUtility.PingObject(mono);
                    }
                }
            }
            else
            {
                var prefab = item.LoadPrefab();
                if (prefab != null)
                {
                    Selection.activeObject = prefab;
                    EditorGUIUtility.PingObject(prefab);
                }
            }
        }

        private static MonoScript FindMonoScript(System.Type type)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:MonoScript"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (mono != null && mono.GetClass() == type)
                    return mono;
            }
            return null;
        }

        private void Instantiate(ToolboxItem item)
        {
            var go = ToolboxCatalog.InstantiateItem(item, _selectedPreset);
            if (go == null) return;
            Selection.activeGameObject = go;
            if (item.category == ToolboxCategory.Tool && _selectedPreset != null && _selectedPreset.overrides.Count > 0)
                Debug.Log($"[EXMach] 已应用预设「{_selectedPreset.name}」到 {go.name}（{_selectedPreset.overrides.Count} 项覆写）");
        }

        private void ExportManifest()
        {
            var path = EditorUtility.SaveFilePanel("导出工具箱清单（JSON）", "Assets/_EXProceduralMachine/Docs",
                "toolbox-manifest", "json");
            if (string.IsNullOrEmpty(path))
                return;
            // 若保存到项目内，转换为相对路径
            if (path.StartsWith(Application.dataPath))
                path = "Assets" + path.Substring(Application.dataPath.Length);
            ToolboxCatalog.ExportJson(ToolboxCatalog.BuildManifest(), path);
        }
    }
}
#endif