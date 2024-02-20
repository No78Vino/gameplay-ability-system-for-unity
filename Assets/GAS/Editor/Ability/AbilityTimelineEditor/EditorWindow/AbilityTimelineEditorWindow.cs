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
    public static void ShowExample()
    {
        AbilityTimelineEditorWindow wnd = GetWindow<AbilityTimelineEditorWindow>();
        wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
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
    }

    #region Config
    private AbilityTimelineEditorConfig _config = new AbilityTimelineEditorConfig();
    #endregion
    
    
    #region AbilityAsset

    private ObjectField SequentialAbilityAsset;
    void InitLeftInspector()
    {
        SequentialAbilityAsset = root.Q<ObjectField>(nameof(SequentialAbilityAsset));
        SequentialAbilityAsset.objectType = typeof(GeneralSequentialAbilityAsset);
        SequentialAbilityAsset.RegisterValueChangedCallback(OnSequentialAbilityAssetChanged);
    }

    private void OnSequentialAbilityAssetChanged(ChangeEvent<Object> evt)
    {
        GeneralSequentialAbilityAsset asset = evt.newValue as GeneralSequentialAbilityAsset;
        if (asset != null)
        {
            Debug.Log(asset.name);
        }
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
    private VisualElement contentViewPort;

    private bool timerShaftMouseIn;
    private int currentSelectFrameIndex;
    private int CurrentSelectFrameIndex
    {
        set
        {
            if (currentSelectFrameIndex == value) return;
            currentSelectFrameIndex = value;
            RefreshTimerDraw();
        }
    }
    
    private float CurrentFramePos => Mathf.Abs(TimeLineContainer.transform.position.x);
    private float CurrentSelectFramePos => currentSelectFrameIndex * _config.frameUnitWidth;
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
    }

    void RefreshTimerDraw()
    {
        TimerShaft.MarkDirtyRepaint();
        SelectLine.MarkDirtyRepaint();
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
        return Mathf.RoundToInt(x + CurrentFramePos) / _config.frameUnitWidth;
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
        _config.frameUnitWidth =
            Mathf.Clamp(_config.frameUnitWidth - deltaY,
                AbilityTimelineEditorConfig.StandardFrameUnitWidth,
                AbilityTimelineEditorConfig.MaxFrameUnitLevel * AbilityTimelineEditorConfig.StandardFrameUnitWidth);
        RefreshTimerDraw();
    }

    private void OnTimerShaftGUI()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        
        var rect = TimerShaft.contentRect;

        int tickStep = AbilityTimelineEditorConfig.MaxFrameUnitLevel + 1 -
                       (_config.frameUnitWidth / AbilityTimelineEditorConfig.StandardFrameUnitWidth);//5;
        tickStep /= 2;
        tickStep = Mathf.Max(tickStep, 1);
        
        int index = Mathf.CeilToInt(CurrentFramePos / _config.frameUnitWidth);
        float startFrameOffset =
            index > 0 ? _config.frameUnitWidth - CurrentFramePos % _config.frameUnitWidth : 0;

        for(var i=startFrameOffset;i<=rect.width;i+=_config.frameUnitWidth)
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
    private TextField CurrentFrame;
    private TextField MaxFrame;

    private void InitController()
    {
        BtnPlay = root.Q<Button>(nameof(BtnPlay));
        BtnPlay.clickable.clicked += OnPlay;
        
        BtnLeftFrame = root.Q<Button>(nameof(BtnLeftFrame));
        BtnLeftFrame.clickable.clicked += OnLeftFrame;
        
        BtnRightFrame = root.Q<Button>(nameof(BtnRightFrame));
        BtnRightFrame.clickable.clicked += OnRightFrame;
        
        CurrentFrame = root.Q<TextField>(nameof(CurrentFrame));
        MaxFrame = root.Q<TextField>(nameof(MaxFrame));
    }
    
    private void OnPlay()
    {
        
    }
    
    private void OnLeftFrame()
    {
        
    }

    private void OnRightFrame()
    {
    }

    #endregion
}

public class AbilityTimelineEditorConfig
{
    public int frameUnitWidth = 10;
    public const int StandardFrameUnitWidth = 10;
    public const int MaxFrameUnitLevel= 10;
}
