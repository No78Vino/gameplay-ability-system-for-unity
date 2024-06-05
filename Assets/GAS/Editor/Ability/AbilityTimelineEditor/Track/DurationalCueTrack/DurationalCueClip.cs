
#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class DurationalCueClip : TrackClip<DurationalCueTrack>
    {
        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        public DurationalCueClipEvent DurationalCueClipData => clipData as DurationalCueClipEvent;

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
            ItemLabel.text = DurationalCueClipData.cue ? DurationalCueClipData.cue.name : "NULL!";

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
        
        public void UpdateClipDataCue(GameplayCueDurational newCue)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.cue = newCue;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
            RefreshShow(FrameUnitWidth);
        }

        public override void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
            DurationalCueClipData.cue.OnEditorPreview(AbilityTimelineEditorWindow.Instance.PreviewObject, frameIndex,
                startFrame, endFrame);
        }

        #region Clip Visual Element Event

        // private int MinStartFrameIndex(float lastMainDragStartPos)
        // {
        //     var minFrame = 0;
        //     foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
        //         if (clipEvent != ClipData && clipEvent.EndFrame <= lastMainDragStartPos)
        //             minFrame = Mathf.Max(minFrame, clipEvent.EndFrame);
        //
        //     return minFrame;
        // }
        //
        // private int MaxEndFrameIndex(float lastMainDragStartPos)
        // {
        //     var maxFrame = AbilityTimelineEditorWindow.Instance.AbilityAsset.FrameCount;
        //     foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
        //         if (clipEvent != ClipData && clipEvent.startFrame >= lastMainDragStartPos + DurationFrame)
        //             maxFrame = Mathf.Min(maxFrame, clipEvent.startFrame);
        //
        //     return maxFrame;
        // }

        #endregion

        public override Object DataInspector => DurationalCueClipEditor.Create(this);
    }
}
#endif