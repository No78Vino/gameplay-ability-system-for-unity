
#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using Runtime;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public abstract class TrackBase
    {
        protected float _frameWidth;
        protected TrackDataBase _trackData;
        protected List<TrackItemBase> _trackItems = new();
        protected VisualElement BoundingBox;
        protected VisualElement Lock;
        protected VisualElement MenuBox;
        protected VisualElement MenuParent;
        public VisualElement MenuRoot;
        public Label MenuText;
        protected VisualElement TrackParent;
        public VisualElement TrackRoot;
        public List<TrackItemBase> TrackItems => _trackItems;
        public VisualElement Track { get; protected set; }

        protected virtual string TrackAssetGuid => "67e1b3c42dcc09a4dbb9e9b107500dfd";
        protected virtual string MenuAssetGuid => "afb618c74510baa41a7d3928c0e57641";
        protected static AbilityTimelineEditorWindow EditorInst => AbilityTimelineEditorWindow.Instance;
        public abstract Type TrackDataType { get; }
        protected abstract Color TrackColor { get; }
        protected abstract Color MenuColor { get; }

        public virtual bool IsFixedTrack()
        {
            return false;
        }

        public abstract void TickView(int frameIndex, params object[] param);
        // public abstract VisualElement Inspector();

        public virtual UnityEngine.Object DataInspector => null;


        public virtual void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            _trackData = trackData;
            TrackParent = trackParent;
            MenuParent = menuParent;
            var trackAssetPath = AssetDatabase.GUIDToAssetPath(TrackAssetGuid);
            var menuAssetPath = AssetDatabase.GUIDToAssetPath(MenuAssetGuid);
            TrackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query()
                .ToList()[1];
            MenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];
            Track = TrackRoot.Q<VisualElement>("Container");
            MenuText = MenuRoot.Q<Label>("TrackName");
            BoundingBox = MenuRoot.Q<VisualElement>("BoundingBox");
            Lock = MenuRoot.Q<VisualElement>("Lock");
            MenuBox = MenuRoot.Q<VisualElement>("Box");
            Lock.style.display = DisplayStyle.None;
            TrackParent.Add(TrackRoot);
            MenuParent.Add(MenuRoot);

            _frameWidth = frameWidth;

            MenuRoot.RegisterCallback<MouseDownEvent>(OnMenuMouseDown);
            MenuRoot.AddManipulator(new ContextualMenuManipulator(OnMenuContextMenu));

            TrackRoot.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            TrackRoot.RegisterCallback<PointerOutEvent>(OnPointerOut);
            TrackRoot.AddManipulator(new ContextualMenuManipulator(OnContextMenu));

            // Track.style.backgroundColor = new Color(0, 0, 0, 0); //TrackColor;
            // BoundingBox.style.backgroundColor = MenuColor;

            MenuBox.style.right = 0;
            MenuBox.style.left = new StyleLength(StyleKeyword.Auto);
        }

        private void OnMenuMouseDown(MouseDownEvent evt)
        {
            OnSelect();
        }

        private void OnPointerOut(PointerOutEvent evt)
        {
            foreach (var trackItemBase in _trackItems)
                if (trackItemBase is TrackClipBase clipViewPair)
                    clipViewPair.ClipVe.OnHover(false);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            var mousePos = evt.position;
            foreach (var trackItemBase in _trackItems)
                if (trackItemBase is TrackClipBase clipViewPair)
                {
                    clipViewPair.ClipVe.OnHover(false);
                    if (!clipViewPair.ClipVe.InClipRect(mousePos)) continue;
                    clipViewPair.ClipVe.OnHover(true);
                    evt.StopImmediatePropagation();
                    return;
                }
        }

        public virtual void RefreshShow(float newFrameWidth)
        {
            _frameWidth = newFrameWidth;
        }

        public virtual void RefreshShow()
        {
            RefreshShow(_frameWidth);
        }

        public void RemoveTrackItem(TrackItemBase item)
        {
            Track.Remove(item.Ve);
            _trackItems.Remove(item);
        }

        public static int GetTrackIndexByMouse(float mouseLocalPositionX)
        {
            var x = mouseLocalPositionX - EditorInst.TimerShaftView.TimerShaft.worldBound.x +
                    EditorInst.CurrentFramePos;
            return Mathf.RoundToInt(x) / EditorInst.Config.FrameUnitWidth;
        }

        #region Select

        public bool Selected { get; private set; }
        private static readonly Color MenuSelectedColor = new(0.5f, 0.5f, 0.5f, 1f);
        private static readonly Color MenuUnSelectedColor = new(0.5f, 0.5f, 0.5f, 0f);

        public void OnSelect()
        {
            Selected = true;
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
            MenuRoot.style.backgroundColor = MenuSelectedColor;
        }

        public void OnUnSelect()
        {
            Selected = false;
            MenuRoot.style.backgroundColor = MenuUnSelectedColor;
        }

        #endregion


        #region Operation of Track

        private void OnMenuContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Delete", OnRemoveTrack, DropdownMenuAction.AlwaysEnabled);
        }

        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Add Item", OnAddTrackItem, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete Track", OnRemoveTrack, DropdownMenuAction.AlwaysEnabled);
        }

        protected abstract void OnAddTrackItem(DropdownMenuAction action);

        protected abstract void OnRemoveTrack(DropdownMenuAction action);

        #endregion
    }
}
#endif