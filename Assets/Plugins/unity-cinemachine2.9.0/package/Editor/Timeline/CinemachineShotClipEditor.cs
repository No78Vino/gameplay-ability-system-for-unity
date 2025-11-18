#if !UNITY_2019_1_OR_NEWER
#define CINEMACHINE_TIMELINE
#endif

#if CINEMACHINE_TIMELINE && UNITY_2019_2_OR_NEWER

using UnityEngine.Timeline;
using UnityEditor.Timeline;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomTimelineEditor(typeof(CinemachineShot))]
public class CinemachineShotClipEditor : ClipEditor
{
    [InitializeOnLoad]
    class EditorInitialize 
    { 
        static EditorInitialize() { CinemachineMixer.GetMasterPlayableDirector = GetMasterDirector; } 
        static PlayableDirector GetMasterDirector() { return TimelineEditor.masterDirector; }
    }
    public delegate double TimelineGlobalToLocalTimeDelegate(double globalTime);
    public static TimelineGlobalToLocalTimeDelegate TimelineGlobalToLocalTime = DefaultTimeConversion;
#if CINEMACHINE_TIMELINE_1_5_0
    static double DefaultTimeConversion(double time) { return TimelineEditor.GetInspectedTimeFromMasterTime(time); }
#else
    static double DefaultTimeConversion(double time) { return time; }
#endif

    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        var shotClip = (CinemachineShot) clip.asset;
        var clipOptions = base.GetClipOptions(clip);
        if (shotClip != null)
        {
            var director = TimelineEditor.inspectedDirector;
            if (director != null)
            {
                var vcam = shotClip.VirtualCamera.Resolve(director);
                if (vcam == null)
                    clipOptions.errorText = "A virtual camera must be assigned";
                else
                    clipOptions.tooltip = vcam.Name;

            }
        }
        return clipOptions;
    }

    public override void OnClipChanged(TimelineClip clip)
    {
        var shotClip = (CinemachineShot)clip.asset;
        if (shotClip == null)
            return;
        if (shotClip.DisplayName != null && shotClip.DisplayName.Length != 0)
            clip.displayName = shotClip.DisplayName;
        else
        {
            var director = TimelineEditor.inspectedDirector;
            if (director != null)
            {
                var vcam = shotClip.VirtualCamera.Resolve(director);
                if (vcam != null)
                    clip.displayName = vcam.Name;
            }
        }
    }

    public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
    {
        base.OnCreate(clip, track, clonedFrom);
        if (CinemachineShotEditor.AutoCreateShotFromSceneView)
        {
            var asset = clip.asset as CinemachineShot;
            var vcam = CinemachineShotEditor.CreatePassiveVcamFromSceneView();
            var d = TimelineEditor.inspectedDirector;
            if (d != null && d.GetReferenceValue(asset.VirtualCamera.exposedName, out bool idValid) == null)
            {
                asset.VirtualCamera.exposedName = System.Guid.NewGuid().ToString();
                d.SetReferenceValue(asset.VirtualCamera.exposedName, vcam);
            }
        }
    }

    GUIContent kUndamped = new GUIContent("UNCACHED");

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        base.DrawBackground(clip, region);

        if (Application.isPlaying || !TargetPositionCache.UseCache 
            || TargetPositionCache.CacheMode == TargetPositionCache.Mode.Disabled
            || TimelineEditor.inspectedDirector == null)
        {
            return;
        }

        // Draw the cache indicator over the cached region
        var cacheRange = TargetPositionCache.CacheTimeRange;
        if (!cacheRange.IsEmpty)
        {
            cacheRange.Start = (float)TimelineGlobalToLocalTime(cacheRange.Start);
            cacheRange.End = (float)TimelineGlobalToLocalTime(cacheRange.End);

            // Clip cacheRange to rect
            float start = (float)region.startTime;
            float end = (float)region.endTime;
            cacheRange.Start = Mathf.Max((float)clip.ToLocalTime(cacheRange.Start), start);
            cacheRange.End = Mathf.Min((float)clip.ToLocalTime(cacheRange.End), end);
                
            var r = region.position;
            var a = r.x + r.width * (cacheRange.Start - start) / (end - start);
            var b = r.x + r.width * (cacheRange.End - start) / (end - start);
            r.x = a; r.width = b-a;
            r.y += r.height; r.height *= 0.2f; r.y -= r.height;
            EditorGUI.DrawRect(r, new Color(0.1f, 0.2f, 0.8f, 0.6f));
        }

        // Draw the "UNCACHED" indicator, if appropriate
        if (!TargetPositionCache.IsRecording && !TargetPositionCache.CurrentPlaybackTimeValid)
        {
            var r = region.position;
            var t = clip.ToLocalTime(TimelineGlobalToLocalTime(TimelineEditor.masterDirector.time));
            var pos = r.x + r.width 
                * (float)((t - region.startTime) / (region.endTime - region.startTime));
    
            var s = EditorStyles.miniLabel.CalcSize(kUndamped);
            r.width = s.x; r.x = pos - r.width / 2;
            var c = GUI.color;
            GUI.color = Color.yellow;
            EditorGUI.LabelField(r, kUndamped, EditorStyles.miniLabel);
            GUI.color = c;
        }
    }
}


#endif
