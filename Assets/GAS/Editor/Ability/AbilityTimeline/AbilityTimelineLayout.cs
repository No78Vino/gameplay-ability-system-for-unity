using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Ability.AbilityTimeline
{
    public static class AbilityTimelineLayout
    {
        [MenuItem( "Test/Ability Timeline")]
        public static void OpenTimelineWindow()
        {
            var window = UnityEditor.Timeline.TimelineEditor.GetOrCreateWindow();
            EditorApplication.delayCall +=
                () =>
                {
                    var inspectorWindow = AbilityTimelineInspector.Open();
                    inspectorWindow.Show();
                    window.DockWindow(inspectorWindow, DockUtilities.DockPosition.Right);
                };
        }
    }
}