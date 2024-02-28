using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackMarkBase:TrackItemBase
    {
        private static string MarkAssetGuid => "5a3b3360bcba29b4cac2875f518af19d";
        
        protected VisualElement ve;
        public VisualElement Ve => ve;
        public float FrameUnitWidth { get;protected set; }
        public int StartFrameIndex=>markData.startFrame;
        
        protected MarkEventBase markData;
        public MarkEventBase MarkData => markData;
        
        protected TrackBase trackBase;
        public TrackBase TrackBase => trackBase;


        public Label ItemLabel { get;protected set; }

        public virtual void InitTrackMark(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            MarkEventBase markData)
        {
            trackBase = track;
            FrameUnitWidth = frameUnitWidth;
            this.markData = markData;
            
            string markAssetPath = AssetDatabase.GUIDToAssetPath(MarkAssetGuid);
            ve = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(markAssetPath).Instantiate().Query().ToList()[1];
            ItemLabel = ve.Q<Label>("MarkText");
            parent.Add(ve);
            
            if(AbilityTimelineEditorWindow.Instance.CurrentInspectorObject is TrackMarkBase markBase &&
               markData == markBase.markData)
                OnSelect();
            else
                OnUnSelect();
        }

        public abstract VisualElement Inspector();

        public abstract void Delete();

        public virtual void RefreshShow(float newFrameUnitWidth)
        {
            FrameUnitWidth = newFrameUnitWidth;
            
            // 位置
            var mainPos = ve.transform.position;
            mainPos.x = StartFrameIndex * FrameUnitWidth;
            ve.transform.position = mainPos;
        }
        
        public abstract void UpdateMarkDataFrame(int newStartFrame);
        
        #region Select
        public bool Selected { get; private set; }
        private static Color SelectedColor = new Color(0.8f, 0.6f, 0.3f, 1f);
        private static Color UnSelectedColor = new Color(0.9f, 0.5f, 0.5f, 0.5f);
        public void OnSelect()
        {
            Selected = true;
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
            ve.style.backgroundColor = SelectedColor;
        }

        public void OnUnSelect()
        {
            Selected = false;
            ve.style.backgroundColor = UnSelectedColor;
        }

        #endregion
    }
    
    public abstract class TrackMark<T>:TrackMarkBase where T : TrackBase
    {
        protected T track;

        public override void InitTrackMark(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            MarkEventBase markData)
        {
            this.track = (T) track;
            base.InitTrackMark(track, parent, frameUnitWidth, markData);
            
            RefreshShow(FrameUnitWidth);
        }
       
    }
}