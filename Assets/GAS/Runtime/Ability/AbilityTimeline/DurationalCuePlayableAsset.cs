using UnityEngine;
using UnityEngine.Playables;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [System.Serializable]
    public class DurationalCuePlayableAsset : PlayableAsset
    {
        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return Playable.Create(graph);
        }
    }
}
