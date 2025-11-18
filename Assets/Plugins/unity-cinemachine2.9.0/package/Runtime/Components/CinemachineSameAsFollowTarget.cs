using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to match the orientation of the Follow target.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    public class CinemachineSameAsFollowTarget : CinemachineComponentBase
    {
        /// <summary>
        /// How much time it takes for the aim to catch up to the target's rotation
        /// </summary>
        [Tooltip("How much time it takes for the aim to catch up to the target's rotation")]
        [FormerlySerializedAs("m_AngularDamping")]
        public float m_Damping = 0;

        Quaternion m_PreviousReferenceOrientation = Quaternion.identity;

        /// <summary>True if component is enabled and has a Follow target defined</summary>
        public override bool IsValid { get { return enabled && FollowTarget != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Aim stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Aim; } }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() { return m_Damping; }

        /// <summary>Orients the camera to match the Follow target's orientation</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Not used.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
                return;

            Quaternion dampedOrientation = FollowTargetRotation;
            if (deltaTime >= 0)
            {
                float t = VirtualCamera.DetachedFollowTargetDamp(1, m_Damping, deltaTime);
                dampedOrientation = Quaternion.Slerp(
                    m_PreviousReferenceOrientation, FollowTargetRotation, t);
            }
            m_PreviousReferenceOrientation = dampedOrientation;
            curState.RawOrientation = dampedOrientation;
        }
    }
}
