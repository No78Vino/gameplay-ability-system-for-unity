using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using XYooAsset;

namespace DemoForESC._Script
{
    public static class TimelineHelper
    {
        public static PlayableDirector CreateTimeline(string path)
        {
            var prefab = XYoo.LoadAssetSync<GameObject>(path);
            var go = Object.Instantiate(prefab);
            return go.GetComponent<PlayableDirector>();
        }
        
        public static TrackAsset GetTrackByName(this TimelineAsset timeline, string trackName)
        {
            return timeline.GetOutputTracks().FirstOrDefault(track => track.name == trackName);
        }
    }
}