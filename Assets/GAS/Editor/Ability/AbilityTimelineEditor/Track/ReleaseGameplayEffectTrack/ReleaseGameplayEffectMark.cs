using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.General;
using GAS.Runtime.Ability.TargetCatcher;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class ReleaseGameplayEffectMark:TrackMark<ReleaseGameplayEffectTrack>
    {
        private static Type[] _targetCatcherInspectorTypes;

        public static Type[] TargetCatcherInspectorTypes =>
            _targetCatcherInspectorTypes ??= TypeUtil.GetAllSonTypesOf(typeof(TargetCatcherInspector));

        
        private static Dictionary<Type, Type> _targetCatcherInspectorMap;
        private static Dictionary<Type, Type> TargetCatcherInspectorMap
        {
            get
            {
                if (_targetCatcherInspectorMap != null) return _targetCatcherInspectorMap;
                _targetCatcherInspectorMap = new Dictionary<Type, Type>();
                foreach (var catcherInspectorType in TargetCatcherInspectorTypes)
                {
                    var targetCatcherType = catcherInspectorType.BaseType.GetGenericArguments()[0];
                    _targetCatcherInspectorMap.Add(targetCatcherType, catcherInspectorType);
                }

                return _targetCatcherInspectorMap;
            }
        }
        
        private ReleaseGameplayEffectMarkEvent MarkData => markData as ReleaseGameplayEffectMarkEvent;

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

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = MarkData.gameplayEffectAssets.Count.ToString();
        }
        
        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{markData.startFrame}");
            inspector.Add(markFrame);

            // 目标捕捉器
            // 选择项：所有TargetCatcher子类
            var targetCatcherSonTypes= ReleaseGameplayEffectMarkEvent.TargetCatcherSonTypes;
            List<string> targetCatcherSons  = targetCatcherSonTypes.Select(sonType => sonType.FullName).ToList();
            var catcherTypeSelector =
                TrackInspectorUtil.CreateDropdownField("TargetCatcher", targetCatcherSons,
                    MarkData.jsonTargetCatcher.Type, OnTargetCatcherChanged);
            inspector.Add(catcherTypeSelector);
            
            // 根据选择的TargetCatcher子类，显示对应的属性
            var targetCatcher = MarkData.LoadTargetCatcher();
            if(TargetCatcherInspectorMap.TryGetValue(targetCatcher.GetType(), out var inspectorType))
            {
                var targetCatcherInspector = (TargetCatcherInspector)Activator.CreateInstance(inspectorType, targetCatcher);
                inspector.Add(targetCatcherInspector.Inspector());
            }
            else
            {
                Debug.LogError( $"[EX] TargetCatcherInspector not found: {targetCatcher.GetType()}");
            }
            
            // GameplayEffects
            var list = TrackInspectorUtil.CreateObjectListView("GameplayEffects", MarkData.gameplayEffectAssets, OnGameplayEffectAssetChanged);
            inspector.Add(list);
            
            return inspector;
        }

        private void OnTargetCatcherChanged(ChangeEvent<string> evt)
        {
            MarkDataForSave.jsonTargetCatcher.Type = evt.newValue;
            MarkDataForSave.jsonTargetCatcher.Data = null;
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
                var targetCatcher = MarkData.LoadTargetCatcher();
                if (TargetCatcherInspectorMap.TryGetValue(targetCatcher.GetType(), out var inspectorType))
                {
                    TargetCatcherInspector inspector= (TargetCatcherInspector)Activator.CreateInstance(inspectorType, targetCatcher);
                    inspector.OnTargetCatcherPreview(AbilityTimelineEditorWindow.Instance.PreviewObject);
                }
            }
        }
    }
}