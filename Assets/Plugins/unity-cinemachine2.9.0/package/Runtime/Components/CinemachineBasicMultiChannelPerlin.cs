using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{
    /// <summary>
    /// As a part of the Cinemachine Pipeline implementing the Noise stage, this
    /// component adds Perlin Noise to the Camera state, in the Correction
    /// channel of the CameraState.
    ///
    /// The noise is created by using a predefined noise profile asset.  This defines the
    /// shape of the noise over time.  You can scale this in amplitude or in time, to produce
    /// a large family of different noises using the same profile.
    /// </summary>
    /// <seealso cref="NoiseSettings"/>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    public class CinemachineBasicMultiChannelPerlin : CinemachineComponentBase
    {
        /// <summary>
        /// Serialized property for referencing a NoiseSettings asset
        /// </summary>
        [Tooltip("The asset containing the Noise Profile.  Define the frequencies and amplitudes "
            + "there to make a characteristic noise profile.  Make your own or just use one of the many presets.")]
        [FormerlySerializedAs("m_Definition")]
        [NoiseSettingsProperty]
        public NoiseSettings m_NoiseProfile;

        /// <summary>
        /// When rotating the camera, offset the camera's pivot position by this much (camera space)
        /// </summary>
        [Tooltip("When rotating the camera, offset the camera's pivot position by this much (camera space)")]
        public Vector3 m_PivotOffset = Vector3.zero;

        /// <summary>
        /// Gain to apply to the amplitudes defined in the settings asset.
        /// </summary>
        [Tooltip("Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  "
            + "Setting this to 0 completely mutes the noise.")]
        public float m_AmplitudeGain = 1f;

        /// <summary>
        /// Scale factor to apply to the frequencies defined in the settings asset.
        /// </summary>
        [Tooltip("Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  "
            + "Larger magnitudes will make the noise shake more rapidly.")]
        public float m_FrequencyGain = 1f;

        /// <summary>True if the component is valid, i.e. it has a noise definition and is enabled.</summary>
        public override bool IsValid { get { return enabled && m_NoiseProfile != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Noise stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Noise; } }

        /// <summary>Applies noise to the Correction channel of the CameraState if the
        /// delta time is greater than 0.  Otherwise, does nothing.</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">How much to advance the perlin noise generator.
        /// Noise is only applied if this value is greater than or equal to 0</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid || deltaTime < 0)
            {
                mInitialized = false;
                return;
            }

            if (!mInitialized)
                Initialize();

            if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback
                    && TargetPositionCache.HasCurrentTime)
                mNoiseTime = TargetPositionCache.CurrentTime * m_FrequencyGain;
            else
                mNoiseTime += deltaTime * m_FrequencyGain;
            curState.PositionCorrection += curState.CorrectedOrientation * NoiseSettings.GetCombinedFilterResults(
                    m_NoiseProfile.PositionNoise, mNoiseTime, mNoiseOffsets) * m_AmplitudeGain;
            Quaternion rotNoise = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(
                    m_NoiseProfile.OrientationNoise, mNoiseTime, mNoiseOffsets) * m_AmplitudeGain);
            if (m_PivotOffset != Vector3.zero)
            {
                Matrix4x4 m = Matrix4x4.Translate(-m_PivotOffset);
                m = Matrix4x4.Rotate(rotNoise) * m;
                m = Matrix4x4.Translate(m_PivotOffset) * m;
                curState.PositionCorrection += curState.CorrectedOrientation * m.MultiplyPoint(Vector3.zero);
            }
            curState.OrientationCorrection = curState.OrientationCorrection * rotNoise;
        }

        private bool mInitialized = false;
        private float mNoiseTime = 0;

        [SerializeField][HideInInspector]
        private Vector3 mNoiseOffsets = Vector3.zero;

        /// <summary>Generate a new random seed</summary>
        public void ReSeed()
        {
            mNoiseOffsets = new Vector3(
                    Random.Range(-1000f, 1000f),
                    Random.Range(-1000f, 1000f),
                    Random.Range(-1000f, 1000f));
        }

        void Initialize()
        {
            mInitialized = true;
            mNoiseTime = CinemachineCore.CurrentTime * m_FrequencyGain;
            if (mNoiseOffsets == Vector3.zero)
                ReSeed();
        }
    }
}
