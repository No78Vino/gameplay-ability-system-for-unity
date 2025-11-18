using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Cinemachine
{
    internal static class Documentation 
    {
        /// <summary>This must be used like
        /// [HelpURL(Documentation.BaseURL + "api/some-page.html")]
        /// or
        /// [HelpURL(Documentation.BaseURL + "manual/some-page.html")]
        /// It cannot support String.Format nor string interpolation</summary>
        public const string BaseURL = "https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/";
    }

    /// <summary>A singleton that manages complete lists of CinemachineBrain and,
    /// Cinemachine Virtual Cameras, and the priority queue.  Provides
    /// services to keeping track of whether Cinemachine Virtual Cameras have
    /// been updated each frame.</summary>
    public sealed class CinemachineCore
    {
        /// <summary>Data version string.  Used to upgrade from legacy projects</summary>
        public static readonly int kStreamingVersion = 20170927;

        /// <summary>Human-readable Cinemachine Version</summary>
        public static readonly string kVersionString = "2.9.0";

        /// <summary>
        /// Stages in the Cinemachine Component pipeline, used for
        /// UI organization>.  This enum defines the pipeline order.
        /// </summary>
        public enum Stage
        {
            /// <summary>Second stage: position the camera in space</summary>
            Body,

            /// <summary>Third stage: orient the camera to point at the target</summary>
            Aim,

            /// <summary>Final pipeline stage: apply noise (this is done separately, in the
            /// Correction channel of the CameraState)</summary>
            Noise,

            /// <summary>Post-correction stage.  This is invoked on all virtual camera
            /// types, after the pipeline is complete</summary>
            Finalize
        };

        private static CinemachineCore sInstance = null;

        /// <summary>Get the singleton instance</summary>
        public static CinemachineCore Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new CinemachineCore();
                return sInstance;
            }
        }

        /// <summary>
        /// If true, show hidden Cinemachine objects, to make manual script mapping possible.
        /// </summary>
        public static bool sShowHiddenObjects = false;

        /// <summary>Delegate for overriding Unity's default input system.  Returns the value
        /// of the named axis.</summary>
        public delegate float AxisInputDelegate(string axisName);

        /// <summary>Delegate for overriding Unity's default input system.
        /// If you set this, then your delegate will be called instead of
        /// System.Input.GetAxis(axisName) whenever in-game user input is needed.</summary>
#if ENABLE_LEGACY_INPUT_MANAGER
        public static AxisInputDelegate GetInputAxis = UnityEngine.Input.GetAxis;
#else
        public static AxisInputDelegate GetInputAxis = delegate { return 0; };
