using System;
using System.Collections.Generic;
using System.Reflection;
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
    
    public static void ShowWindow(TimelineAbilityAsset asset)
    {
        AbilityTimelineEditorWindow wnd = GetWindow<AbilityTimelineEditorWindow>();
        wnd.titleContent = new GUIContent("AbilityTimelineEditorWindow");
        wnd.InitAbility(asset);
    }

    private VisualElement _root;
    
    
    private TimelineTrackView _trackView;
    public TimelineTrackView TrackView => _trackView;
    
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
        _timerShaftView = new TimerShaftView(_root);
        _trackView = new TimelineTrackView(_root);
        InitClipInspector();
    }

    public void Save() => AbilityAsset.Save();

    private void InitAbility(TimelineAbilityAsset asset)
    {
        _abilityAsset.value = asset;
        MaxFrame.value = AbilityAsset.MaxFrameCount;
        CurrentSelectFrameIndex = 0;
        _timerShaftView.RefreshTimerDraw();
        _trackView.RefreshTrackDraw();
    }
    
    private void SaveAsset()
    {
        EditorUtility.SetDirty(AbilityAsset);
        AssetDatabase.SaveAssetIfDirty(AbilityAsset);
    }
    
    public void TrackMenusUnSelect()
    {
        _trackView.TrackMenusUnSelect();
    }
    #region Config
    private AbilityTimelineEditorConfig _config = new AbilityTimelineEditorConfig();
    public AbilityTimelineEditorConfig Config => _config;
    
    private ObjectField _abilityAsset;
    private Button _btnShowAbilityAssetDetail;
    public TimelineAbilityAsset AbilityAsset => _abilityAsset.value as TimelineAbilityAsset;

    private TimelineAbilityEditorWindow AbilityAssetEditor => AbilityAsset != null
        ? Editor.CreateEditor(AbilityAsset) as TimelineAbilityEditorWindow
        : null;

    void InitAbilityAssetBar()
    {
        _abilityAsset = _root.Q<ObjectField>(("SequentialAbilityAsset"));
        _abilityAsset.RegisterValueChangedCallback(OnSequentialAbilityAssetChanged);
        
        _btnShowAbilityAssetDetail = _root.Q<Button>("BtnShowAbilityAssetDetail");
        _btnShowAbilityAssetDetail.clickable.clicked += ShowAbilityAssetDetail;
    }

    private void OnSequentialAbilityAssetChanged(ChangeEvent<Object> evt)
    {
        TimelineAbilityAsset asset = evt.newValue as TimelineAbilityAsset;
        MaxFrame.value = AbilityAsset.MaxFrameCount;
        CurrentSelectFrameIndex = 0;
        _timerShaftView.RefreshTimerDraw();
        _trackView.RefreshTrackDraw();
    }

    private void ShowAbilityAssetDetail()
    {
        if (AbilityAsset == null) return;
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        EditorWindow inspectorInstance = CreateInstance(inspectorType) as EditorWindow;
        Object prevSelection = Selection.activeObject;
        Selection.activeObject = AbilityAsset;
        var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
        if (isLocked != null) isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });
        Selection.activeObject = prevSelection;
        if (inspectorInstance != null) inspectorInstance.Show();
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

    private TimerShaftView _timerShaftView;
    public TimerShaftView TimerShaftView => _timerShaftView;
    
    private int _currentMaxFrame;
    public int CurrentMaxFrame
    {
        get => _currentMaxFrame;
        private set
        {
            if (_currentMaxFrame == value) return;
            _currentMaxFrame = value;
            AbilityAsset.MaxFrameCount = _currentMaxFrame;
            SaveAsset();
            MaxFrame.value = _currentMaxFrame;
            _trackView.UpdateContentSize();
            _timerShaftView.RefreshTimerDraw();
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
            _timerShaftView.RefreshTimerDraw();

            EvaluateFrame(_currentSelectFrameIndex);
        }
    }
    
    public float CurrentFramePos => Mathf.Abs(_timerShaftView.TimeLineContainer.transform.position.x);
    public float CurrentSelectFramePos => _currentSelectFrameIndex * _config.FrameUnitWidth;
    public float CurrentEndFramePos => CurrentMaxFrame * _config.FrameUnitWidth;

    public int GetFrameIndexByPosition(float x) => _timerShaftView.GetFrameIndexByPosition(x);
    
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
                break;
            case TrackBase track:
                _clipInspector.Add(track.Inspector());
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
            _trackView.animationTrack.TickView(frameIndex, _previewObjectField.value as GameObject);
        }
    }

    bool CanPlay()
    {
        bool canPlay= AbilityAsset != null && _previewObjectField.value != null;
        return canPlay;
    }

    #endregion
}