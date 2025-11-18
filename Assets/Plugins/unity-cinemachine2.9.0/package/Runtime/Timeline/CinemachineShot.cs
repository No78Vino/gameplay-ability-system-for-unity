#if !UNITY_2019_1_OR_NEWER
#define CINEMACHINE_TIMELINE
#endif
#if CINEMACHINE_TIMELINE

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

//namespace Cinemachine.Timeline
//{
    /// <summary>
    /// Internal use only.  Not part of the public API.
    /// </summary>
    public sealed class CinemachineShot : PlayableAsset, IPropertyPreview
    {
        /// <summary>The name to display on the track</summary>
#if !UNITY_2019_2_OR_NEWER
        [HideInInspector]
#endif
        public string DisplayName;

        /// <summary>The virtual camera to activate</summary>
        public ExposedReference<CinemachineVirtualCameraBase> VirtualCamera;

        /// <summary>PlayableAsset implementation</summary>
        /// <param name="graph"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CinemachineShotPlayable>.Create(graph);
            playable.GetBehaviour().VirtualCamera = VirtualCamera.Resolve(graph.GetResolver());
            return playable;
        }

        /// <summary>IPropertyPreview implementation</summary>
        /// <param name="director"></param>
        /// <param name="driver"></param>
        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            driver.AddFromName<Transform>("m_LocalPosition.x");
            driver.AddFromName<Transform>("m_LocalPosition.y");
            driver.AddFromName<Transform>("m_LocalPosition.z");
            driver.AddFromName<Transform>("m_LocalRotation.x");
            driver.AddFromName<Transform>("m_LocalRotation.y");
            driver.AddFromName<Transform>("m_LocalRotation.z");
            driver.AddFromName<Transform>("m_LocalRotation.w");

            driver.AddFromName<Camera>("field of view");
            driver.AddFromName<Camera>("near clip plane");
            driver.AddFromName<Camera>("far clip plane");
        }
    }
//}
#endif
