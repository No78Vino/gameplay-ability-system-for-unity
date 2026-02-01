using System;
using GAS.General;
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

        private static EditorWindow _childInspector;

        private const float AUTO_SAVE_INTERVAL = 10f;
        // 上一次自动保存的时间
        private double _lastAutoSaveTime;

        // 当前是否启用自动保存（随焦点开启/关闭）
        private bool _autoSaveRunning = true;
        
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
            
            TimelineInspector = new TimelineInspector(_root);
            TimerShaftView = new TimerShaftView(_root);
            TrackView = new TimelineTrackView(_root);

            OnUpdateShow();
        }

        private void OnGUI()
        {
            HandleShortcut();
        }

        /// <summary>
        /// 这个方法被反射引用到, 重构请小心!!
        /// </summary>
        [MenuItem("EXTool/EX-GAS/时间轴技能编辑器")]
        public static void ShowWindow()
        {
            GeneralGasChoiceHelper.LoadCache();
            var wnd = GetWindow<AbilityTimelineEditorWindow>();
            wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
            // 打开子Inspector
            EditorApplication.delayCall += () => wnd.ShowChildInspector();
        }

        public void Save()
        {
            GasAbilityTimelineXlsxReadWrite.Write();
        }

        private void OnUpdateShow()
        {
            CurrentSelectFrameIndex = 0;
            _currentMaxFrame = AbilityConfig?.LifeTime ?? 0;
            MaxFrame.value = AbilityConfig?.LifeTime ?? 0;
            _toggleManualEndAbility.value = AbilityConfig?.ManualEndAbility ?? false;
            _abilityName.value = AbilityConfig?.Name ?? string.Empty;
            TimerShaftView.RefreshTimerDraw();
            TrackView.RefreshTrackDraw();
        }
        #region Config

        public AbilityTimelineEditorConfig Config { get; } = new();
        
        private DropdownField _dropDownAbilityID;
        private Toggle _toggleManualEndAbility;
        private TextField _abilityName;

        public XParamTimeline AbilityConfig => GasAbilityTimelineXlsxReadWrite.GetTimelineAbility(_dropDownAbilityID.value);

        private void InitAbilityAssetBar()
        {
            _dropDownAbilityID = _root.Q<DropdownField>("DropdownField");
            _toggleManualEndAbility = _root.Q<Toggle>("ManualEnd");
            _abilityName = _root.Q<TextField>("AbilityName");
            
            _toggleManualEndAbility.RegisterValueChangedCallback(OnToggleManualEndAbility);
            _abilityName.RegisterValueChangedCallback(OnAbilityNameChanged);
            _abilityName.isDelayed = true;
            
            _dropDownAbilityID.choices = GasAbilityTimelineXlsxReadWrite.GetTimelineAbilityIDList();
            _dropDownAbilityID.index = _dropDownAbilityID.choices.Count > 0 ? 0 : -1;
            _dropDownAbilityID.RegisterValueChangedCallback(OnAbilityIDChanged);

            var btnSave = _root.Q<Button>("btn_save");
            btnSave.clicked += OnClickSave;
        }

        private void OnAbilityIDChanged(ChangeEvent<string> evt)
        {
            if (AbilityConfig == null)
                Selection.activeObject = null;

            OnUpdateShow();
        }

        private void OnToggleManualEndAbility(ChangeEvent<bool> evt)
        {
            if (AbilityConfig == null) return;
            AbilityConfig.SetManualEndAbility(evt.newValue);
        }
        
        private void OnAbilityNameChanged(ChangeEvent<string> evt)
        {
            if (AbilityConfig == null) return;
            AbilityConfig.SetName(evt.newValue);
        }
        
        private void OnClickSave()
        {
            if (AbilityConfig == null) return;
            Save();
            Debug.Log("TimelineAbility保存成功");
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

            BtnChildInspector = _root.Q<Button>(nameof(BtnChildInspector));
            BtnChildInspector.clickable.clicked += ShowChildInspector;

            _previewObjectField = _root.Q<ObjectField>("PreviewInstance");
            _previewObjectField.RegisterValueChangedCallback(OnPreviewObjectChanged);
        }

        private void ShowChildInspector()
        {
            if (_childInspector == null)
            {
                _childInspector = GetInspectTarget();
                _childInspector.Show();
            }

            EditorApplication.delayCall += () =>
                DockUtilities.DockWindow(this, _childInspector, DockUtilities.DockPosition.Right);
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
            // 创建一个新的Scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            // 在这里添加临时预览的内容，例如放置一些对象
            // 这里只是演示，具体可以根据需求添加你的内容
            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // SceneManager.MoveGameObjectToScene(cube, newScene);
            // 激活新创建的Scene
            SceneManager.SetActiveScene(newScene);
        }

        #endregion

        #region TimerShaft

        public TimerShaftView TimerShaftView { get; private set; }

        private int _currentMaxFrame;

        public int CurrentMaxFrame
        {
            get => _currentMaxFrame;
            private set
            {
                if (AbilityConfig == null)
                {
                    _currentMaxFrame = 0;
                    return;
                }

                if (_currentMaxFrame == value) return;
                _currentMaxFrame = value;
                AbilityConfig.SetLifeTime(_currentMaxFrame);
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
            BtnPlay.text = !IsPlaying ? "▶" : "⏹";
            BtnPlay.style.backgroundColor =
                !IsPlaying ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.1f, 0.8f, 0.1f, 0.5f);
        }

        private void OnPlay()
        {
            if (AbilityConfig == null) return;
            IsPlaying = !IsPlaying;
        }

        private void OnLeftFrame()
        {
            if (AbilityConfig == null) return;
            IsPlaying = false;
            CurrentSelectFrameIndex -= 1;
        }

        private void OnRightFrame()
        {
            if (AbilityConfig == null) return;
            IsPlaying = false;
            CurrentSelectFrameIndex += 1;
        }

        #endregion

        #region Clip Inspector

        public object CurrentInspectorObject => TimelineInspector??TimelineInspector.CurrentInspectorObject;

        public void SetInspector(object target = null)
        {
            if (AbilityConfig == null) return;
            TimelineInspector.SetInspector(target);
        }

        #endregion

        #region TimelinePreview

        private DateTime _startTime;
        private int _startPlayFrameIndex;
        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                _isPlaying = CanPlay() && value;

                if (_isPlaying)
                {
                    _startTime = DateTime.Now;
                    _startPlayFrameIndex = CurrentSelectFrameIndex;
                }

                RefreshPlayButton();
            }
        }

        private void Update()
        {
            if (IsPlaying)
            {
                var deltaTime = (DateTime.Now - _startTime).TotalSeconds;
                var frameIndex = (int)(deltaTime * Config.DefaultFrameRate) + _startPlayFrameIndex;
                if (frameIndex >= CurrentMaxFrame)
                {
                    frameIndex = CurrentMaxFrame;
                    IsPlaying = false;
                }

                CurrentSelectFrameIndex = frameIndex;
            }
        }

        private void EvaluateFrame(int frameIndex)
        {
            if (AbilityConfig == null || _previewObjectField.value == null) return;

            foreach (var track in TrackView.TrackList)
                track.TickView(frameIndex);
        }

        private bool CanPlay()
        {
            var canPlay = AbilityConfig != null && _previewObjectField.value != null;
            return canPlay;
        }

        #endregion


        #region Another Inspector

        private static EditorWindow GetInspectTarget(Object targetGO = null)
        {
            Type inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            EditorWindow inspectorInstance = CreateInstance(inspectorType) as EditorWindow;
            if (targetGO) Selection.activeObject = targetGO;
            return inspectorInstance;
        }

        #endregion
        
        
        private void HandleShortcut()
        {
            var e = Event.current;
            if (e == null) return;
            // 只在 KeyDown 事件中检查
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.S)
            {
                // 调用你的保存逻辑
                Save();
                // 阻止 Unity 默认的 Ctrl+S（保存场景/工程）
                e.Use();
            }
        }
    }
}