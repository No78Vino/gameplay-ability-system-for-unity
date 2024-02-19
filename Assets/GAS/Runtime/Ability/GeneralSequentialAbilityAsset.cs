using System;
using GAS.Core;
using GAS.General;
using GAS.General.AbilityTimeline;
using GAS.Runtime.Effects.Modifier;
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
        [AssetSelector]
        public TimelineAsset timelineAsset;

        bool IsTimelineAssetEmpty()
        {
            return timelineAsset == null;
        }

#if UNITY_EDITOR
        void OnCreateTimelineAsset(TimelineAsset asset)
        {
            timelineAsset = asset;
            OpenTimelineWindow();
        }
        
        void CreateTimelineAsset()
        {
            string path = AssetDatabase.GetAssetPath(this);
            path = path.Substring(0, path.IndexOf(GasDefine.GAS_ABILITY_LIBRARY_FOLDER, StringComparison.Ordinal));
            path +=  GasDefine.GAS_ABILITY_TIMELINE_LIBRARY_FOLDER;
            ScriptableObjectCreator.ShowDialog<TimelineAsset>(path, OnCreateTimelineAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void OpenTimelineWindow()
        {
            var window = TimelineEditor.GetOrCreateWindow();
            EditorApplication.delayCall +=
                () =>
                {
                    var inspectorWindow = AbilityTimelineInspector.Open(timelineAsset);
                    inspectorWindow.Show();
                    window.DockWindow(inspectorWindow, DockUtilities.DockPosition.Left);
                    window.SetTimeline(timelineAsset);
                };
        }
        
        private void EditTimelineAsset()
        {
            if (timelineAsset == null)
            {
                CreateTimelineAsset();
            }
            else
            {
                OpenTimelineWindow();
            }
        }
#endif
    }
}