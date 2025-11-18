#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#define CINEMACHINE_PHYSICS_2D
#endif

using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
#if CINEMACHINE_PHYSICS
    /// <summary>
    /// Cinemachine ClearShot is a "manager camera" that owns and manages a set of
    /// Virtual Camera gameObject children.  When Live, the ClearShot will check the
    /// children, and choose the one with the best quality shot and make it Live.
    ///
    /// This can be a very powerful tool.  If the child cameras have CinemachineCollider
    /// extensions, they will analyze the scene for target obstructions, optimal target
    /// distance, and other items, and report their assessment of shot quality back to
    /// the ClearShot parent, who will then choose the best one.  You can use this to set
    /// up complex multi-camera coverage of a scene, and be assured that a clear shot of
    /// the target will always be available.
    ///
    /// If multiple child cameras have the same shot quality, the one with the highest
    /// priority will be chosen.
    ///
    /// You can also define custom blends between the ClearShot children.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/CinemachineClearShot")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineClearShot.html")]
    public class CinemachineClearShot : CinemachineVirtualCameraBase
    {
        /// <summary>Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty.</summary>
        [Tooltip("Default object for the camera children to look at (the aim target), if not specified in a child camera.  May be empty if all children specify targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_LookAt = null;

        /// <summary>Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty.</summary>
        [Tooltip("Default object for the camera children wants to move with (the body target), if not specified in a child camera.  May be empty if all children specify targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_Follow = null;

        /// <summary>When enabled, the current camera and blend will be indicated in the game window, for debugging</summary>
        [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
        [NoSaveDuringPlay]
        public bool m_ShowDebugText = false;

        /// <summary>Internal API for the editor.  Do not use this filed.</summary>
        [SerializeField, HideInInspector, NoSaveDuringPlay]
        internal CinemachineVirtualCameraBase[] m_ChildCameras = null;

        /// <summary>Wait this many seconds before activating a new child camera</summary>
        [Tooltip("Wait this many seconds before activating a new child camera")]
        public float m_ActivateAfter;

        /// <summary>An active camera must be active for at least this many seconds</summary>
        [Tooltip("An active camera must be active for at least this many seconds")]
        public float m_MinDuration;

        /// <summary>If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order will be used</summary>
        [Tooltip("If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order and child camera priority will be used.")]
        public bool m_RandomizeChoice = false;

        /// <summary>The blend which is used if you don't explicitly define a blend between two Virtual Cameras</summary>
        [CinemachineBlendDefinitionProperty]
        [Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Cameras")]
        public CinemachineBlendDefinition m_DefaultBlend
            = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);

        /// <summary>This is the asset which contains custom settings for specific blends</summary>
        [HideInInspector]
        public CinemachineBlenderSettings m_CustomBlends = null;

        /// <summary>Gets a brief debug description of this virtual camera, for use when displayiong debug info</summary>
        public override string Description
        {
            get
            {
                // Show the active camera and blend
                if (mActiveBlend != null)
                    return mActiveBlend.Description;

                ICinemachineCamera vcam = LiveChild;
                if (vcam == null)
                    return "(none)";
                var sb = CinemachineDebug.SBFromPool();
                sb.Append("["); sb.Append(vcam.Name); sb.Append("]");
                string text = sb.ToString();
                CinemachineDebug.ReturnToPool(sb);
                return text;
            }
        }

        /// <summary>Get the current "best" child virtual camera, that would be chosen
        /// if the ClearShot camera were active.</summary>
        public ICinemachineCamera LiveChild { get; set; }

        /// <summary>The CameraState of the currently live child</summary>
        public override CameraState State { get { return m_State; } }

        /// <summary>Check whether the vcam a live child of this camera.</summary>
        /// <param name="vcam">The Virtual Camera to check</param>
        /// <param name="dominantChildOnly">If truw, will only return true if this vcam is the dominat live child</param>
        /// <returns>True if the vcam is currently actively influencing the state of this vcam</returns>
        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            return vcam == LiveChild || (mActiveBlend != null && mActiveBlend.Uses(vcam));
        }

        /// <summary>Get the current LookAt target.  Returns parent's LookAt if parent
        /// is non-null and no specific LookAt defined for this camera</summary>
        override public Transform LookAt
        {
            get { return ResolveLookAt(m_LookAt); }
            set { m_LookAt = value; }
        }

        /// <summary>Get the current Follow target.  Returns parent's Follow if parent
        /// is non-null and no specific Follow defined for this camera</summary>
        override public Transform Follow
        {
            get { return ResolveFollow(m_Follow); }
            set { m_Follow = value; }
        }

        /// <summary>This is called to notify the vcam that a target got warped,
        /// so that the vcam can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            UpdateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.OnTargetObjectWarped(target, positionDelta);
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            UpdateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.ForceCameraPosition(pos, rot);
            base.ForceCameraPosition(pos, rot);
        }
        
        /// <summary>Internal use only.  Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  This implementation
        /// updates all the children, chooses the best one, and implements any required blending.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            // Choose the best camera
            UpdateListOfChildren();
            ICinemachineCamera previousCam = LiveChild;
            LiveChild = ChooseCurrentCamera(worldUp);

            // Are we transitioning cameras?
            if (previousCam != LiveChild && LiveChild != null)
            {
                // Notify incoming camera of transition
                LiveChild.OnTransitionFromCamera(previousCam, worldUp, deltaTime);

                // Generate Camera Activation event in the brain if live
                CinemachineCore.Instance.GenerateCameraActivationEvent(LiveChild, previousCam);

                // Are we transitioning cameras?
                if (previousCam != null)
                {
                    // Create a blend (will be null if a cut)
                    mActiveBlend = CreateBlend(
                            previousCam, LiveChild,
                            LookupBlend(previousCam, LiveChild), mActiveBlend);

                    // If cutting, generate a camera cut event if live
                    if (mActiveBlend == null || !mActiveBlend.Uses(previousCam))
                        CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
                }
            }

            // Advance the current blend (if any)
            if (mActiveBlend != null)
            {
                mActiveBlend.TimeInBlend += (deltaTime >= 0)
                    ? deltaTime : mActiveBlend.Duration;
                if (mActiveBlend.IsComplete)
                    mActiveBlend = null;
            }

            if (mActiveBlend != null)
            {
                mActiveBlend.UpdateCameraState(worldUp, deltaTime);
                m_State =  mActiveBlend.State;
            }
            else if (LiveChild != null)
            {
                if (m_TransitioningFrom != null)
                    LiveChild.OnTransitionFromCamera(m_TransitioningFrom, worldUp, deltaTime);
                m_State =  LiveChild.State;
            }
            m_TransitioningFrom = null;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
            PreviousStateIsValid = true;
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateListOfChildren();
            mActiveBlend = null;
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
            CinemachineDebug.OnGUIHandlers += OnGuiHandler;
        }

        /// <summary>
        /// Uninstall the GUI handler
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        public void OnTransformChildrenChanged()
        {
            InvalidateListOfChildren();
            UpdateListOfChildren();
        }

        ///  Will only be called if Unity Editor - never in build
        private void OnGuiHandler()
        {
            if (!m_ShowDebugText)
                CinemachineDebug.ReleaseScreenPos(this);
            else
            {
                var sb = CinemachineDebug.SBFromPool();
                sb.Append(Name); sb.Append(": "); sb.Append(Description);
                string text = sb.ToString();
                Rect r = CinemachineDebug.GetScreenPos(this, text, GUI.skin.box);
                GUI.Label(r, text, GUI.skin.box);
                CinemachineDebug.ReturnToPool(sb);
            }
        }

        /// <summary>Is there a blend in progress?</summary>
        public bool IsBlending => mActiveBlend != null;

        /// <summary>
        /// Get the current active blend in progress.  Will return null if no blend is in progress.
        /// </summary>
        public CinemachineBlend ActiveBlend => mActiveBlend;

        CameraState m_State = CameraState.Default;

        /// <summary>The list of child cameras.  These are just the immediate children in the hierarchy.</summary>
        public CinemachineVirtualCameraBase[] ChildCameras
        {
            get { UpdateListOfChildren(); return m_ChildCameras; }
        }

        float mActivationTime = 0;
        float mPendingActivationTime = 0;
        ICinemachineCamera mPendingCamera;
        private CinemachineBlend mActiveBlend = null;

        void InvalidateListOfChildren()
        {
            m_ChildCameras = null;
            m_RandomizedChilden = null;
            LiveChild = null;
        }

        /// <summary>If RandomizeChoice is enabled, call this to re-randomize the children next frame.
        /// This is useful if you want to freshen up the shot.</summary>
        public void ResetRandomization()
        {
            m_RandomizedChilden = null;
            mRandomizeNow = true;
        }

        void UpdateListOfChildren()
        {
            if (m_ChildCameras != null)
                return;

            List<CinemachineVirtualCameraBase> list = new List<CinemachineVirtualCameraBase>();
            CinemachineVirtualCameraBase[] kids = GetComponentsInChildren<CinemachineVirtualCameraBase>(true);
            foreach (CinemachineVirtualCameraBase k in kids)
                if (k.transform.parent == transform)
                    list.Add(k);
            m_ChildCameras = list.ToArray();

            // Zap the cached current instructions
            mActivationTime = mPendingActivationTime = 0;
            mPendingCamera = null;
            LiveChild = null;
            mActiveBlend = null;
        }

        private bool mRandomizeNow = false;
        private  CinemachineVirtualCameraBase[] m_RandomizedChilden = null;

        private ICinemachineCamera ChooseCurrentCamera(Vector3 worldUp)
        {
            if (m_ChildCameras == null || m_ChildCameras.Length == 0)
            {
                mActivationTime = 0;
                return null;
            }

            CinemachineVirtualCameraBase[] childCameras = m_ChildCameras;
            if (!m_RandomizeChoice)
                m_RandomizedChilden = null;
            else if (m_ChildCameras.Length > 1)
            {
                if (m_RandomizedChilden == null)
                    m_RandomizedChilden = Randomize(m_ChildCameras);
                childCameras = m_RandomizedChilden;
            }

            if (LiveChild != null && !LiveChild.VirtualCameraGameObject.activeSelf)
                LiveChild = null;
            ICinemachineCamera best = LiveChild;
            for (int i = 0; i < childCameras.Length; ++i)
            {
                CinemachineVirtualCameraBase vcam = childCameras[i];
                if (vcam != null && vcam.gameObject.activeInHierarchy)
                {
                    // Choose the first in the list that is better than the current
                    if (best == null
                        || vcam.State.ShotQuality > best.State.ShotQuality
                        || (vcam.State.ShotQuality == best.State.ShotQuality && vcam.Priority > best.Priority)
                        || (m_RandomizeChoice && mRandomizeNow && (ICinemachineCamera)vcam != LiveChild
                            && vcam.State.ShotQuality == best.State.ShotQuality
                            && vcam.Priority == best.Priority))
                    {
                        best = vcam;
                    }
                }
            }
            mRandomizeNow = false;

            float now = CinemachineCore.CurrentTime;
            if (mActivationTime != 0)
            {
                // Is it active now?
                if (LiveChild == best)
                {
                    // Yes, cancel any pending
                    mPendingActivationTime = 0;
                    mPendingCamera = null;
                    return best;
                }

                // Is it pending?
                if (PreviousStateIsValid)
                {
                    if (mPendingActivationTime != 0 && mPendingCamera == best)
                    {
                        // Has it been pending long enough, and are we allowed to switch away
                        // from the active action?
                        if ((now - mPendingActivationTime) > m_ActivateAfter
                            && (now - mActivationTime) > m_MinDuration)
                        {
                            // Yes, activate it now
                            m_RandomizedChilden = null; // reshuffle the children
                            mActivationTime = now;
                            mPendingActivationTime = 0;
                            mPendingCamera = null;
                            return best;
                        }
                        return LiveChild;
                    }
                }
            }
            // Neither active nor pending.
            mPendingActivationTime = 0; // cancel the pending, if any
            mPendingCamera = null;

            // Can we activate it now?
            if (PreviousStateIsValid && mActivationTime > 0)
            {
                if (m_ActivateAfter > 0
                    || (now - mActivationTime) < m_MinDuration)
                {
                    // Too early - make it pending
                    mPendingCamera = best;
                    mPendingActivationTime = now;
                    return LiveChild;
                }
            }
            // Activate now
            m_RandomizedChilden = null; // reshuffle the children
            mActivationTime = now;
            return best;
        }

        struct Pair { public int a; public float b; }
        CinemachineVirtualCameraBase[] Randomize(CinemachineVirtualCameraBase[] src)
        {
            List<Pair> pairs = new List<Pair>();
            for (int i = 0; i < src.Length; ++i)
            {
                Pair p = new Pair(); p.a = i; p.b = Random.Range(0, 1000f);
                pairs.Add(p);
            }
            pairs.Sort((p1, p2) => (int)p1.b - (int)p2.b);
            CinemachineVirtualCameraBase[] dst = new CinemachineVirtualCameraBase[src.Length];
            Pair[] result = pairs.ToArray();
            for (int i = 0; i < src.Length; ++i)
                dst[i] = src[result[i].a];
            return dst;
        }

        private CinemachineBlendDefinition LookupBlend(
            ICinemachineCamera fromKey, ICinemachineCamera toKey)
        {
            // Get the blend curve that's most appropriate for these cameras
            CinemachineBlendDefinition blend = m_DefaultBlend;
            if (m_CustomBlends != null)
            {
                string fromCameraName = (fromKey != null) ? fromKey.Name : string.Empty;
                string toCameraName = (toKey != null) ? toKey.Name : string.Empty;
                blend = m_CustomBlends.GetBlendForVirtualCameras(
                        fromCameraName, toCameraName, blend);
            }
            if (CinemachineCore.GetBlendOverride != null)
                blend = CinemachineCore.GetBlendOverride(fromKey, toKey, blend, this);
            return blend;
        }

        /// <summary>Notification that this virtual camera is going live.
        /// This implementation resets the child randomization.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            m_TransitioningFrom = fromCam;
            if (m_RandomizeChoice && mActiveBlend == null)
            {
                m_RandomizedChilden = null;
                LiveChild = null;
            }
            InternalUpdateCameraState(worldUp, deltaTime);
        }

        ICinemachineCamera m_TransitioningFrom;
    }
#endif
}
