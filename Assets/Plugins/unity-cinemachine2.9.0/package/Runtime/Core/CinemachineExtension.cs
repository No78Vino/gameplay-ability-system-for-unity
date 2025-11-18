using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// Base class for a Cinemachine Virtual Camera extension module.
    /// Hooks into the Cinemachine Pipeline.  Use this to add extra processing 
    /// to the vcam, modifying its generated state
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.API)]
    public abstract class CinemachineExtension : MonoBehaviour
    {
        /// <summary>Useful constant for very small floats</summary>
        protected const float Epsilon = Utility.UnityVectorExtensions.Epsilon;

        /// <summary>Get the CinemachineVirtualCamera to which this extension is attached</summary>
        public CinemachineVirtualCameraBase VirtualCamera
        {
            get
            {
                if (m_vcamOwner == null)
                    m_vcamOwner = GetComponent<CinemachineVirtualCameraBase>();
                return m_vcamOwner;
            }
        }
        CinemachineVirtualCameraBase m_vcamOwner;

        /// <summary>Connect to virtual camera pipeline.
        /// Override implementations must call this base implementation</summary>
        protected virtual void Awake()
        {
            ConnectToVcam(true);
        }

        /// <summary>Does nothing.  It's here for the little checkbox in the inspector.</summary>
        protected virtual void OnEnable() {}

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptReload()
        {
            var extensions = Resources.FindObjectsOfTypeAll(
                typeof(CinemachineExtension)) as CinemachineExtension[];
            foreach (var e in extensions)
                e.ConnectToVcam(true);
        }
#endif
        /// <summary>Disconnect from virtual camera pipeline.
        /// Override implementations must call this base implementation</summary>
        protected virtual void OnDestroy()
        {
            ConnectToVcam(false);
        }

        internal void EnsureStarted() { ConnectToVcam(true); }

        /// <summary>Connect to virtual camera.  Implementation must be safe to be called
        /// redundantly.  Override implementations must call this base implementation</summary>
        /// <param name="connect">True if connecting, false if disconnecting</param>
        protected virtual void ConnectToVcam(bool connect)
        {
            if (connect && VirtualCamera == null)
                Debug.LogError("CinemachineExtension requires a Cinemachine Virtual Camera component");
            if (VirtualCamera != null)
            {
                if (connect)
                    VirtualCamera.AddExtension(this);
                else
                    VirtualCamera.RemoveExtension(this);
            }
            mExtraState = null;
        }

        /// <summary>Override this to do such things as offset the RefereceLookAt.
        /// Base class implementation does nothing.</summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="curState">Input state that must be mutated</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        public virtual void PrePipelineMutateCameraStateCallback(
            CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime) {}

        /// <summary>Legacy support.  This is only here to avoid changing the API
        /// to make PostPipelineStageCallback() public</summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        public void InvokePostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            PostPipelineStageCallback(vcam, stage, ref state, deltaTime);
        }

        /// <summary>
        /// This callback will be called after the virtual camera has implemented
        /// each stage in the pipeline.  This method may modify the referenced state.
        /// If deltaTime less than 0, reset all state info and perform no damping.
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected abstract void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime);

        /// <summary>This is called to notify the extension that a target got warped,
        /// so that the extension can update its internal state to make the camera
        /// also warp seamlessy.  Base class implementation does nothing.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public virtual void OnTargetObjectWarped(Transform target, Vector3 positionDelta) {}

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public virtual void ForceCameraPosition(Vector3 pos, Quaternion rot) {}
        
        /// <summary>Notification that this virtual camera is going live.
        /// Base class implementation must be called by any overridden method.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        /// <returns>True to request a vcam update of internal state</returns>
        public virtual bool OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime) { return false; }

        /// <summary>
        /// Report maximum damping time needed for this extension.
        /// Only used in editor for timeline scrubbing.
        /// </summary>
        /// <returns>Highest damping setting in this extension</returns>
        public virtual float GetMaxDampTime() { return 0; }
        
        /// <summary>Extensions that require user input should implement this and return true.</summary>
        public virtual bool RequiresUserInput => false;

        /// <summary>Because extensions can be placed on manager cams and will in that
        /// case be called for all the vcam children, vcam-specific state information
        /// should be stored here.  Just define a class to hold your state info
        /// and use it exclusively when calling this.</summary>
        /// /// <typeparam name="T">The type of the extra state class</typeparam>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <returns>The extra state, cast as type T</returns>
        protected T GetExtraState<T>(ICinemachineCamera vcam) where T : class, new()
        {
            if (mExtraState == null)
                mExtraState = new Dictionary<ICinemachineCamera, System.Object>();
            System.Object extra = null;
            if (!mExtraState.TryGetValue(vcam, out extra))
                extra = mExtraState[vcam] = new T();
            return extra as T;
        }

        /// <summary>Inefficient method to get all extra state info for all vcams.
        /// Intended for Editor use only, not runtime!
        /// </summary>
        /// <typeparam name="T">The extra state type</typeparam>
        /// <returns>A dynamically-allocated list with all the extra states</returns>
        protected List<T> GetAllExtraStates<T>() where T : class, new()
        {
            var list = new List<T>();
            if (mExtraState != null)
                foreach (var v in mExtraState)
                    list.Add(v.Value as T);
            return list;
        }

        private Dictionary<ICinemachineCamera, System.Object> mExtraState;
    }
}
