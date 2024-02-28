using System;
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackBase
    {
        protected TrackDataBase _trackData;
        protected List<TrackItemBase> _trackItems = new List<TrackItemBase>();

        protected float _frameWidth;
        protected VisualElement Menu;
        protected VisualElement MenuParent;
        protected VisualElement Track;
        protected VisualElement TrackParent;
        protected Label MenuText;
        protected VisualElement BoundingBox;
        
        private static string TrackAssetGuid => "67e1b3c42dcc09a4dbb9e9b107500dfd";
        private static string MenuAssetGuid => "afb618c74510baa41a7d3928c0e57641";
        public abstract Type TrackDataType { get; }
        protected abstract Color TrackColor { get; }
        protected abstract Color MenuColor { get; }
        public virtual bool IsFixedTrack() => false;
        public abstract void TickView(int frameIndex, params object[] param);
        public abstract VisualElement Inspector();
        
        public virtual void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,TrackDataBase trackData)
        {
            _trackData = trackData;
            TrackParent = trackParent;
            MenuParent = menuParent;
            var trackAssetPath = AssetDatabase.GUIDToAssetPath(TrackAssetGuid);
            var menuAssetPath = AssetDatabase.GUIDToAssetPath(MenuAssetGuid);
            Track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];
            Menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];
            MenuText = Menu.Q<Label>("TrackName");
            BoundingBox = Menu.Q<VisualElement>("BoundingBox");
            TrackParent.Add(Track);
            MenuParent.Add(Menu);

            _frameWidth = frameWidth;

            Menu.RegisterCallback<MouseDownEvent>(OnMenuMouseDown);
                
            Track.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            Track.RegisterCallback<PointerOutEvent>(OnPointerOut);
            Track.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            
            Track.style.backgroundColor = TrackColor;
            BoundingBox.style.backgroundColor = MenuColor;
        }

        private void OnMenuMouseDown(MouseDownEvent evt)
        {
            OnSelect();
        }

        private void OnPointerOut(PointerOutEvent evt)
        {
            foreach (var trackItemBase in _trackItems)
            {
                if (trackItemBase is TrackClipBase clipViewPair)
                    clipViewPair.ClipVe.OnHover(false);
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            var mousePos = evt.position;
            foreach (var trackItemBase in _trackItems)
            {
                if (trackItemBase is TrackClipBase clipViewPair)
                {
                    clipViewPair.ClipVe.OnHover(false);
                    if (!clipViewPair.ClipVe.InClipRect(mousePos)) continue;
                    clipViewPair.ClipVe.OnHover(true);
                    evt.StopImmediatePropagation();
                    return;
                }
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
        
        #region Select
        public bool Selected { get; private set; }
        private static Color MenuSelectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private static Color MenuUnSelectedColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        public void OnSelect()
        {
            Selected = true;
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
            Menu.style.backgroundColor = MenuSelectedColor;
        }

        public void OnUnSelect()
        {
            Selected = false;
            Menu.style.backgroundColor = MenuUnSelectedColor;
        }

        #endregion

        
        #region Operation of Track

        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Add Clip", OnAddTrackItem, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Remove Track", OnRemoveTrack, DropdownMenuAction.AlwaysEnabled);
        }

        protected abstract void OnAddTrackItem(DropdownMenuAction action);

        protected abstract void OnRemoveTrack(DropdownMenuAction action);

        #endregion
    }
}
#endif