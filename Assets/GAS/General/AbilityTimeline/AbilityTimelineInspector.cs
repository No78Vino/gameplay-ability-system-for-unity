using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = System.Object;

#if UNITY_EDITOR
namespace GAS.General.AbilityTimeline
{
    public class AbilityTimelineInspector: OdinEditorWindow
    {
        private static TimelineEditorWindow _timelineEditorWindow;
        private static TimelineAsset _currentTimeline;

        public static AbilityTimelineInspector Open(TimelineAsset timeline)
        {
            _currentTimeline = timeline;
            return GetWindow<AbilityTimelineInspector>();
        }
        
        private void Awake()
        {
            if(_timelineEditorWindow == null)
                _timelineEditorWindow = TimelineEditor.GetWindow();
            
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeObject is PlayableAsset ||
                Selection.activeObject is ScriptableObject)
            {
                //_timelineEditorWindow.SetCurrentTimeline((TimelineAsset)Selection.activeObject);
                target = Selection.activeObject;
            }
            else
            {
                target = null;
            }
        }
        
        float _time = 3.6f;

        [BoxGroup("0", false, order: 0)] 
        [LabelText("预览用的实例单位")]
        [InlineButton("PreviewDirector", "预览")]
        [SceneObjectsOnly]
        public GameObject directorForPreview;
        
        [BoxGroup("A", false, order: 1)] [Title("时间轴信息")]
        [HideLabel][DisplayAsString(TextAlignment.Left,true)]
        public string blank = "<size=6>  </size>";
        
        [BoxGroup("A",false,order:1)]
        [DisplayAsString(TextAlignment.Left,true)]
        [ShowInInspector]
        private string FrameInfo => $"<size=13><color=white>开始 -> {_time}s/0f </color></size>";

        [BoxGroup("B",false,order:2)]
        [Title("面板属性")] 
        [ShowInInspector]
        [InlineEditor(InlineEditorModes.FullEditor,InlineEditorObjectFieldModes.Foldout,Expanded = true,DrawHeader = false)]
        private Object target;
        
        void PreviewDirector()
        {
            if (directorForPreview == null)
            {
                Debug.LogError("[feat] 没有设置预览用的实例单位！");
                return;
            }
            var director = directorForPreview.GetComponent<PlayableDirector>();
            if (director == null)
            {
                director= directorForPreview.AddComponent<PlayableDirector>();
            }

            director.playableAsset = _currentTimeline;
            director.time = _time;
            director.Play();
        }
    }
}
#endif