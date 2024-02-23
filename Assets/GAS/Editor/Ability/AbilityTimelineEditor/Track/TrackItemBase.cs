using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackItem<T> : TrackItemBase where T : TrackBase
    {
        protected T track;

        public virtual void InitTrackItem(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            TrackEventBase trackEvent)
        {
            this.track = (T)track;
            this.frameUnitWidth = frameUnitWidth;
            this.trackEvent = trackEvent;

            var path = AssetDatabase.GUIDToAssetPath(ItemAssetGUID);
            Item = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path).Instantiate().Query().ToList()[1];
     
            parent.Add(Item);
            startFrameIndex = trackEvent.startFrame;

            OnUnSelect();
        }
    }

    public abstract class TrackItemBase
    {
        public float frameUnitWidth;
        protected VisualElement Item;
        protected int startFrameIndex;
        protected TrackEventBase trackEvent;
        protected abstract string ItemAssetGUID { get; }
        public abstract VisualElement Inspector();
        public abstract void Delete();

        public virtual void RefreshShow(float newFrameUnitWidth)
        {
            frameUnitWidth = newFrameUnitWidth;
        }

        protected void CheckStyle(VisualElement element)
        {
            element.style.fontSize = 14;
            //element.style.width = new StyleLength(1);
        }

        #region Select

        protected static readonly Color NormalColor = new(0, 0.8f, 0.1f, 0.5f);
        protected static readonly Color SelectColor = new(1, 0.8f, 0.1f, 0.5f);

        public virtual void Select()
        {
            // 更新小面板
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        public virtual void OnSelect()
        {
            Item.style.backgroundColor = SelectColor;
        }

        public virtual void OnUnSelect()
        {
            Item.style.backgroundColor = NormalColor;
        }

        #endregion
    }
}
#endif