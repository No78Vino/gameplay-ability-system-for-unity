using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace GAS.Runtime.Ability
{
    public class GeneralSequentialAbilityAsset : AbilityAsset
    {
        public override Type AbilityType()
        {
            return typeof(GeneralSequentialAbility);
        }

        [BoxGroup]
        [LabelText("时间轴资源")]
        [InfoBox("时间轴资源不可为空！", InfoMessageType.Error, "IsTimelineAssetEmpty")]
        [LabelWidth(100)]
        [InlineButton("EditTimelineAsset", "编辑时间轴")]
        public TimelineAsset timelineAsset;

        bool IsTimelineAssetEmpty()
        {
            return timelineAsset == null;
        }

#if UNITY_EDITOR
        private void EditTimelineAsset()
        {
            if (timelineAsset == null)
            {
                // 创建 TimelineAsset
                TimelineAsset asset = CreateInstance<TimelineAsset>();

                // // 添加轨道
                // TrackAsset track = asset.CreateTrack<AnimationTrack>(null, "Animation Track");
                //
                // // 添加片段
                // AnimationClip clip = /* 获取你的 AnimationClip */;
                // asset.CreateClip(clip, track);

                // 保存 TimelineAsset
                string path = "Assets/TimelineExample.timeline";
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            //TimelineEditor.timelineAsset = 
            var window = UnityEditor.Timeline.TimelineEditor.GetOrCreateWindow();
            EditorApplication.delayCall +=
                () =>
                {
                    // var inspectorWindow = AbilityTimelineInspector.Open();
                    // inspectorWindow.Show();
                    // window.DockWindow(inspectorWindow, DockUtilities.DockPosition.Right);
                };
        }
#endif
    }
}