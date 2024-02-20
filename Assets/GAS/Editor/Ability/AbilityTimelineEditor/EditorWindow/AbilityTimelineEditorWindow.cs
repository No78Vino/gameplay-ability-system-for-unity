using GAS.Runtime.Ability;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AbilityTimelineEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("EX-GAS/Ability/AbilityTimelineEditorWindow")]
    public static void Open(GeneralSequentialAbilityAsset asset)
    {
        AbilityTimelineEditorWindow wnd = GetWindow<AbilityTimelineEditorWindow>();
        wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
        wnd.InitAbility(asset);
    }

    private VisualElement root;
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        
        InitLeftInspector();
        InitTopBar();
        InitTimerShaft();
        InitController();
        InitTracks();
    }

    private void InitAbility(GeneralSequentialAbilityAsset asset)
    {
        SequentialAbilityAsset.value = asset;
        MaxFrame.value = asset.MaxFrameCount;
    }
    
    private void SaveAsset()
    {
        EditorUtility.SetDirty(AbilityAsset);
        AssetDatabase.SaveAssetIfDirty(AbilityAsset);
    }
    
    #region Config
    private AbilityTimelineEditorConfig _config = new AbilityTimelineEditorConfig();
    private ObjectField SequentialAbilityAsset;
    private GeneralSequentialAbilityAsset AbilityAsset => SequentialAbilityAsset.value as GeneralSequentialAbilityAsset;

    void InitLeftInspector()
    {
        SequentialAbilityAsset = root.Q<ObjectField>(nameof(SequentialAbilityAsset));
        SequentialAbilityAsset.objectType = typeof(GeneralSequentialAbilityAsset);
        SequentialAbilityAsset.RegisterValueChangedCallback(OnSequentialAbilityAssetChanged);
    }

    private void OnSequentialAbilityAssetChanged(ChangeEvent<Object> evt)
    {
        GeneralSequentialAbilityAsset asset = evt.newValue as GeneralSequentialAbilityAsset;
        RefreshTimerDraw();
    }

    #endregion
    
    #region TopBar
    private string _previousScenePath;
    private Button BtnLoadPreviewScene;
    private Button BtnBackToScene;

    void InitTopBar()
    {
        BtnLoadPreviewScene = root.Q<Button>(nameof(BtnLoadPreviewScene));
        BtnLoadPreviewScene.clickable.clicked += LoadPreviewScene;
        BtnBackToScene = root.Q<Button>(nameof(BtnBackToScene));
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
        var mainContainer = root.Q<ScrollView>("MainContent");
        TimeLineContainer = mainContainer.Q<VisualElement>("unity-content-container");
        contentViewPort = mainContainer.Q<VisualElement>("unity-content-viewport");
        
        TimerShaft = root.Q<IMGUIContainer>(nameof(TimerShaft));
        TimerShaft.onGUIHandler = OnTimerShaftGUI;
        TimerShaft.RegisterCallback<WheelEvent>(OnWheelEvent);
        TimerShaft.RegisterCallback<MouseDownEvent>(OnTimerShaftMouseDown);
        TimerShaft.RegisterCallback<MouseMoveEvent>(OnTimerShaftMouseMove);
        TimerShaft.RegisterCallback<MouseUpEvent>(OnTimerShaftMouseUp);
        TimerShaft.RegisterCallback<MouseOutEvent>(OnTimerShaftMouseOut);
        
        SelectLine = root.Q<IMGUIContainer>(nameof(SelectLine));
        SelectLine.onGUIHandler = OnSelectLineGUI;
        
        FinishLine = root.Q<IMGUIContainer>(nameof(FinishLine));
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
        return Mathf.RoundToInt(x + CurrentFramePos) / _config.FrameUnitWidth;
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
        BtnPlay = root.Q<Button>(nameof(BtnPlay));
        BtnPlay.clickable.clicked += OnPlay;
        
        BtnLeftFrame = root.Q<Button>(nameof(BtnLeftFrame));
        BtnLeftFrame.clickable.clicked += OnLeftFrame;
        
        BtnRightFrame = root.Q<Button>(nameof(BtnRightFrame));
        BtnRightFrame.clickable.clicked += OnRightFrame;
        
        CurrentFrame = root.Q<IntegerField>(nameof(CurrentFrame));
        CurrentFrame.RegisterValueChangedCallback(OnCurrentFrameChanged);
        MaxFrame = root.Q<IntegerField>(nameof(MaxFrame));
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

    private VisualElement ContentTrackList;

    void InitTracks()
    {
        ContentTrackList = root.Q<VisualElement>(nameof(ContentTrackList));
        UpdateContentSize();    
    }

    private void UpdateContentSize()
    {
        ContentTrackList.style.width = (CurrentMaxFrame + 10) * _config.FrameUnitWidth;
    }
    #endregion
}

public class AbilityTimelineEditorConfig
{
    public int FrameUnitWidth = 10;
    public const int StandardFrameUnitWidth = 10;
    public const int MaxFrameUnitLevel= 10;
}
