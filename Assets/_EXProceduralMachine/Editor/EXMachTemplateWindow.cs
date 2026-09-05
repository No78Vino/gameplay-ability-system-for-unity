#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EXProceduralMachine.Editor
{
    /// <summary>
    ///     EXMach 程序化动画模板集中窗口。
    ///     扫描模块 Examples 目录下的动画模板预制体，列出名称、挂载组件与预览缩略图，
    ///     支持「定位资产」与「一键实例化到当前场景」。
    ///     后续新增模板（轮式机械、双足/四足等）放入扫描目录后点「刷新」即可自动收录。
    /// </summary>
    public class EXMachTemplateWindow : EditorWindow
    {
        [SerializeField]
        private string scanFolder = "Assets/_EXProceduralMachine/Examples";

        private readonly List<TemplateEntry> _templates = new List<TemplateEntry>();
        private Vector2 _scroll;

        [MenuItem("Tools/EXMach/程序化动画模板列表")]
        public static void Open()
        {
            var window = GetWindow<EXMachTemplateWindow>("EXMach 程序化动画模板");
            window.minSize = new Vector2(460, 340);
            window.Show();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawList();
            DrawFooter();
        }

        // ==================== 头部 ====================

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("程序化动画模板库", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("扫描目录", GUILayout.Width(64));
                scanFolder = EditorGUILayout.TextField(scanFolder);
                if (GUILayout.Button("刷新", GUILayout.Width(56)))
                    Refresh();
            }

            EditorGUILayout.HelpBox(
                $"已发现 {_templates.Count} 个程序化动画模板。点击模板名可定位资产，「实例化到场景」把模板放入当前场景。",
                MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        // ==================== 列表 ====================

        private void DrawList()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            if (_templates.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "未找到模板预制体。\n\n请确认：\n1. 扫描目录存在且包含 .prefab 文件\n2. 模板预制体已正确创建（例如 Examples/SpiderMachine.prefab）\n3. 点击「刷新」重新扫描",
                    MessageType.Warning);
            }

            foreach (var entry in _templates)
            {
                DrawEntry(entry);
                GUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEntry(TemplateEntry entry)
        {
            EditorGUILayout.BeginVertical("box");

            using (new EditorGUILayout.HorizontalScope())
            {
                // 预览缩略图（AssetPreview 异步生成，首帧可能为空）
                var preview = AssetPreview.GetAssetPreview(entry.Prefab);
                if (preview != null)
                {
                    GUILayout.Label(preview, GUILayout.Width(72), GUILayout.Height(72));
                }
                else
                {
                    GUILayout.Box("预览…", GUILayout.Width(72), GUILayout.Height(72));
                    Repaint(); // 等待预览生成
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(4);
                    if (GUILayout.Button(entry.Name, EditorStyles.boldLabel))
                        PingTemplate(entry);

                    EditorGUILayout.LabelField(entry.Path, EditorStyles.miniLabel);
                    if (entry.Components.Length > 0)
                        EditorGUILayout.LabelField("组件: " + string.Join(", ", entry.Components), EditorStyles.miniLabel);

                    GUILayout.Space(4);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("定位资产", GUILayout.Width(72)))
                            PingTemplate(entry);
                        if (GUILayout.Button("实例化到场景", GUILayout.Width(104)))
                            InstantiateTemplate(entry);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        // ==================== 底部 ====================

        private void DrawFooter()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"共 {_templates.Count} 个模板", EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("在资源管理器中打开扫描目录", EditorStyles.miniButton))
                    EditorUtility.RevealInFinder(scanFolder);
            }
        }

        // ==================== 逻辑 ====================

        /// <summary>重新扫描扫描目录下的所有 Prefab 模板</summary>
        private void Refresh()
        {
            _templates.Clear();

            if (string.IsNullOrEmpty(scanFolder) || !Directory.Exists(scanFolder))
                return;

            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { scanFolder });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;

                // 收集挂载的脚本组件（过滤 Transform / Renderer 等内置组件）
                var componentNames = prefab.GetComponentsInChildren<MonoBehaviour>(true)
                    .Select(c => c.GetType().Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToArray();

                _templates.Add(new TemplateEntry
                {
                    Name = prefab.name,
                    Path = path,
                    Components = componentNames,
                    Prefab = prefab
                });
            }
        }

        /// <summary>在 Project 窗口定位并选中模板资产</summary>
        private static void PingTemplate(TemplateEntry entry)
        {
            if (entry.Prefab == null)
                return;

            EditorGUIUtility.PingObject(entry.Prefab);
            Selection.activeObject = entry.Prefab;
        }

        /// <summary>把模板实例化到当前激活场景并选中</summary>
        private static void InstantiateTemplate(TemplateEntry entry)
        {
            if (entry.Prefab == null)
                return;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(entry.Prefab);
            if (instance == null)
            {
                Debug.LogError($"[EXMach] 模板 {entry.Name} 实例化失败，请检查预制体是否有效。");
                return;
            }

            Undo.RegisterCreatedObjectUndo(instance, $"Instantiate {entry.Name}");
            Selection.activeGameObject = instance;
        }

        // ==================== 数据 ====================

        [System.Serializable]
        private class TemplateEntry
        {
            public string Name;
            public string Path;
            public string[] Components;
            public GameObject Prefab;
        }
    }
}
#endif