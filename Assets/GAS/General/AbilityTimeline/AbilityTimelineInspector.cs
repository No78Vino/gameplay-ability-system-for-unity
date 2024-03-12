#if UNITY_EDITOR
namespace GAS.General.AbilityTimeline
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEditor.Timeline;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Object = System.Object;
    
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
                target = Selection.activeObject;
            }
            else
            {
                target = null;
            }
        }
        
        // [BoxGroup("0", false, order: 0)] 
        // [LabelText("预览用的实例单位")]
        // [InlineButton("PreviewDirector", "预览")]
        // [SerializeField]
        // public GeneralSequentialAbilityAsset directorForPreview;
        
        [BoxGroup("0", false, order: 0)] 
        [LabelText("预览用的实例")]
        [InlineButton("PreviewDirector", "预览")]
        [SerializeField]
        public GameObject directorForPreview;

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
            //director.time = _time;
            
            var window = TimelineEditor.GetWindow();
            window.SetTimeline(director);
            //director.Play();
        }
    }
}
#endif