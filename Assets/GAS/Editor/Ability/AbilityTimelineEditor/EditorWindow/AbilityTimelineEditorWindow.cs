using System;
using System.Collections.Generic;
using GAS.Editor.Ability;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack;
using GAS.Runtime.Ability;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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
        
        InitAbilityAssetBar();
        InitTopBar();
        InitTimerShaft();
        InitController();
        InitTracks();
        InitClipInspector();
    }

    public void Save() => AbilityAsset.Save();

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

    private GeneralSequentialAbilityEditorWindow AbilityAssetEditor => AbilityAsset != null
        ? Editor.CreateEditor(AbilityAsset) as GeneralSequentialAbilityEditorWindow
        : null;

    void InitAbilityAssetBar()
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
    private ObjectField _previewObjectField;

    void InitTopBar()
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
    private IMGUIContainer DottedLine;
    private VisualElement contentViewPort;

    private bool timerShaftMouseIn;
    private int _currentSelectFrameIndex;
    public int CurrentSelectFrameIndex
    {
        get => _currentSelectFrameIndex;
        set
        {
            if (_currentSelectFrameIndex == value) return;
            _currentSelectFrameIndex = Mathf.Clamp(value, 0, MaxFrame.value);
            CurrentFrame.value = _currentSelectFrameIndex;
            RefreshTimerDraw();
            
            EvaluateFrame(_currentSelectFrameIndex);
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

    private int _dottedLineFrameIndex = -1;
    public int DottedLineFrameIndex
    {
        get => _dottedLineFrameIndex;
        set
        {
            if (_dottedLineFrameIndex == value) return;
            _dottedLineFrameIndex = value;
            bool showDottedLine = _dottedLineFrameIndex* _config.FrameUnitWidth >= CurrentFramePos &&
                                  _dottedLineFrameIndex* _config.FrameUnitWidth < CurrentFramePos + TimerShaft.contentRect.width;
            DottedLine.style.display = showDottedLine ? DisplayStyle.Flex : DisplayStyle.None;
            DottedLine.MarkDirtyRepaint();

            // AbilityAsset.MaxFrameCount = _currentMaxFrame;
            // SaveAsset();
            // MaxFrame.value = _currentMaxFrame;
            // UpdateContentSize();
            // RefreshTimerDraw();
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
        
        DottedLine = _root.Q<IMGUIContainer>(nameof(DottedLine));
        DottedLine.onGUIHandler = OnDottedLineGUI;
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
        if (CurrentSelectFramePos >= CurrentFramePos && CurrentSelectFramePos< CurrentFramePos + TimerShaft.contentRect.width)
        {
            Handles.BeginGUI();
            Handles.color = Color.green;
            var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
            var x = CurrentSelectFramePos - CurrentFramePos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
            Handles.EndGUI();
        }
    }

    private void OnFinishLineGUI()
    {
        if (CurrentEndFramePos >= CurrentFramePos && CurrentEndFramePos < CurrentFramePos + TimerShaft.contentRect.width)
        {
            Handles.BeginGUI();
            Handles.color = Color.red;
            var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
            var x = CurrentEndFramePos - CurrentFramePos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, length));
            Handles.EndGUI();
        }
    }
    
    private void OnDottedLineGUI()
    {
        var dottedLinePos =  DottedLineFrameIndex * _config.FrameUnitWidth;
        if (dottedLinePos >= CurrentFramePos &&
            dottedLinePos < CurrentFramePos + TimerShaft.contentRect.width)
        {
            Handles.BeginGUI();
            Handles.color = new Color(1, 0.5f, 0, 1);
            var length = contentViewPort.contentRect.height + TimerShaft.contentRect.height;
            var x = dottedLinePos - CurrentFramePos;

            float lineUnitSize = 10f;
            int lineUnitsCount = 0;
            for (float i = 0; i < length; i += lineUnitSize)
            {
                if (lineUnitsCount++ % 2 == 0)
                    Handles.DrawLine(new Vector3(x, i), new Vector3(x, i + lineUnitSize));
            }

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

    void RefreshPlayButton()
    {
        BtnPlay.text = !IsPlaying?"▶":"⏹";
        BtnPlay.style.backgroundColor = !IsPlaying?new Color(0.5f, 0.5f, 0.5f, 0.5f):new Color(0.1f, 0.8f, 0.1f, 0.5f);
    }
    
    private void OnPlay()
    {
        IsPlaying = !IsPlaying;
    }
    
    private void OnLeftFrame()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex -= 1;
    }

    private void OnRightFrame()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex += 1;
    }

    #endregion

    #region Track

    private VisualElement _contentTrackListParent;
    private VisualElement _trackMenuParent;
    private AnimationTrack animationTrack;
    List<TrackBase> _trackList = new List<TrackBase>();
    private void InitTracks()
    {
        _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
        _trackMenuParent = _root.Q<VisualElement>("TrackMenu");
        RefreshTrackDraw();
        UpdateContentSize();
    }

    private void RefreshTrackDraw()
    {
        _trackList.Clear();
        _contentTrackListParent.Clear();
        _trackMenuParent.Clear();
        animationTrack = new AnimationTrack();
        animationTrack.Init(_contentTrackListParent, _trackMenuParent,_config.FrameUnitWidth);
        _trackList.Add(animationTrack);
    }
    
    private void UpdateContentSize()
    {
        _contentTrackListParent.style.width = CurrentMaxFrame * _config.FrameUnitWidth;
        foreach (var track in _trackList)
        {
            track.RefreshShow(_config.FrameUnitWidth);
        }
        
    }
    #endregion

    #region Clip Inspector

    private VisualElement _clipInspector;
    public object CurrentInspectorObject;
    void InitClipInspector()
    {
        _clipInspector = _root.Q<VisualElement>("ClipInspector");
        SetInspector();
    }

    public void SetInspector(object target=null)
    {
        if (CurrentInspectorObject == target) return;
        if (CurrentInspectorObject != null)
        {
            if (CurrentInspectorObject is TrackClipBase oldTrackItem) oldTrackItem.Ve.OnUnSelect();
            if (CurrentInspectorObject is TrackBase oldTrack) oldTrack.OnUnSelect();
        }

        CurrentInspectorObject = target;
        _clipInspector.Clear();
        switch (CurrentInspectorObject)
        {
            case null:
                return;
            case TrackClipBase trackClip:
                _clipInspector.Add(trackClip.Inspector());
                //trackClip.Ve.OnSelect();
                break;
            case TrackBase track:
                //track.OnSelect();
                break;
        }
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
            var frameIndex = (int)(deltaTime * _config.DefaultFrameRate) + _startPlayFrameIndex;
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
        if (AbilityAsset != null && _previewObjectField.value != null)
        {
            // TODO : 在这里处理预览对象的动画,特效等等
            animationTrack.TickView(frameIndex, _previewObjectField.value as GameObject);
        }
    }

    bool CanPlay()
    {
        bool canPlay= AbilityAsset != null && _previewObjectField.value != null;
        return canPlay;
    }

    #endregion
}

