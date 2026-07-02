using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    /// <summary>
    ///     GAS中心 当前Tag总览
    /// </summary>
    public class GASCenterViewTag
    {
        
        [TitleGroup("Tag总览")]
        [HorizontalGroup("Tag总览/A")]
        [Button("打开Tag Excel文件所在文件夹")]
        void OpenTagExcelFileExplore()
        {
            var tagExcelFilePath = _settingAsset.PathOfExcelTag;
            if (File.Exists(tagExcelFilePath))
            {
                if (tagExcelFilePath != null)
                    EditorUtility.RevealInFinder(tagExcelFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("Tag总览/A")]
        [Button("打开Tag Json文件所在文件夹")]
        void OpenTagJsonFileExplore()
        {
            var tagJsonFilePath = _settingAsset.PathOfJsonTag;
            if (File.Exists(tagJsonFilePath))
            {
                if (tagJsonFilePath != null)
                    EditorUtility.RevealInFinder(tagJsonFilePath);
            }
            else
                EditorUtility.DisplayDialog("错误", "Tag JSON文件未找到，请检查设置。", "确定");
        }
        
        [HorizontalGroup("Tag总览/A")]
        [Button("导出更新Json表")]
        void ExportJson()
        {
            CodeGenerator.GenerateGasConfigTables();
        }
        [HorizontalGroup("Tag总览/A")]
        [Button("刷新",Icon = SdfIconType.Recycle)]
        void RefreshUI()
        {
            BuildMenuTree();
        }

        [HorizontalGroup("Tag总览/A")]  
        [Button("🌐 Web编辑器")]  
        void OpenTagWebEditor()  
        {  
            GASWebEditorManager.LaunchTagWebEditor();  
        }
        
        // 添加分割线控制参数
        private static float _splitterPosition = 300f;
        private static bool _isDraggingSplitter;
        private readonly GASSettingAsset _settingAsset;
        private OdinMenuTree _menuTree;
        private TagDesc _selectedTagDesc;
        private TagInEditor[] _tags;

        public GASCenterViewTag()
        {
            _settingAsset = GASSettingAsset.LoadOrCreate();
            BuildMenuTree();
        }

        private void BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.Config.DrawSearchToolbar = true;
            tree.Selection.SelectionChanged += OnSelectionChanged;

            var tagJsonFilePath = _settingAsset.PathOfJsonTag;
            // 检查文件是否存在
            if (!File.Exists(tagJsonFilePath))
            {
                // 文件不存在时也要构建（空）菜单树并赋值给 _menuTree，否则 _menuTree 会一直为 null，
                // 而 DrawMenuTreeArea 在 OnInspectorGUI 中每帧检测到 null 就会重新调用本方法，
                // 从而反复弹窗/打印日志形成死循环。这里改为只记录一次警告，并在面板中以 HelpBox 提示。
                Debug.LogWarning($"[EX-GAS] Tag JSON文件未找到，将以空列表显示: {tagJsonFilePath}");
                _tags = new TagInEditor[0];
                _menuTree = tree;
                return;
            }

            var tagJsonText = File.ReadAllText(tagJsonFilePath);
            _tags = GasJsonReader.ReadTags(tagJsonText);
            foreach (var tag in _tags)
            {
                var tagMenu = tag.name;
                tagMenu = tagMenu.Replace('.', '/'); // 替换点为斜杠
                var so = ScriptableObject.CreateInstance<TagDesc>();
                so.Init(tag.id, tag.name, tag.desc);
                tree.Add(tagMenu, so);
            }

            _menuTree = tree;
        }

        [OnInspectorGUI]
        protected void Draw()
        {
            EditorGUILayout.Space(10);
            // 2. 主内容区域（水平布局）
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            // 3. 左侧菜单树区域
            DrawMenuTreeArea();
            // 4. 可调节分割线
            DrawSplitter();
            // 5. 右侧Inspector区域
            DrawInspectorArea();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMenuTreeArea()
        {
            // 使用固定宽度，但保留最小宽度限制
            EditorGUILayout.BeginVertical(GUILayout.Width(_splitterPosition), GUILayout.MinWidth(150));
            if (_menuTree == null) BuildMenuTree();
            if (_tags == null || _tags.Length == 0)
                EditorGUILayout.HelpBox("Tag JSON文件未找到或为空，请先生成配置表（点击上方“导出更新Json表”），再点击“刷新”。",
                    MessageType.Warning);
            _menuTree?.DrawMenuTree();
            EditorGUILayout.EndVertical();
        }

        private void DrawSplitter()
        {
            var splitterRect = GUILayoutUtility.GetRect(3, EditorGUIUtility.singleLineHeight,
                GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(splitterRect, Color.grey);
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && splitterRect.Contains(Event.current.mousePosition))
                _isDraggingSplitter = true;
            if (_isDraggingSplitter)
                _splitterPosition = Mathf.Clamp(Event.current.mousePosition.x, 150, 800);
            if (Event.current.type == EventType.MouseUp) _isDraggingSplitter = false;
        }

        private void DrawInspectorArea()
        {
            // 使用剩余宽度，设置最小宽度
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            if (_selectedTagDesc != null)
            {
                EditorGUILayout.BeginVertical("box");
                var editor = UnityEditor.Editor.CreateEditor(_selectedTagDesc);
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.HelpBox("请从左侧列表中选择一个Tag进行查看", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }


        private void OnSelectionChanged(SelectionChangedType selectionChangedType)
        {
            // 当选择改变时，更新当前选中的Tag描述
            var selectedTag = _menuTree.Selection.SelectedValue as TagDesc;
            if (selectedTag != null) _selectedTagDesc = selectedTag;
        }

        public class TagDesc : ScriptableObject
        {
            private string _desc;
            private int _id;
            private string _name;

            [LabelText("ID")]
            [LabelWidth(150)]
            [ShowInInspector]
            [DisplayAsString(EnableRichText = true)]
            public string ID => $"<color=white>{_id}</color>";

            [LabelText("标签名")]
            [LabelWidth(150)]
            [ShowInInspector]
            [DisplayAsString(EnableRichText = true)]
            public string Name => $"<color=white>{_name}</color>";

            [LabelText("Tag描述")]
            [LabelWidth(150)]
            [ShowInInspector]
            [DisplayAsString(EnableRichText = true)]
            public string Description => $"<color=white>{_desc}</color>";

            public void Init(int id, string name, string desc)
            {
                _id = id;
                _name = name;
                _desc = desc;
            }
        }
    }
}