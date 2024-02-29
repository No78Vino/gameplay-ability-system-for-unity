using System;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class ReleaseGameplayEffectMark:TrackMark<ReleaseGameplayEffectTrack>
    {
        private ReleaseGameplayEffectMarkEvent MarkData => markData as ReleaseGameplayEffectMarkEvent;

        private ReleaseGameplayEffectMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = MarkData.gameplayEffectAssets.Count.ToString();
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            // 释放单位方法选取
            var methodType = TrackInspectorUtil.CreateEnumField<LockMethod>("释放方式", MarkData.method.method, OnMethodChanged);
            inspector.Add(methodType);

            if (MarkData.method.method== LockMethod.Circle2D || MarkData.method.method== LockMethod.Sphere3D ||
                (MarkData.method.method== LockMethod.Box2D || MarkData.method.method== LockMethod.Box3D))
            {
                var layerMask = TrackInspectorUtil.CreateLayerMaskField("Layer", MarkData.method.checkLayer, (mask) =>
                {
                    MarkDataForSave.method.checkLayer = mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(layerMask);
                
                var centerType = TrackInspectorUtil.CreateEnumField<CenterType>("OffsetType", MarkData.method.centerType, (mask) =>
                {
                    MarkDataForSave.method.centerType = (CenterType)mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(centerType);
                
                var center = TrackInspectorUtil.CreateVector3Field("Offset", MarkData.method.center, (mask) =>
                {
                    MarkDataForSave.method.center = mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(center);
            }
            
            if (MarkData.method.method is LockMethod.Circle2D or LockMethod.Sphere3D)
            {
                var radius = TrackInspectorUtil.CreateFloatField("Radius", MarkData.method.radius, (mask) =>
                {
                    MarkDataForSave.method.radius = mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(radius);
            }
            
            if (MarkData.method.method is LockMethod.Box2D or LockMethod.Box3D)
            {
                var size = TrackInspectorUtil.CreateVector3Field("Size", MarkData.method.size, (mask) =>
                {
                    MarkDataForSave.method.size = mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(size);
            }
            
            if (MarkData.method.method is LockMethod.Custom)
            {
                var customMethodRegisterKey = TrackInspectorUtil.CreateTextField("MethodKey", MarkData.method.customMethodRegisterKey, (mask) =>
                {
                    MarkDataForSave.method.customMethodRegisterKey = mask.newValue;
                    AbilityTimelineEditorWindow.Instance.Save();
                });
                inspector.Add(customMethodRegisterKey);
            }
            
            var list = TrackInspectorUtil.CreateObjectListView("GameplayEffect列表", MarkData.gameplayEffectAssets, OnGameplayEffectAssetChanged);
            inspector.Add(list);
            
            return inspector;
        }

        private void OnMethodChanged(ChangeEvent<Enum> evt)
        {
            MarkDataForSave.method.method = (LockMethod) evt.newValue;
            AbilityTimelineEditorWindow.Instance.Save();
            
            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }

        private void OnGameplayEffectAssetChanged(int index, ChangeEvent<Object> evt)
        {
            var gameplayEffectAsset = evt.newValue as GameplayEffectAsset;
            MarkDataForSave.gameplayEffectAssets[index] = gameplayEffectAsset;
            AbilityTimelineEditorWindow.Instance.Save();
            
            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData.markEvents.Remove(MarkData);
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
    }
}