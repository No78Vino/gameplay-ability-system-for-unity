using System;
using GAS.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GAS.Editor
{
    /// <summary>
    /// 这个类被反射引用到, 重构请小心!!
    /// </summary>
    public class AbilityTimelineEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        private VisualElement _root;


        public static AbilityTimelineEditorWindow Instance { get; private set; }
        public TimelineTrackView TrackView { get; private set; }

        public TimelineInspector TimelineInspector { get; private set; }


        public void CreateGUI()
        {
            Instance = this;
            _root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
            _root.Add(labelFromUxml);

            InitAbilityAssetBar();
            InitTopBar();
            InitController();
            TimerShaftView = new TimerShaftView(_root);
            TrackView = new TimelineTrackView(_root);
            TimelineInspector = new TimelineInspector(_root);
            InitClipInspector();
            InitSplitter();
            InitSyncScrollViews();
        }

        /// <summary>
        /// 这个方法被反射引用到, 重构请小心!!
        /// </summary>
        public static void ShowWindow(TimelineAbilityAssetBase asset)
        {
            var wnd = GetWindow<AbilityTimelineEditorWindow>();
            wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
            wnd.InitAbility(asset);
        }

        public void Save()
        {
            // Debug.Log("数据保存了！");
            // Undo.RecordObject(AbilityAsset, "Change Audio Clip");
            // EditorUtility.SetDirty(AbilityAsset); // 标记资产已修改
            DirtyManager.MarkDirty(AbilityAsset);
            // AbilityAsset.Save(); //标脏后依托unity 的自动保存处理机制不需要手动保存 需要立即生效的修改再调用目前暂时不需要
        }

        private void InitAbility(TimelineAbilityAssetBase asset)
        {
            _abilityAsset.value = asset;
            MaxFrame.value = AbilityAsset.FrameCount;
            CurrentSelectFrameIndex = 0;
            TimerShaftView.RefreshTimerDraw();
            TrackView.RefreshTrackDraw();
        }

        private void SaveAsset()
        {
            EditorUtility.SetDirty(AbilityAsset);
            AssetDatabase.SaveAssetIfDirty(AbilityAsset);
        }

        private void OnDestroy()
        {
            // DragSplitter
            splitter.UnregisterCallback<MouseDownEvent>(OnSplitterMouseDown);
            splitter.UnregisterCallback<MouseUpEvent>(OnSplitterMouseUp);
            splitter.UnregisterCallback<MouseMoveEvent>(OnSplitterMouseMove);
            EditorPrefs.SetFloat("InspectorWidth", rightPanel.resolvedStyle.width);

            // ClipInspector
            if (cachedEditor != null)
                DestroyImmediate(cachedEditor);
            Selection.selectionChanged -= UpdateClipInspector;
        }

        #region Config

        public AbilityTimelineEditorConfig Config { get; } = new();

        private ObjectField _abilityAsset;
        private Button _btnShowAbilityAssetDetail;
        public TimelineAbilityAssetBase AbilityAsset => _abilityAsset.value as TimelineAbilityAssetBase;

        // private TimelineAbilityEditorWindow AbilityAssetEditor => AbilityAsset != null
        //     ? UnityEditor.Editor.CreateEditor(AbilityAsset) as TimelineAbilityEditorWindow
        //     : null;

        private void InitAbilityAssetBar()
        {
            _abilityAsset = _root.Q<ObjectField>("SequentialAbilityAsset");
            _abilityAsset.RegisterValueChangedCallback(OnSequentialAbilityAssetChanged);

            _btnShowAbilityAssetDetail = _root.Q<Button>("BtnShowAbilityAssetDetail");
            _btnShowAbilityAssetDetail.clickable.clicked += ShowAbilityAssetDetail;
        }

        private void OnSequentialAbilityAssetChanged(ChangeEvent<Object> evt)
        {
            if (AbilityAsset != null)
            {
                MaxFrame.value = AbilityAsset.FrameCount;
            }
            else
            {
                Selection.activeObject = null;
            }

            CurrentSelectFrameIndex = 0;
            TimerShaftView.RefreshTimerDraw();
            TrackView.RefreshTrackDraw();
        }

        private void ShowAbilityAssetDetail()
        {
            if (AbilityAsset == null) return;
            Selection.activeObject = AbilityAsset;
        }

        #endregion

        #region TopBar

        private string _previousScenePath;
        private Button BtnLoadPreviewScene;
        private Button BtnBackToScene;
        private Button BtnChildInspector;
        private ObjectField _previewObjectField;
        public GameObject PreviewObject => _previewObjectField.value as GameObject;

        private void InitTopBar()
        {
            BtnLoadPreviewScene = _root.Q<Button>(nameof(BtnLoadPreviewScene));
            BtnLoadPreviewScene.clickable.clicked += LoadPreviewScene;
            BtnBackToScene = _root.Q<Button>(nameof(BtnBackToScene));
            BtnBackToScene.clickable.clicked += BackToScene;

            _previewObjectField = _root.Q<ObjectField>("PreviewInstance");
            _previewObjectField.RegisterValueChangedCallback(OnPreviewObjectChanged);
        }

        private void OnPreviewObjectChanged(ChangeEvent<Object> evt)
        {
            // TODO : 在这里处理预览对象的变化
        }

        private void BackToScene()
        {
            // 判断是否有记录前一个Scene
            if (!string.IsNullOrEmpty(_previousScenePath))
                // 激活前一个Scene
                EditorSceneManager.OpenScene(_previousScenePath);
            else
                Debug.LogWarning("No previous scene available.");
        }

        private void LoadPreviewScene()
        {
            // 记录当前Scene
            _previousScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene("Assets/0_Unicorn/EditRes/pveEditor.unity");

            // 创建一个新的Scene
            //var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            // 在这里添加临时预览的内容，例如放置一些对象
            // 这里只是演示，具体可以根据需求添加你的内容
            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // SceneManager.MoveGameObjectToScene(cube, newScene);
            // 激活新创建的Scene
            //SceneManager.SetActiveScene(newScene);
        }

        #endregion

        #region TimerShaft

        public TimerShaftView TimerShaftView { get; private set; }

        private int _currentMaxFrame;

        public int CurrentMaxFrame
        {
            get => _currentMaxFrame;
            set
            {
                if (AbilityAsset == null)
                {
                    _currentMaxFrame = 0;
                    return;
                }

                if (_currentMaxFrame == value) return;
                _currentMaxFrame = value;
                AbilityAsset.FrameCount = _currentMaxFrame;
                SaveAsset();
                MaxFrame.value = _currentMaxFrame;
                TrackView.UpdateContentSize();
                TimerShaftView.RefreshTimerDraw();
            }
        }

        private int _currentSelectFrameIndex;

        public int CurrentSelectFrameIndex
        {
            get => _currentSelectFrameIndex;
            set
            {
                if (_currentSelectFrameIndex == value) return;
                _currentSelectFrameIndex = Mathf.Clamp(value, 0, MaxFrame.value);
                CurrentFrame.value = _currentSelectFrameIndex;
                TimerShaftView.RefreshTimerDraw();

                EvaluateFrame(_currentSelectFrameIndex);
            }
        }

        public float CurrentFramePos => Mathf.Abs(TimerShaftView.TimeLineContainer.transform.position.x);
        public float CurrentSelectFramePos => _currentSelectFrameIndex * Config.FrameUnitWidth;
        public float CurrentEndFramePos => CurrentMaxFrame * Config.FrameUnitWidth;

        public int GetFrameIndexByPosition(float x)
        {
            return TimerShaftView.GetFrameIndexByPosition(x);
        }

        public int GetFrameIndexByMouse(float x)
        {
            return TimerShaftView.GetFrameIndexByMouse(x);
        }

        #endregion

        #region Controller

        private Button BtnPlay;
        private Button BtnLeftFrame;
        private Button BtnRightFrame;
        private Button BtnLoop;
        private IntegerField CurrentFrame;
        private IntegerField MaxFrame;

        private void InitController()
        {
            BtnPlay = _root.Q<Button>(nameof(BtnPlay));
            BtnPlay.clickable.clicked += OnPlay;

            BtnLeftFrame = _root.Q<Button>(nameof(BtnLeftFrame));
            BtnLeftFrame.clickable.clicked += OnLeftFrame;

            BtnRightFrame = _root.Q<Button>(nameof(BtnRightFrame));
            BtnRightFrame.clickable.clicked += OnRightFrame;

            BtnLoop = _root.Q<Button>(nameof(BtnLoop));
            BtnLoop.clickable.clicked += OnSetloop;

            CurrentFrame = _root.Q<IntegerField>(nameof(CurrentFrame));
            CurrentFrame.RegisterValueChangedCallback(OnCurrentFrameChanged);
            MaxFrame = _root.Q<IntegerField>(nameof(MaxFrame));
            MaxFrame.RegisterValueChangedCallback(OnMaxFrameChanged);
        }

        private void OnMaxFrameChanged(ChangeEvent<int> evt)
        {
            CurrentMaxFrame = evt.newValue;
            MaxFrame.value = CurrentMaxFrame;
        }

        private void OnCurrentFrameChanged(ChangeEvent<int> evt)
        {
            CurrentSelectFrameIndex = evt.newValue;
            CurrentFrame.value = CurrentSelectFrameIndex;
        }

        private void RefreshPlayButton()
        {
            BtnPlay.text = !IsPlaying ? "▶" : "■";
            BtnPlay.style.backgroundColor =
                !IsPlaying ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.1f, 0.8f, 0.1f, 0.5f);
        }

        public void OnPlay()
        {
            if (AbilityAsset == null) return;
            IsPlaying = !IsPlaying;
        }

        private void OnLeftFrame()
        {
            if (AbilityAsset == null) return;
            IsPlaying = false;
            CurrentSelectFrameIndex -= 1;
        }

        private void OnRightFrame()
        {
            if (AbilityAsset == null) return;
            IsPlaying = false;
            CurrentSelectFrameIndex += 1;
        }

        private void OnSetloop()
        {
            IsLoop = !IsLoop;
        }

        #endregion

        #region Clip Inspector

        public object CurrentInspectorObject => TimelineInspector.CurrentInspectorObject;

        public void SetInspector(object target = null)
        {
            if (AbilityAsset == null) return;
            TimelineInspector.SetInspector(target);
        }

        #endregion

        #region TimelinePreview

        private DateTime _startTime;
        private int _startPlayFrameIndex;
        private bool _isPlaying;
        private bool _isLoop;

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                //_isPlaying = CanPlay() && value;
                _isPlaying = value;
                if (_isPlaying)
                {
                    _startTime = DateTime.Now;
                    if (CurrentSelectFrameIndex == CurrentMaxFrame)
                    {
                        CurrentSelectFrameIndex = 0;
                    }
                    _startPlayFrameIndex = CurrentSelectFrameIndex;
                }

                RefreshPlayButton();
            }
        }

        public bool IsLoop
        {
            get => _isLoop;
            private set
            {
                _isLoop = value;
                BtnLoop.style.backgroundColor = value ? new Color(0.1f, 0.8f, 0.1f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        private void Update()
        {
            if (IsPlaying)
            {
                var deltaTime = (DateTime.Now - _startTime).TotalSeconds;
                var frameIndex = (int)(deltaTime * Config.DefaultFrameRate) + _startPlayFrameIndex;
                if (frameIndex > CurrentMaxFrame)
                {
                    if (IsLoop)
                    {
                        frameIndex = 0;
                        _startPlayFrameIndex = 0;
                        _startTime = DateTime.Now;
                    }
                    else
                        IsPlaying = false;
                }

                CurrentSelectFrameIndex = frameIndex;
            }
        }

        private void EvaluateFrame(int frameIndex)
        {
            if (AbilityAsset == null || _previewObjectField.value == null) return;

            foreach (var track in TrackView.TrackList)
                track.TickView(frameIndex);
        }

        private bool CanPlay()
        {
            var canPlay = AbilityAsset != null && _previewObjectField.value != null;
            return canPlay;
        }

        #endregion

        #region DragSplitter

        private VisualElement splitter;
        private VisualElement leftPanel;
        private VisualElement rightPanel;
        private bool isDragging;
        private float initialMouseX;
        private float initialLeftWidth;

        // 添加初始化分隔条的方法
        private void InitSplitter()
        {
            splitter = _root.Q<VisualElement>("Splitter");
            leftPanel = _root.Q<VisualElement>("LeftPanel");
            rightPanel = _root.Q<VisualElement>("InspectorPanel");

            // 加载保存的宽度
            rightPanel.style.width = EditorPrefs.GetFloat("InspectorWidth", 400);

            // 绑定事件
            splitter.RegisterCallback<MouseDownEvent>(OnSplitterMouseDown);
            splitter.RegisterCallback<MouseUpEvent>(OnSplitterMouseUp);
            splitter.RegisterCallback<MouseMoveEvent>(OnSplitterMouseMove);
        }

        // 添加事件处理方法
        private void OnSplitterMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 0)
            {
                isDragging = true;
                initialMouseX = evt.mousePosition.x;
                initialLeftWidth = leftPanel.resolvedStyle.width;
                splitter.CaptureMouse();
                evt.StopPropagation();
            }
        }

        private void OnSplitterMouseUp(MouseUpEvent evt)
        {
            if (isDragging)
            {
                isDragging = false;
                splitter.ReleaseMouse();
                evt.StopPropagation();
                EditorPrefs.SetFloat("InspectorWidth", rightPanel.resolvedStyle.width);
            }
        }

        private void OnSplitterMouseMove(MouseMoveEvent evt)
        {
            if (isDragging)
            {
                float delta = evt.mousePosition.x - initialMouseX;
                float newLeftWidth = initialLeftWidth + delta;
                float newRightWidth = rootVisualElement.resolvedStyle.width - newLeftWidth - splitter.resolvedStyle.width;

                newRightWidth = Mathf.Clamp(newRightWidth, 200, 600);
                rightPanel.style.width = newRightWidth;

                evt.StopPropagation();
            }
        }

        #endregion

        #region ClipInspector

        private IMGUIContainer inspectorContainer;
        private UnityEditor.Editor cachedEditor;

        public void InitClipInspector()
        {
            // 获取ClipInspector容器
            inspectorContainer = _root.Q<IMGUIContainer>("NativeInspector");

            if (Selection.activeObject != null)
            {
                if (cachedEditor != null)
                    DestroyImmediate(cachedEditor);
                cachedEditor = UnityEditor.Editor.CreateEditor(Selection.activeObject);
            }

            inspectorContainer.onGUIHandler = DrawInspectorGUI;

            // 监听选中对象变化
            Selection.selectionChanged += UpdateClipInspector;
        }

        private void UpdateClipInspector()
        {
            // 当选中对象变化时更新
            if (Selection.activeObject != null && Selection.activeObject.GetType().Namespace.StartsWith("GAS.Editor"))
            {
                if (cachedEditor != null)
                    DestroyImmediate(cachedEditor);
                cachedEditor = UnityEditor.Editor.CreateEditor(Selection.activeObject);
                //Debug.Log("SelectionOBJ = "+ Selection.activeObject.GetType().Namespace.StartsWith("GAS.Editor")+" == " + Selection.activeObject.GetType());
            }

            // 触发重绘
            inspectorContainer.MarkDirtyRepaint();
        }

        private void DrawInspectorGUI()
        {
            if (cachedEditor != null)
            {
                cachedEditor.OnInspectorGUI();
            }
            else
            {
                GUILayout.Label("No object selected", EditorStyles.centeredGreyMiniLabel);
            }
        }
        #endregion

        #region SyncScrollViews

        private ScrollView trackMenuScroll;
        private ScrollView mainContentScroll;
        private void InitSyncScrollViews()
        {
            trackMenuScroll = _root.Q<ScrollView>("TrackMenuScroll");
            mainContentScroll = _root.Q<ScrollView>("MainContent");

            // 监听 TrackMenuScroll 的滚动事件
            trackMenuScroll.verticalScroller.valueChanged += value =>
            {
                if (Math.Abs(mainContentScroll.verticalScroller.value - value) > 0.01f)
                {
                    mainContentScroll.verticalScroller.value = value;
                }
            };

            // 监听 MainContentScroll 的滚动事件
            mainContentScroll.verticalScroller.valueChanged += value =>
            {
                if (Math.Abs(trackMenuScroll.verticalScroller.value - value) > 0.01f)
                {
                    trackMenuScroll.verticalScroller.value = value;
                }
            };
        }

        #endregion

    }
}
