using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class DurationalCueClip : TrackClip<DurationalCueTrack>
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private DurationalCueClipEvent DurationalCueClipData => clipData as DurationalCueClipEvent;

        private DurationalCueClipEvent ClipDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.CueTrackDataForSave;
                for (var i = 0; i < cueTrackDataForSave.clipEvents.Count; i++)
                    if (cueTrackDataForSave.clipEvents[i] == DurationalCueClipData)
                        return track.CueTrackDataForSave.clipEvents[i];
                return null;
            }
        }

        public override void InitTrackClip(TrackBase track, VisualElement parent, float frameUnitWidth,
            ClipEventBase clipData)
        {
            base.InitTrackClip(track, parent, frameUnitWidth, clipData);

            //ve.RegisterFuncGetMinStartFrameIndex(MinStartFrameIndex);
            //ve.RegisterFuncGetMaxEndFrameIndex(MaxEndFrameIndex);
        }

        public override void Delete()
        {
            var success = track.CueTrackDataForSave.clipEvents.Remove(DurationalCueClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            // clip 文本
            ItemLabel.text = DurationalCueClipData.cue ? DurationalCueClipData.cue.name : "【NULL】";

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.durationFrame = newDurationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        #region Clip Visual Element Event

        private int MinStartFrameIndex(float lastMainDragStartPos)
        {
            var minFrame = 0;
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
                if (clipEvent != ClipData && clipEvent.EndFrame <= lastMainDragStartPos)
                    minFrame = Mathf.Max(minFrame, clipEvent.EndFrame);

            return minFrame;
        }

        private int MaxEndFrameIndex(float lastMainDragStartPos)
        {
            var maxFrame = AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount;
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
                if (clipEvent != ClipData && clipEvent.startFrame >= lastMainDragStartPos + DurationFrame)
                    maxFrame = Mathf.Min(maxFrame, clipEvent.startFrame);

            return maxFrame;
        }

        #endregion


        #region Inspector

        private Label _startFrameLabel;
        private Label _endFrameLabel;
        private IntegerField _durationField;

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            // cue Asset
            var cue = TrackInspectorUtil.CreateObjectField("Cue资源", typeof(GameplayCueDurational),
                DurationalCueClipData.cue,
                evt =>
                {
                    // 修改数据
                    ClipDataForSave.cue = evt.newValue as GameplayCueDurational;
                    AbilityAsset.Save();
                    clipData = ClipDataForSave;
                    // 修改显示
                    RefreshShow(FrameUnitWidth);
                });
            inspector.Add(cue);

            // 开始帧
            _startFrameLabel = TrackInspectorUtil.CreateLabel($"开始帧:{DurationalCueClipData.startFrame}");
            inspector.Add(_startFrameLabel);

            // 结束帧
            _endFrameLabel = TrackInspectorUtil.CreateLabel($"结束帧:{DurationalCueClipData.EndFrame}");
            inspector.Add(_endFrameLabel);

            // 持续帧
            _durationField = TrackInspectorUtil.CreateIntegerField("持续帧数(f)", DurationalCueClipData.durationFrame,
                OnDurationFrameChanged);
            inspector.Add(_durationField);

            // 删除按钮
            var deleteButton = TrackInspectorUtil.CreateButton("删除", Delete);
            deleteButton.style.backgroundColor = new StyleColor(new Color(0.5f, 0, 0, 1f));
            inspector.Add(deleteButton);

            return inspector;
        }

        private void OnDurationFrameChanged(ChangeEvent<int> evt)
        {
            // 钳制
            var max = AbilityAsset.MaxFrameCount - DurationalCueClipData.startFrame;
            var newValue = Mathf.Clamp(evt.newValue, 1, max);
            // 保存数据
            UpdateClipDataDurationFrame(newValue);
            // 修改显示
            RefreshShow(FrameUnitWidth);
            _endFrameLabel.text = $"结束帧:{DurationalCueClipData.EndFrame}";
            _durationField.value = newValue;
        }

        #endregion
    }
}