using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackBase
    {
        protected List<AnimationTrackClip> _trackItems = new List<AnimationTrackClip>();
        
        protected float FrameWidth;
        protected VisualElement MenuParent;
        protected VisualElement TrackParent;
        protected VisualElement Track;
        protected VisualElement Menu;
        
        protected abstract string TrackAssetPath { get; }
        protected abstract string MenuAssetPath { get; }
        public abstract void TickView(int frameIndex, params object[] param);
        public abstract bool CheckFrameIndexOnDrag(int targetIndex);
        public abstract void SetFrameIndex(int oldIndex, int newIndex);

        public virtual void Init( VisualElement trackParent, VisualElement menuParent, float frameWidth)
        {
            TrackParent = trackParent;
            MenuParent = menuParent;
            Track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];
            Menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];
            TrackParent.Add(Track);
            MenuParent.Add(Menu);
            
            FrameWidth = frameWidth;
            
            
            Track.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            Track.RegisterCallback<PointerOutEvent>(OnPointerOut);
            
            Track.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
        }

        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            // 添加右键菜单项
            string[] menuName = {"Add Clip"};
            evt.menu.AppendAction("Add Clip", OnMenuItemClicked, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete Clip", OnMenuItemClicked, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Remove Track", OnMenuItemClicked, DropdownMenuAction.AlwaysEnabled);
        }

        // 右键菜单项点击回调函数
        private void OnMenuItemClicked(DropdownMenuAction action)
        {
            Debug.Log($"Menu Item Clicked: {action.name}");
        }
        
        private void OnPointerOut(PointerOutEvent evt)
        {
            foreach (var clipViewPair in _trackItems)
            {
                clipViewPair.Ve.OnHover(false);
            }
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            var mousePos = evt.position;
            foreach (var clipViewPair in _trackItems)
            {
                clipViewPair.Ve.OnHover(false);
                if (clipViewPair.Ve.InClipRect(mousePos))
                {
                    clipViewPair.Ve.OnHover(true);
                    evt.StopImmediatePropagation();
                    return;
                }
            }
        }

        public virtual void RefreshShow(float newFrameWidth)
        {
            FrameWidth = newFrameWidth;
        }

        public virtual void RefreshShow()
        {
            RefreshShow(FrameWidth);
        }

        #region Select

        public void Select(){}
        public void OnSelect(){}
        public void OnUnSelect(){}

        #endregion
    }
}
#endif