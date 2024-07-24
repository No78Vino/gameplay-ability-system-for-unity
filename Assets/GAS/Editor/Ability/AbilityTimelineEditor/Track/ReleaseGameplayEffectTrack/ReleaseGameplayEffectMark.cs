using GAS.Runtime;
using UnityEngine;

namespace GAS.Editor
{
    public class ReleaseGameplayEffectMark : TrackMark<ReleaseGameplayEffectTrack>
    {
        private new ReleaseGameplayEffectMarkEvent MarkData => markData as ReleaseGameplayEffectMarkEvent;

        public ReleaseGameplayEffectMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = track.ReleaseGameplayEffectTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return track.ReleaseGameplayEffectTrackData.markEvents[i];
                return null;
            }
        }

        public override void Duplicate()
        {
            var startFrame = markData.startFrame < AbilityAsset.FrameCount
                ? markData.startFrame + 1
                : markData.startFrame - 1;
            startFrame = Mathf.Clamp(startFrame, 0, AbilityAsset.FrameCount);
            var markEvent = new ReleaseGameplayEffectMarkEvent
            {
                startFrame = startFrame,
                jsonTargetCatcher = (markData as ReleaseGameplayEffectMarkEvent)?.jsonTargetCatcher,
                gameplayEffectAssets = (markData as ReleaseGameplayEffectMarkEvent)?.gameplayEffectAssets
            };
            track.ReleaseGameplayEffectTrackData.markEvents.Add(markEvent);
            AbilityTimelineEditorWindow.Instance.Save();

            var mark = new ReleaseGameplayEffectMark();
            mark.InitTrackMark(track, track.Track, FrameUnitWidth, markEvent);
            track.TrackItems.Add(mark);
            mark.OnSelect();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = MarkData.gameplayEffectAssets.Count.ToString();
        }

        public override Object DataInspector => ReleaseGameplayEffectMarkEditor.Create(this);
        // public override VisualElement Inspector()
        // {
        //     var inspector = TrackInspectorUtil.CreateTrackInspector();
        //     var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{markData.startFrame}");
        //     inspector.Add(markFrame);
        //
        //     // 目标捕捉器
        //     // 选择项：所有TargetCatcher子类
        //     var targetCatcherSonTypes = ReleaseGameplayEffectMarkEvent.TargetCatcherSonTypes;
        //     var targetCatcherSons = targetCatcherSonTypes.Select(sonType => sonType.FullName).ToList();
        //     var catcherTypeSelector =
        //         TrackInspectorUtil.CreateDropdownField("TargetCatcher", targetCatcherSons,
        //             MarkData.jsonTargetCatcher.Type, OnTargetCatcherChanged);
        //     inspector.Add(catcherTypeSelector);
        //
        //     // 根据选择的TargetCatcher子类，显示对应的属性
        //     var targetCatcher = MarkData.LoadTargetCatcher();
        //     if (TargetCatcherInspectorMap.TryGetValue(targetCatcher.GetType(), out var inspectorType))
        //     {
        //         var targetCatcherInspector =
        //             (TargetCatcherInspector)Activator.CreateInstance(inspectorType, targetCatcher);
        //         inspector.Add(targetCatcherInspector.Inspector());
        //     }
        //     else
        //     {
        //         Debug.LogError($"[EX] TargetCatcherInspector not found: {targetCatcher.GetType()}");
        //     }
        //
        //     // GameplayEffects
        //     var list = TrackInspectorUtil.CreateObjectListView("GameplayEffects", MarkData.gameplayEffectAssets,
        //         OnGameplayEffectAssetChanged);
        //     inspector.Add(list);
        //
        //     return inspector;
        // }

        // private void OnTargetCatcherChanged(ChangeEvent<string> evt)
        // {
        //     MarkDataForSave.jsonTargetCatcher.Type = evt.newValue;
        //     MarkDataForSave.jsonTargetCatcher.Data = null;
        //     AbilityTimelineEditorWindow.Instance.Save();
        //
        //     AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        // }
        //
        // private void OnGameplayEffectAssetChanged(int index, ChangeEvent<Object> evt)
        // {
        //     var gameplayEffectAsset = evt.newValue as GameplayEffectAsset;
        //     MarkDataForSave.gameplayEffectAssets[index] = gameplayEffectAsset;
        //     AbilityTimelineEditorWindow.Instance.Save();
        //
        //     RefreshShow(FrameUnitWidth);
        // }

        public override void Delete()
        {
            var success = track.ReleaseGameplayEffectTrackData.markEvents.Remove(MarkData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void UpdateMarkDataFrame(int newStartFrame)
        {
            var updatedClip = MarkDataForSave;
            MarkDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            markData = updatedClip;
        }

        public override void OnTickView(int frameIndex)
        {
            if (frameIndex == StartFrameIndex)
            {
                var targetCatcher = MarkData.TargetCatcher;
                targetCatcher.OnEditorPreview(AbilityTimelineEditorWindow.Instance.PreviewObject);
            }
        }
    }
}