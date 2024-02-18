using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.Timeline;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [TrackColor(1,1,0)]
    [TrackClipType(typeof(DurationalCuePlayableAsset))]
    [TrackBindingType(typeof(GameplayCueDurational))]
    public class DurationalCueTrack : TrackAsset
    {
        
    }
}