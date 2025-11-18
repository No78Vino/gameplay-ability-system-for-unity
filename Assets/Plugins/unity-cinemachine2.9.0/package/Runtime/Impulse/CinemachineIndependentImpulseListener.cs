using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// This behaviour can be attached to any object to make it shake in response to Impulses.
    /// 
    /// This behaviour can be attached to the main Camera with the CinemachineBrain,
    /// to allow the main camera to shake without putting Listeners on the virtual cameras. 
    /// In this case, camera shake is not dependent on the active virtual camera.
    /// 
    /// It is also possible to put this behaviour on other scene objects to shake them
    /// in response to impulses.
    /// </summary>
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "api/Cinemachine.CinemachineIndependentImpulseListener.html")]
    public class CinemachineIndependentImpulseListener : MonoBehaviour
    {
        private Vector3 impulsePosLastFrame;
        private Quaternion impulseRotLastFrame;

        /// <summary>
        /// Impulse events on channels not included in the mask will be ignored.
        /// </summary>
        [Tooltip("Impulse events on channels not included in the mask will be ignored.")]
        [CinemachineImpulseChannelProperty]
        public int m_ChannelMask;

        /// <summary>
        /// Gain to apply to the Impulse signal.
        /// </summary>
        [Tooltip("Gain to apply to the Impulse signal.  1 is normal strength.  Setting this to 0 completely mutes the signal.")]
        public float m_Gain;

        /// <summary>
        /// Enable this to perform distance calculation in 2D (ignore Z).
        /// </summary>
        [Tooltip("Enable this to perform distance calculation in 2D (ignore Z)")]
        public bool m_Use2DDistance;

        /// <summary>
        /// Enable this to process all impulse signals in camera space.
        /// </summary>
        [Tooltip("Enable this to process all impulse signals in camera space")]
        public bool m_UseLocalSpace;

        /// <summary>
        /// This controls the secondary reaction of the listener to the incoming impulse.  
        /// The impulse might be for example a sharp shock, and the secondary reaction could
        /// be a vibration whose amplitude and duration is controlled by the size of the 
        /// original impulse.  This allows different listeners to respond in different ways 
        /// to the same impulse signal.
        /// </summary>
        [Tooltip("This controls the secondary reaction of the listener to the incoming impulse.  "
            + "The impulse might be for example a sharp shock, and the secondary reaction could "
            + "be a vibration whose amplitude and duration is controlled by the size of the "
            + "original impulse.  This allows different listeners to respond in different ways "
            + "to the same impulse signal.")]
        public CinemachineImpulseListener.ImpulseReaction m_ReactionSettings;

        private void Reset()
        {
            m_ChannelMask = 1;
            m_Gain = 1;
            m_Use2DDistance = false;
            m_UseLocalSpace = true;
            m_ReactionSettings = new CinemachineImpulseListener.ImpulseReaction 
            { 
                m_AmplitudeGain = 1, 
                m_FrequencyGain = 1,
                m_Duration = 1f
            };
        }

        private void OnEnable()
        {
            impulsePosLastFrame = Vector3.zero;
            impulseRotLastFrame = Quaternion.identity;
        }

        private void Update()
        {
            // Unapply previous shake
            transform.position -= impulsePosLastFrame;
            transform.rotation = transform.rotation * Quaternion.Inverse(impulseRotLastFrame);
        }

        // We do this in LateUpdate specifically to support attaching this script to the
        // Camera with the CinemachineBrain.  Script execution order is after the brain.
        private void LateUpdate()
        {
            // Apply the shake
            bool haveImpulse = CinemachineImpulseManager.Instance.GetImpulseAt(
                transform.position, m_Use2DDistance, m_ChannelMask, 
                out impulsePosLastFrame, out impulseRotLastFrame);
            bool haveReaction = m_ReactionSettings.GetReaction(
                Time.deltaTime, impulsePosLastFrame, out var reactionPos, out var reactionRot);

            if (haveImpulse)
            {
                impulseRotLastFrame = Quaternion.SlerpUnclamped(
                    Quaternion.identity, impulseRotLastFrame, m_Gain);
                impulsePosLastFrame *= m_Gain;
            }
            if (haveReaction)
            {
                impulsePosLastFrame += reactionPos;
                impulseRotLastFrame *= reactionRot;
            }
            if (haveImpulse || haveReaction)
            {
                if (m_UseLocalSpace)
                    impulsePosLastFrame = transform.rotation * impulsePosLastFrame;

                transform.position += impulsePosLastFrame;
                transform.rotation = transform.rotation * impulseRotLastFrame;
            }
        }
    }
}
