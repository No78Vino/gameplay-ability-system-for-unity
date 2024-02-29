using System;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class CustomClipEventTrack:TrackBase
    {
        private CustomClipEventTrackData _customClipEventTrackData;
        public override Type TrackDataType => typeof(CustomClipEventTrackData);
        protected override Color TrackColor => new Color(0.7f, 0.3f, 0.7f, 0.2f);
        protected override Color MenuColor => new Color(0.5f, 0.3f, 0.5f, 1);

        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        public CustomClipEventTrackData CustomClipTrackDataForSave
        {
            get
            {
                for (int i = 0; i < AbilityAsset.CustomClips.Count; i++)
                {
                    if(AbilityAsset.CustomClips[i] == _customClipEventTrackData)
                        return AbilityAsset.CustomClips[i];
                }
                return null;
            }
        }
        
        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth, TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _customClipEventTrackData = trackData as CustomClipEventTrackData;
            MenuText.text = _customClipEventTrackData.trackName;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackClipBase)item).ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in _customClipEventTrackData.clipEvents)
                {
                    var item = new CustomClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }
        
        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Clip数据
            var clipEvent = new CustomClipEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                durationFrame = 5
            };
            CustomClipTrackDataForSave.clipEvents.Add(clipEvent);
            
            // 刷新显示
            var item = new CustomClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);
            
            // 选中新Clip
            item.ClipVe.OnSelect();
            
            Debug.Log("[EX] Add a new Custom Clip Event");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.CustomClips.Remove(_customClipEventTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(Menu);
            Debug.Log("[EX] Remove Custom Clip Track");
        }


        #region Inspector
        
        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            // track Name
            var trackNameTextField =TrackInspectorUtil.CreateTextField("轨道名",_customClipEventTrackData.trackName,
                (evt =>
                {
                    // 修改数据
                    CustomClipTrackDataForSave.trackName = evt.newValue;
                    AbilityAsset.Save();
                    // 修改显示
                    MenuText.text = evt.newValue;
                }));
            inspector.Add(trackNameTextField);
            
            
            return inspector;
        }
        #endregion
    }
}