#endif

        /// <summary>
        /// If non-negative, cinemachine will update with this uniform delta time.
        /// Usage is for timelines in manual update mode.
        /// </summary>
        public static float UniformDeltaTimeOverride = -1;

        /// <summary>
        /// Replacement for Time.deltaTime, taking UniformDeltaTimeOverride into account.
        /// </summary>
        public static float DeltaTime => UniformDeltaTimeOverride >= 0 ? UniformDeltaTimeOverride : Time.deltaTime;

        /// <summary>
        /// If non-negative, cinemachine willuse this value whenever it wants current game time.
        /// Usage is for master timelines in manual update mode, for deterministic behaviour.
        /// </summary>
        public static float CurrentTimeOverride = -1;

        /// <summary>
        /// Replacement for Time.time, taking CurrentTimeTimeOverride into account.
        /// </summary>
        public static float CurrentTime => CurrentTimeOverride >= 0 ? CurrentTimeOverride : Time.time;

        /// <summary>
        /// Delegate for overriding a blend that is about to be applied to a transition.
        /// A handler can either return the default blend, or a new blend specific to
        /// current conditions.
        /// </summary>
        /// <param name="fromVcam">The outgoing virtual camera</param>
        /// <param name="toVcam">Yhe incoming virtual camera</param>
        /// <param name="defaultBlend">The blend that would normally be applied</param>
        /// <param name="owner">The context in which the blend is taking place.
        /// Can be a CinemachineBrain, or CinemachineStateDrivenCamera, or other manager
        /// object that can initiate a blend</param>
        /// <returns>The blend definition to use for this transition.</returns>
        public delegate CinemachineBlendDefinition GetBlendOverrideDelegate(
            ICinemachineCamera fromVcam, ICinemachineCamera toVcam,
            CinemachineBlendDefinition defaultBlend,
            MonoBehaviour owner);

        /// <summary>
        /// Delegate for overriding a blend that is about to be applied to a transition.
        /// A handler can either return the default blend, or a new blend specific to
        /// current conditions.
        /// </summary>
        public static GetBlendOverrideDelegate GetBlendOverride;

        /// <summary>This event will fire after a brain updates its Camera</summary>
        public static CinemachineBrain.BrainEvent CameraUpdatedEvent = new CinemachineBrain.BrainEvent();

        /// <summary>This event will fire after a brain updates its Camera</summary>
        public static CinemachineBrain.BrainEvent CameraCutEvent = new CinemachineBrain.BrainEvent();

        /// <summary>List of all active CinemachineBrains.</summary>
        private List<CinemachineBrain> mActiveBrains = new List<CinemachineBrain>();

        /// <summary>Access the array of active CinemachineBrains in the scene</summary>
        public int BrainCount { get { return mActiveBrains.Count; } }

        /// <summary>Access the array of active CinemachineBrains in the scene
        /// without generating garbage</summary>
        /// <param name="index">Index of the brain to access, range 0-BrainCount</param>
        /// <returns>The brain at the specified index</returns>
        public CinemachineBrain GetActiveBrain(int index)
        {
            return mActiveBrains[index];
        }

        /// <summary>Called when a CinemachineBrain is enabled.</summary>
        internal void AddActiveBrain(CinemachineBrain brain)
        {
            // First remove it, just in case it's being added twice
            RemoveActiveBrain(brain);
            mActiveBrains.Insert(0, brain);
        }

        /// <summary>Called when a CinemachineBrain is disabled.</summary>
        internal void RemoveActiveBrain(CinemachineBrain brain)
        {
            mActiveBrains.Remove(brain);
        }

        /// <summary>List of all active ICinemachineCameras.</summary>
        private List<CinemachineVirtualCameraBase> mActiveCameras = new List<CinemachineVirtualCameraBase>();

        private bool m_ActiveCamerasAreSorted;
        private int m_ActivationSequence;
        
        /// <summary>
        /// List of all active Cinemachine Virtual Cameras for all brains.
        /// This list is kept sorted by priority.
        /// </summary>
        public int VirtualCameraCount { get { return mActiveCameras.Count; } }

        /// <summary>Access the priority-sorted array of active ICinemachineCamera in the scene
        /// without generating garbage</summary>
        /// <param name="index">Index of the camera to access, range 0-VirtualCameraCount</param>
        /// <returns>The virtual camera at the specified index</returns>
        public CinemachineVirtualCameraBase GetVirtualCamera(int index)
        {
            if (!m_ActiveCamerasAreSorted && mActiveCameras.Count > 1)
            {
                mActiveCameras.Sort((x, y) => 
                    x.Priority == y.Priority ? y.m_ActivationId - x.m_ActivationId : y.Priority - x.Priority);
                m_ActiveCamerasAreSorted = true;
            }
            return mActiveCameras[index];
        }

        /// <summary>Called when a Cinemachine Virtual Camera is enabled.</summary>
        internal void AddActiveCamera(CinemachineVirtualCameraBase vcam)
        {
            Assert.IsFalse(mActiveCameras.Contains(vcam));
            vcam.m_ActivationId = m_ActivationSequence++;
            mActiveCameras.Add(vcam);
            m_ActiveCamerasAreSorted = false;
        }

        /// <summary>Called when a Cinemachine Virtual Camera is disabled.</summary>
        internal void RemoveActiveCamera(CinemachineVirtualCameraBase vcam)
        {
            if (mActiveCameras.Contains(vcam))
                mActiveCameras.Remove(vcam);
        }

        /// <summary>Called when a Cinemachine Virtual Camera is destroyed.</summary>
        internal void CameraDestroyed(CinemachineVirtualCameraBase vcam)
        {
            if (mActiveCameras.Contains(vcam))
                mActiveCameras.Remove(vcam);
            if (mUpdateStatus != null && mUpdateStatus.ContainsKey(vcam))
                mUpdateStatus.Remove(vcam);
        }

        // Registry of all vcams that are present, active or not
        private List<List<CinemachineVirtualCameraBase>> mAllCameras
            = new List<List<CinemachineVirtualCameraBase>>();

        /// <summary>Called when a vcam is enabled.</summary>
        internal void CameraEnabled(CinemachineVirtualCameraBase vcam)
        {
            int parentLevel = 0;
            for (ICinemachineCamera p = vcam.ParentCamera; p != null; p = p.ParentCamera)
                ++parentLevel;
            while (mAllCameras.Count <= parentLevel)
                mAllCameras.Add(new List<CinemachineVirtualCameraBase>());
            mAllCameras[parentLevel].Add(vcam);
        }

        /// <summary>Called when a vcam is disabled.</summary>
        internal void CameraDisabled(CinemachineVirtualCameraBase vcam)
        {
            for (int i = 0; i < mAllCameras.Count; ++i)
                mAllCameras[i].Remove(vcam);
            if (mRoundRobinVcamLastFrame == vcam)
                mRoundRobinVcamLastFrame = null;
        }

        CinemachineVirtualCameraBase mRoundRobinVcamLastFrame = null;

        static float s_LastUpdateTime;
        static int s_FixedFrameCount; // Current fixed frame count

        /// <summary>Update all the active vcams in the scene, in the correct dependency order.</summary>
        internal void UpdateAllActiveVirtualCameras(int layerMask, Vector3 worldUp, float deltaTime)
        {
            // Setup for roundRobin standby updating
            var filter = m_CurrentUpdateFilter;
            bool canUpdateStandby = (filter != UpdateFilter.SmartFixed); // never in smart fixed
            CinemachineVirtualCameraBase currentRoundRobin = mRoundRobinVcamLastFrame;

            // Update the fixed frame count
            float now = CinemachineCore.CurrentTime;
            if (now != s_LastUpdateTime)
            {
                s_LastUpdateTime = now;
                if ((filter & ~UpdateFilter.Smart) == UpdateFilter.Fixed)
                    ++s_FixedFrameCount;
            }

            // Update the leaf-most cameras first
            for (int i = mAllCameras.Count-1; i >= 0; --i)
            {
                var sublist = mAllCameras[i];
                for (int j = sublist.Count - 1; j >= 0; --j)
                {
                    var vcam = sublist[j];
                    if (canUpdateStandby && vcam == mRoundRobinVcamLastFrame)
                        currentRoundRobin = null; // update the next roundrobin candidate
                    if (vcam == null)
                    {
                        sublist.RemoveAt(j);
                        continue; // deleted
                    }
                    if (vcam.m_StandbyUpdate == CinemachineVirtualCameraBase.StandbyUpdateMode.Always
                        || IsLive(vcam))
                    {
                        // Skip this vcam if it's not on the layer mask
                        if (((1 << vcam.gameObject.layer) & layerMask) != 0)
                            UpdateVirtualCamera(vcam, worldUp, deltaTime);
                    }
                    else if (currentRoundRobin == null
                        && mRoundRobinVcamLastFrame != vcam
                        && canUpdateStandby
                        && vcam.m_StandbyUpdate != CinemachineVirtualCameraBase.StandbyUpdateMode.Never
                        && vcam.isActiveAndEnabled)
                    {
                        // Do the round-robin update
                        m_CurrentUpdateFilter &= ~UpdateFilter.Smart; // force it
                        UpdateVirtualCamera(vcam, worldUp, deltaTime);
                        m_CurrentUpdateFilter = filter;
                        currentRoundRobin = vcam;
                    }
                }
            }

            // Did we manage to update a roundrobin?
            if (canUpdateStandby)
            {
                if (currentRoundRobin == mRoundRobinVcamLastFrame)
                    currentRoundRobin = null; // take the first candidate
                mRoundRobinVcamLastFrame = currentRoundRobin;
            }
        }

        /// <summary>
        /// Update a single Cinemachine Virtual Camera if and only if it
        /// hasn't already been updated this frame.  Always update vcams via this method.
        /// Calling this more than once per frame for the same camera will have no effect.
        /// </summary>
        internal void UpdateVirtualCamera(
            CinemachineVirtualCameraBase vcam, Vector3 worldUp, float deltaTime)
        {
            if (vcam == null)
                return;

            bool isSmartUpdate = (m_CurrentUpdateFilter & UpdateFilter.Smart) == UpdateFilter.Smart;
            UpdateTracker.UpdateClock updateClock
                = (UpdateTracker.UpdateClock)(m_CurrentUpdateFilter & ~UpdateFilter.Smart);

            // If we're in smart update mode and the target moved, then we must examine
            // how the target has been moving recently in order to figure out whether to
            // update now
            if (isSmartUpdate)
            {
                Transform updateTarget = GetUpdateTarget(vcam);
                if (updateTarget == null)
                    return;   // vcam deleted
                if (UpdateTracker.GetPreferredUpdate(updateTarget) != updateClock)
                    return;   // wrong clock
            }

            // Have we already been updated this frame?
            if (mUpdateStatus == null)
                mUpdateStatus = new Dictionary<CinemachineVirtualCameraBase, UpdateStatus>();
            if (!mUpdateStatus.TryGetValue(vcam, out UpdateStatus status))
            {
                status = new UpdateStatus
                {
                    lastUpdateDeltaTime = -2,
                    lastUpdateMode = UpdateTracker.UpdateClock.Late,
                    lastUpdateFrame = Time.frameCount + 2, // so that frameDelta ends up negative
                    lastUpdateFixedFrame = s_FixedFrameCount + 2
                };
                mUpdateStatus.Add(vcam, status);
            }
            int frameDelta = (updateClock == UpdateTracker.UpdateClock.Late)
                ? Time.frameCount - status.lastUpdateFrame
                : s_FixedFrameCount - status.lastUpdateFixedFrame;
            if (deltaTime >= 0)
            {
                if (frameDelta == 0 && status.lastUpdateMode == updateClock
                        && status.lastUpdateDeltaTime == deltaTime)
                    return; // already updated
                if (frameDelta > 0)
                    deltaTime *= frameDelta; // try to catch up if multiple frames
            }

//Debug.Log((vcam.ParentCamera == null ? "" : vcam.ParentCamera.Name + ".") + vcam.Name + ": frame " + Time.frameCount + "/" + status.lastUpdateFixedFrame + ", " + CurrentUpdateFilter + ", deltaTime = " + deltaTime);
            vcam.InternalUpdateCameraState(worldUp, deltaTime);
            status.lastUpdateFrame = Time.frameCount;
            status.lastUpdateFixedFrame = s_FixedFrameCount;
            status.lastUpdateMode = updateClock;
            status.lastUpdateDeltaTime = deltaTime;
        }

        class UpdateStatus
        {
            public int lastUpdateFrame;
            public int lastUpdateFixedFrame;
            public UpdateTracker.UpdateClock lastUpdateMode;
            public float lastUpdateDeltaTime;
        }
        Dictionary<CinemachineVirtualCameraBase, UpdateStatus> mUpdateStatus;

        [RuntimeInitializeOnLoadMethod]
        static void InitializeModule()
        {
            CinemachineCore.Instance.mUpdateStatus = new Dictionary<CinemachineVirtualCameraBase, UpdateStatus>();
        }

        /// <summary>Internal use only</summary>
        internal enum UpdateFilter
        {
            Fixed = UpdateTracker.UpdateClock.Fixed,
            Late = UpdateTracker.UpdateClock.Late,
            Smart = 8, // meant to be or'ed with the others
            SmartFixed = Smart | Fixed,
            SmartLate = Smart | Late
        }
        internal UpdateFilter m_CurrentUpdateFilter;

        private static Transform GetUpdateTarget(CinemachineVirtualCameraBase vcam)
        {
            if (vcam == null || vcam.gameObject == null)
                return null;
            Transform target = vcam.LookAt;
            if (target != null)
                return target;
            target = vcam.Follow;
            if (target != null)
                return target;
            // If no target, use the vcam itself
            return vcam.transform;
        }

        /// <summary>Internal use only - inspector</summary>
        internal UpdateTracker.UpdateClock GetVcamUpdateStatus(CinemachineVirtualCameraBase vcam)
        {
            UpdateStatus status;
            if (mUpdateStatus == null || !mUpdateStatus.TryGetValue(vcam, out status))
                return UpdateTracker.UpdateClock.Late;
            return status.lastUpdateMode;
        }

        /// <summary>
        /// Is this virtual camera currently actively controlling any Camera?
        /// </summary>
        /// <param name="vcam">The virtual camea in question</param>
        /// <returns>True if the vcam is currently driving a Brain</returns>
        public bool IsLive(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                for (int i = 0; i < BrainCount; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.IsLive(vcam))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the vcam is live as part of an outgoing blend in any active CinemachineBrain.  
        /// Does not check whether the vcam is also the current active vcam.
        /// </summary>
        /// <param name="vcam">The virtual camera to check</param>
        /// <returns>True if the virtual camera is part of a live outgoing blend, false otherwise</returns>
        public bool IsLiveInBlend(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                for (int i = 0; i < BrainCount; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.IsLiveInBlend(vcam))
                        return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Signal that the virtual has been activated.
        /// If the camera is live, then all CinemachineBrains that are showing it will
        /// send an activation event.
        /// </summary>
        /// <param name="vcam">The virtual camera being activated</param>
        /// <param name="vcamFrom">The previously-active virtual camera (may be null)</param>
        public void GenerateCameraActivationEvent(ICinemachineCamera vcam, ICinemachineCamera vcamFrom)
        {
            if (vcam != null)
            {
                for (int i = 0; i < BrainCount; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.IsLive(vcam))
                        b.m_CameraActivatedEvent.Invoke(vcam, vcamFrom);
                }
            }
        }

        /// <summary>
        /// Signal that the virtual camera's content is discontinuous WRT the previous frame.
        /// If the camera is live, then all CinemachineBrains that are showing it will send a cut event.
        /// </summary>
        /// <param name="vcam">The virtual camera being cut to</param>
        public void GenerateCameraCutEvent(ICinemachineCamera vcam)
        {
            if (vcam != null)
            {
                for (int i = 0; i < BrainCount; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.IsLive(vcam))
                    {
                        if (b.m_CameraCutEvent != null)
                            b.m_CameraCutEvent.Invoke(b);
                        if (CameraCutEvent != null)
                            CameraCutEvent.Invoke(b);
                    }
                }
            }
        }

        /// <summary>
        /// Try to find a CinemachineBrain to associate with a
        /// Cinemachine Virtual Camera.  The first CinemachineBrain
        /// in which this Cinemachine Virtual Camera is live will be used.
        /// If none, then the first active CinemachineBrain with the correct
        /// layer filter will be used.
        /// Brains with OutputCamera == null will not be returned.
        /// Final result may be null.
        /// </summary>
        /// <param name="vcam">Virtual camera whose potential brain we need.</param>
        /// <returns>First CinemachineBrain found that might be
        /// appropriate for this vcam, or null</returns>
        public CinemachineBrain FindPotentialTargetBrain(CinemachineVirtualCameraBase vcam)
        {
            if (vcam != null)
            {
                int numBrains = BrainCount;
                for (int i = 0; i < numBrains; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.OutputCamera != null && b.IsLive(vcam))
                        return b;
                }
                int layer = 1 << vcam.gameObject.layer;
                for (int i = 0; i < numBrains; ++i)
                {
                    CinemachineBrain b = GetActiveBrain(i);
                    if (b != null && b.OutputCamera != null && (b.OutputCamera.cullingMask & layer) != 0)
                        return b;
                }
            }
            return null;
        }

        /// <summary>Call this to notify all virtual camewras that may be tracking a target
        /// that the target's position has suddenly warped to somewhere else, so that
        /// the virtual cameras can update their internal state to make the camera
        /// warp seamlessy along with the target.
        /// 
        /// All virtual cameras are iterated so this call will work no matter how many 
        /// are tracking the target, and whether they are active or inactive.
        /// </summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            int numVcams = VirtualCameraCount;
            for (int i = 0; i < numVcams; ++i)
                GetVirtualCamera(i).OnTargetObjectWarped(target, positionDelta);
        }
    }
}
