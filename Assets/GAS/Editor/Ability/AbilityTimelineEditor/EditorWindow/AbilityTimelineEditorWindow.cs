using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack;
using GAS.Runtime.Ability;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AbilityTimelineEditorWindow : EditorWindow
{
    public static AbilityTimelineEditorWindow Instance { get; private set; }
    
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;


    public static void ShowWindow(GeneralSequentialAbilityAsset asset)
    {
        AbilityTimelineEditorWindow wnd = GetWindow<AbilityTimelineEditorWindow>();
        wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
        wnd.InitAbility(asset);
    }

    private VisualElement _root;
    public void CreateGUI()
    {
        Instance = this;
        _root = rootVisualElement;
        
        // Instantiate UXML
        VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
        _root.Add(labelFromUxml);
        
        InitLeftInspector();
        InitTopBar();
        InitTimerShaft();
        InitController();
        InitTracks();
    }

    private void InitAbility(GeneralSequentialAbilityAsset asset)
    {
        _sequentialAbilityAsset.value = asset;
        MaxFrame.value = AbilityAsset.MaxFrameCount;
        CurrentSelectFrameIndex = 0;
        RefreshTimerDraw();
        RefreshTrackDraw();
    }
    
    private void SaveAsset()
    {
        EditorUtility.SetDirty(AbilityAsset);
        AssetDatabase.SaveAssetIfDirty(AbilityAsset);
    }
    
    #region Config
    private AbilityTimelineEditorConfig _config = new AbilityTimelineEditorConfig();
    public AbilityTimelineEditorConfig Config => _config;
    
    private ObjectField _sequentialAbilityAsset;
    public GeneralSequentialAbilityAsset AbilityAsset => _sequentialAbilityAsset.value as GeneralSequentialAbilityAsset;

    void InitLeftInspector()
    {
        _sequentialAbilityAsset = _root.Q<ObjectField>(("SequentialAbilityAsset"));
        _sequentialAbilityAsset.objectType = typeof(GeneralSequentialAbilityAsset);
        _sequentialAbilityAsset.RegisterValueChangedCallback(OnSequentialAbilityAssetChanged);
    }

    private void OnSequentialAbilityAssetChanged(ChangeEvent<Object> evt)
    {
        GeneralSequentialAbilityAsset asset = evt.newValue as GeneralSequentialAbilityAsset;
        MaxFrame.value = AbilityAsset.MaxFrameCount;
        CurrentSelectFrameIndex = 0;
        RefreshTimerDraw();
        RefreshTrackDraw();
    }

    #endregion
    
    #region TopBar
    private string _previousScenePath;
    private Button BtnLoadPreviewScene;
    private Button BtnBackToScene;

    void InitTopBar()
    {
        BtnLoadPreviewScene = _root.Q<Button>(nameof(BtnLoadPreviewScene));
        BtnLoadPreviewScene.clickable.clicked += LoadPreviewScene;
        BtnBackToScene = _root.Q<Button>(nameof(BtnBackToScene));
        BtnBackToScene.clickable.clicked += BackToScene;
    }

    private void BackToScene()
    {
        // 判断是否有记录前一个Scene
        if (!string.IsNullOrEmpty(_previousScenePath))
        {
            // 激活前一个Scene
            EditorSceneManager.OpenScene(_previousScenePath);
        }
        else
        {
            Debug.LogWarning("No previous scene available.");
        }
    }

    private void LoadPreviewScene()
    {
        // 记录当前Scene
        _previousScenePath = SceneManager.GetActiveScene().path;
        // 创建一个新的Scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        // 在这里添加临时预览的内容，例如放置一些对象
        // 这里只是演示，具体可以根据需求添加你的内容
        // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // SceneManager.MoveGameObjectToScene(cube, newScene);
        // 激活新创建的Scene
        SceneManager.SetActiveScene(newScene);
    }

    #endregion
    
    #region TimerShaft
    
    private IMGUIContainer TimerShaft;
    private VisualElement TimeLineContainer;
    private IMGUIContainer SelectLine;
    private IMGUIContainer FinishLine;
    private VisualElement contentViewPort;

    private bool timerShaftMouseIn;
    private int _currentSelectFrameIndex;
    private int CurrentSelectFrameIndex
    {
        get => _currentSelectFrameIndex;
        set
        {
            if (_currentSelectFrameIndex == value) return;
            _currentSelectFrameIndex = Mathf.Clamp(value, 0, MaxFrame.value);
            CurrentFrame.value = _currentSelectFrameIndex;
            RefreshTimerDraw();
        }
    }

    private int _currentMaxFrame;
    private int CurrentMaxFrame
    {
        get => _currentMaxFrame;
        set
        {
            if (_currentMaxFrame == value) return;
            _currentMaxFrame = value;
            AbilityAsset.MaxFrameCount = _currentMaxFrame;
            SaveAsset();
            MaxFrame.value = _currentMaxFrame;
            UpdateContentSize();
            RefreshTimerDraw();
        }
    }
    
    private float CurrentFramePos => Mathf.Abs(TimeLineContainer.transform.position.x);
    private float CurrentSelectFramePos => _currentSelectFrameIndex * _config.FrameUnitWidth;
    private float CurrentEndFramePos => CurrentMaxFrame * _config.FrameUnitWidth;
    void InitTimerShaft()
    {
        var mainContainer = _root.Q<ScrollView>("MainContent");
        TimeLineContainer = mainContainer.Q<VisualElement>("unity-content-container");
        contentViewPort = mainContainer.Q<VisualElement>("unity-content-viewport");
        
        TimerShaft = _root.Q<IMGUIContainer>(nameof(TimerShaft));
        TimerShaft.onGUIHandler = OnTimerShaftGUI;
        TimerShaft.RegisterCallback<WheelEvent>(OnWheelEvent);
        TimerShaft.RegisterCallback<MouseDownEvent>(OnTimerShaftMouseDown);
        TimerShaft.RegisterCallback<MouseMoveEvent>(OnTimerShaftMouseMove);
        TimerShaft.RegisterCallback<MouseUpEvent>(OnTimerShaftMouseUp);
        TimerShaft.RegisterCallback<MouseOutEvent>(OnTimerShaftMouseOut);
        
        SelectLine = _root.Q<IMGUIContainer>(nameof(SelectLine));
        SelectLine.onGUIHandler = OnSelectLineGUI;
        
        FinishLine = _root.Q<IMGUIContainer>(nameof(FinishLine));
        FinishLine.onGUIHandler = OnFinishLineGUI;
    }

    private void OnFinishLineGUI()
    {
        if (CurrentEndFramePos >= CurrentFramePos)
        {
            Handles.BeginGUI();
            Handles.color = Color.red;
            var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
            var x = CurrentEndFramePos - CurrentFramePos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
            Handles.EndGUI();
        }
    }

    void RefreshTimerDraw()
    {
        TimerShaft.MarkDirtyRepaint();
        SelectLine.MarkDirtyRepaint();
        FinishLine.MarkDirtyRepaint();
    }

    private void OnTimerShaftMouseDown(MouseDownEvent evt)
    {
        timerShaftMouseIn = true;
        CurrentSelectFrameIndex = GetFrameIndexByMouse(evt.localMousePosition.x);
    }
    private void OnTimerShaftMouseUp(MouseUpEvent evt)
    {
        timerShaftMouseIn = false;
    }

    private void OnTimerShaftMouseMove(MouseMoveEvent evt)
    {
        if (timerShaftMouseIn)
        {
            CurrentSelectFrameIndex = GetFrameIndexByMouse(evt.localMousePosition.x);
        }
    }
    
    private void OnTimerShaftMouseOut(MouseOutEvent evt)
    {
    }

    private int GetFrameIndexByMouse(float x)
    {
        return GetFrameIndexByPosition(x + CurrentFramePos);
    }
    
    public int GetFrameIndexByPosition(float x)
    {
        return Mathf.RoundToInt(x) / _config.FrameUnitWidth;
    }
    
    private void OnSelectLineGUI()
    {
        if (CurrentSelectFramePos >= CurrentFramePos)
        {
            Handles.BeginGUI();
            Handles.color = Color.green;
            var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
            var x = CurrentSelectFramePos - CurrentFramePos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
            Handles.EndGUI();
        }
    }

    private void OnWheelEvent(WheelEvent evt)
    {
        int deltaY = (int)evt.delta.y;
        _config.FrameUnitWidth =
            Mathf.Clamp(_config.FrameUnitWidth - deltaY,
                AbilityTimelineEditorConfig.StandardFrameUnitWidth,
                AbilityTimelineEditorConfig.MaxFrameUnitLevel * AbilityTimelineEditorConfig.StandardFrameUnitWidth);
        RefreshTimerDraw();
        UpdateContentSize();    
    }

    private void OnTimerShaftGUI()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        
        var rect = TimerShaft.contentRect;

        int tickStep = AbilityTimelineEditorConfig.MaxFrameUnitLevel + 1 -
                       (_config.FrameUnitWidth / AbilityTimelineEditorConfig.StandardFrameUnitWidth);//5;
        tickStep /= 2;
        tickStep = Mathf.Max(tickStep, 1);
        
        int index = Mathf.CeilToInt(CurrentFramePos / _config.FrameUnitWidth);
        float startFrameOffset =
            index > 0 ? _config.FrameUnitWidth - CurrentFramePos % _config.FrameUnitWidth : 0;

        for(var i=startFrameOffset;i<=rect.width;i+=_config.FrameUnitWidth)
        {
            bool isTick = index % tickStep == 0;
            var x = i;
            var startY = isTick ? rect.height * 0.6f : rect.height * 0.85f;
            var endY = rect.height;
            Handles.DrawLine(new Vector3(x, startY), new Vector3(x, endY));
            
            if (isTick)
            {
                string frameStr = index.ToString();
                Handles.Label(new Vector3(x - frameStr.Length * 4.3f, rect.height * 0.3f), frameStr);
            }
            index++;
        }

        Handles.EndGUI();
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
        MaxFrame.RegisterValueChangedCallback( OnMaxFrameChanged);
    }

    private void OnMaxFrameChanged(ChangeEvent<int> evt)
    {
        CurrentMaxFrame = evt.newValue;
    }

    private void OnCurrentFrameChanged(ChangeEvent<int> evt)
    {
        CurrentSelectFrameIndex = evt.newValue;
    }

    private void OnPlay()
    {
        
    }
    
    private void OnLeftFrame()
    {
        CurrentSelectFrameIndex -= 1;
    }

    private void OnRightFrame()
    {
        CurrentSelectFrameIndex += 1;
    }

    #endregion

    #region Track

    private VisualElement _contentTrackListParent;
    private VisualElement _trackMenuParent;
    
    private void InitTracks()
    {
        _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
        _trackMenuParent = _root.Q<VisualElement>("TrackMenu");
        UpdateContentSize();

        RefreshTrackDraw();
    }

    private void RefreshTrackDraw()
    {
        _contentTrackListParent.Clear();
        _trackMenuParent.Clear();
        var animationTrack = new AnimationTrack();
        animationTrack.Init(_contentTrackListParent, _trackMenuParent,_config.FrameUnitWidth);
    }
    
    private void UpdateContentSize()
    {
        _contentTrackListParent.style.width = CurrentMaxFrame * _config.FrameUnitWidth;
    }
    #endregion
}

